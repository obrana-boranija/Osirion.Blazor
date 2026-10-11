# DefaultNavigation

Purpose
Default admin navigation used in the CMS admin layout. It links to the routed admin pages and marks the link
whose route matches the current address as active.

Parameters
- `Expanded`: bool - Navigation expanded state (default: `true`)

Links
- Overview: `/osirion` (also active on `/osirion/dashboard`)
- Content Browser: `/osirion/content`
- New Content: `/osirion/content/edit`
- General Settings: `/osirion/settings`
- Documentation: the project repository on GitHub, opened in a new tab

The active match is exact and ignores letter case, a trailing slash, the query string and the fragment, so only
one link is active on any admin page.

Example

```razor
<DefaultNavigation />
```
