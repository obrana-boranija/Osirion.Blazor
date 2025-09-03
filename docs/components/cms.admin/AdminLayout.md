
# AdminLayout

Purpose
Main layout used by the admin application. Provides header, navigation and content regions.

Parameters
- `HeaderContent`: RenderFragment - Header content
- `NavigationContent`: RenderFragment - Navigation content
- `FooterContent`: RenderFragment - Footer content

Example

```razor
<AdminLayout>
	<HeaderContent>
		<h1>Admin</h1>
	</HeaderContent>

	<NavigationContent>
		<DefaultNavigation MenuItems="@menuItems" />
	</NavigationContent>

	<p>Main content</p>
</AdminLayout>
```
