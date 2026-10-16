# EditContent

Purpose
Admin page that hosts the `ContentEditor` for one Markdown file of the connected GitHub repository. It loads the file
named in the `Path` query value, or follows the item selected in the content browser, and shows an empty state with
"Create New Post" and "Browse Content" buttons when nothing is being edited. After a save or a discard it returns to
the content browser.

Key parameters
- Route: `/osirion/content/edit`
- Path: `string?`, `[SupplyParameterFromQuery]`, so the page is reached as `/osirion/content/edit?Path=blog/post.md`.
  The file is fetched once per distinct value. When the shared admin state already holds an editing post with the same
  path (for example one opened from the content browser), the file is not fetched again.
- Without `Path`, after the first render the page navigates to `/osirion/content/edit?Path={path}` using the item
  selected in the content browser when it is a Markdown file, or else the post being edited when it has a path and is
  not a new file. Otherwise it stays on the page.
- Class, Style, Theme and Attributes are also declared by the component base (`OsirionComponentBase`); the page markup
  does not render them

Usage
```razor
@inject NavigationManager Navigation

<button type="button" @onclick="@(() => Navigation.NavigateTo("/osirion/content/edit?Path=blog/hello-world.md"))">
    Edit hello-world
</button>
```

Notes
- Declares `@rendermode InteractiveServer`, `@layout OsirionAdminLayout` and
  `[Authorize(Policy = CmsAdminAuthorization.PolicyName)]` (the policy named `Osirion.Cms.Admin`), the last two from
  the `_Imports.razor` of its folder. A host needs an authentication scheme and `UseAuthentication` and
  `UseAuthorization`; `AddOsirionCmsAdmin` registers a default policy of that name that requires an authenticated
  user unless the host already defined one.
- Renders `div.container-fluid.py-4` with a heading (`Edit: {title}`, `New Post`, or `Edit Content`), a subtitle, and a
  "Back to Content" button that goes to `/osirion/content`. The body is one of: a spinner (`spinner-border`) while
  loading, a "No Content Selected" card when there is no editing post, or the `ContentEditor` in a `card`.
- "Create New Post" starts a post titled "New Post" in the current content folder (`new-post.md` at the root), marks it
  as a new file, and publishes a `CreateNewContentEvent`.
- Saving (`OnSaveComplete`) navigates to `/osirion/content`. Discarding (`OnDiscard`) clears the editing state, then
  navigates to `/osirion/content`.
- Injected services: `IGitHubAdminService` (`GetBlogPostAsync`), `ILogger<EditContent>`, and from the admin base
  component `CmsState`, `NavigationManager`, `IEventPublisher`. These are admin services the host registers (the
  `CmsState`, `IEventPublisher` and `IEventSubscriber` registrations are in `AddOsirionCmsAdminDI`).
- A failed load is logged and the page shows the fixed message `UserMessages.LoadFailed` through the admin state, not
  the exception text.
- Subscribes to `CmsState.StateChanged` and unsubscribes in `Dispose`.
- No component named `EditContentPage` exists in the source; this page is the one at the route above, and it takes
  the file as the `Path` query value, not as `owner`, `repo` and `path` route parameters.
