#!/usr/bin/env bash
# Times the getting started walkthrough for one template as a new user would run it, on a simulated clean machine: an
# empty NuGet cache, its own template store and a fresh folder outside the repository. Osirion's packages come from a
# folder of packed packages, so a version not yet on nuget.org can be checked; everything else comes from nuget.org.
# Then it requests each given address and prints its status, title and first heading.
#
# Pack first:   for p in src/*/*.csproj; do dotnet pack "$p" -c Release -o <feed folder>; done
# Usage:        tools/verify-template.sh <template> <project name> <port> <feed folder> <work folder> [address...]
# Example:      tools/verify-template.sh osirion-blog MyBlog 5181 /tmp/feed /tmp/run / /blog /feed.xml /no-such-page
#
# The work folder is created fresh; pass a folder that holds nothing else. Set KEEP_RUNNING=1 to leave the site
# running afterwards, for a browser check.
set -euo pipefail

template=${1:?template short name}
name=${2:?project name}
port=${3:?port}
# NuGet needs a native path: on Git Bash `pwd -W` gives D:/..., elsewhere `pwd` is already native.
feed=$(cd "${4:?feed folder}" && (pwd -W 2>/dev/null || pwd))
work=${5:?work folder}
shift 5

version=$(sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' "$(dirname "$0")/../src/Osirion.Blazor.Templates/Osirion.Blazor.Templates.csproj")
package="$feed/Osirion.Blazor.Templates.$version.nupkg"
[ -f "$package" ] || { echo "Not found: $package; pack the templates first." >&2; exit 1; }

if [ -e "$work" ]; then
  echo "The work folder $work exists; pass a new folder." >&2
  exit 1
fi
mkdir -p "$work"
work=$(cd "$work" && pwd)

export NUGET_PACKAGES="$work/nuget-cache"
# NuGet's HTTP cache too, or packages downloaded by an earlier run make this one look faster.
export NUGET_HTTP_CACHE_PATH="$work/http-cache"
export DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1
cat > "$work/nuget.config" <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="osirion-packed" value="$feed" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
EOF

cd "$work"
start=$(date +%s)
step() { echo "[$(( $(date +%s) - start ))s] $*"; }

step "dotnet new install"
dotnet new install "$package" --debug:custom-hive "$work/hive" > "$work/install.log" 2>&1

step "dotnet new $template -n $name"
dotnet new "$template" -n "$name" --debug:custom-hive "$work/hive" > "$work/new.log" 2>&1
if ! grep -qi "restore succeeded" "$work/new.log"; then
  echo "The restore failed; see $work/new.log." >&2
  exit 1
fi

cd "$name"
step "dotnet run"
# The launch profile sets Development, which serves the packages' static files from the NuGet cache; so does this.
ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_URLS="http://localhost:$port" \
  dotnet run --no-launch-profile > "$work/run.log" 2>&1 &
run_pid=$!

for _ in $(seq 1 300); do
  if curl -s -o /dev/null "http://localhost:$port/"; then break; fi
  if ! kill -0 "$run_pid" 2>/dev/null; then
    echo "dotnet run stopped; see $work/run.log." >&2
    exit 1
  fi
  sleep 1
done
step "first page served"
echo "TOTAL_SECONDS=$(( $(date +%s) - start ))"

for address in "$@"; do
  status=$(curl -s -o "$work/page.html" -w "%{http_code}" "http://localhost:$port$address")
  title=$(grep -o '<title>[^<]*</title>' "$work/page.html" | head -1 || true)
  heading=$(grep -o '<h1[^>]*>[^<]*' "$work/page.html" | head -1 | sed 's/<h1[^>]*>//' || true)
  echo "  $status $address $title h1=[$heading]"
done

if grep -qE " fail:|crit:" "$work/run.log"; then
  echo "Errors in the server log ($work/run.log):"
  grep -E " fail:|crit:" "$work/run.log" | head -5
fi

if [ "${KEEP_RUNNING:-0}" = 1 ]; then
  echo "Still running on http://localhost:$port (process $run_pid)."
  exit 0
fi
kill "$run_pid" 2>/dev/null || true
