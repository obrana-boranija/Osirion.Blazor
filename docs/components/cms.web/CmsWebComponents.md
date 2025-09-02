# CMS Web Components

Overview
Reusable components for content-driven sites powered by Osirion CMS providers.

Core
- ContentList: query and render collections; supports category/tag/locale filters, sorting, pagination
- ContentView: render a single content item (by route/context)
- ContentPage: layout wrapper for content items
- LocalizedContentView: locale-aware rendering
- ContentRenderer: resolve and render content by URL
- SeoMetadataRenderer: emits meta tags from content

Navigation
- ContentBreadcrumbs, OsirionContentNavigation
- CategoriesList, DirectoryNavigation, TagCloud, TableOfContents, SearchBox

Page-level examples
- ArticlePage, DocumentPage, LandingPage, HomePage (ready-made templates)

Sections
- ContentSection, OsirionContentListSection, FeaturedPostsSection

Notes
- Register services via OsirionContentServiceCollectionExtensions
- Provider contracts live in Osirion.Blazor.Cms.Domain
