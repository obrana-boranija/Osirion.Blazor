# OsirionBreadcrumbs

Purpose
SEO- and a11y-friendly breadcrumb trail from a URL/path.

Key parameters
- Path: source path to parse. Example: "/blog/blazor/osirion"
- ShowHome (default true), HomeText ("Home"), HomeUrl ("/")
- LinkLastItem (default false)
- UrlPrefix (default "/")
- SegmentFormatter: Func<string, string> to transform segment caption (default converts slug to Title Case)

Usage
- Provide Path from NavigationManager.Uri or your routing context
- Customize SegmentFormatter for product codes or localized labels

Notes
- Emits osirion-breadcrumbs plus custom Class
- Use inside nav[aria-label="breadcrumb"] when embedding in complex layout.
