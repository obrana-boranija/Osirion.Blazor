# OsirionTabs

Purpose
Tabs that switch without script, for code samples (".NET CLI / Package Manager") and other alternative content. Each
`OsirionTab` is a radio button, its label and a panel, so tabs work under static server-side rendering.

Key parameters
- OsirionTabs: `SelectedIndex` (default 0), `Id` (radio group name and id prefix; generated when empty), `AriaLabel`,
  `Class`
- OsirionTab: `Title` (required), `ChildContent`, `Class` (on the panel)

Usage
```razor
<OsirionTabs AriaLabel="Install command">
    <OsirionTab Title=".NET CLI"><pre><code>dotnet add package Osirion.Blazor</code></pre></OsirionTab>
    <OsirionTab Title="Package Manager"><pre><code>Install-Package Osirion.Blazor</code></pre></OsirionTab>
</OsirionTabs>
```

Notes
- Keyboard: Tab reaches the group, the arrow keys move between tabs (native radio behaviour); the focused tab shows a
  focus ring. Screen readers announce each tab as a radio button with its position ("2 of 3").
- Ids: `{Id}-{index}` for the radio, `-label` and `-panel` suffixes for the label and panel; the panel has
  `role="region"` and `aria-labelledby` its label.
- Printing shows every panel, because a printed page cannot switch tabs.
- An `OsirionTab` outside `OsirionTabs` throws `InvalidOperationException`.
