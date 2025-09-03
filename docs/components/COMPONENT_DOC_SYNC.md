# Component Documentation Sync

This repository includes many Blazor components (.razor). Keep component docs in sync using the provided script.

How to run

Open PowerShell from the repo root and run:

powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\check_component_docs.ps1

What it does
- Scans `src/` for `.razor` files (excluding `obj` and `bin` folders)
- Looks for `.md` files in `docs/components` and `src` component folders whose basename matches or contains the component name
- Prints a table of components that appear to be missing docs

Checklist to add docs
- For each missing component:
  - Create `docs/components/<module>/<ComponentName>.md`
  - Include Purpose, Parameters, Example usage
  - Add an entry in the module index (`docs/components/<module>/README.md` or module index file)
  - Add the module index to `docs/components/README.md` if it's a new module

Automation ideas (future)
- Auto-generate stub docs for missing components
- Run the script in CI as a documentation check
- Add a pre-commit hook to warn about undocumented components

If you'd like, I can:
- Auto-generate stub `.md` files for the missing components reported by the script
- Add a CI job that runs the script and fails on missing docs
