# ContentView

Render a single content item resolved from route or parameters. Handles not-found and loading states.

The item body is sanitized with the registered IHtmlContentSanitizer before it is rendered, so content from custom
providers or items passed in by the page cannot run script. Configure the allow-list with AddOsirionHtmlSanitizer.
