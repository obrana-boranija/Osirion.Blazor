# ContentList

Purpose
List and paginate content items from the registered provider with filters.

Key parameters
- Directory, Category, Tag
- OnlyFeatured, FeaturedCount
- Locale, DirectoryId
- SortBy (Date default), SortDirection (Descending default)
- ShowPagination (bool), ItemsPerPage (10), CurrentPage (1)
- ReadMoreText

Delegates
- ContentUrlFormatter(ContentItem): build item URL
- PaginationUrlFormatter(int): build page URL
- PageChanged(int)
- OnItemSelected(ContentItem)

States
- LoadingText, NoContentText

Notes
- Uses IContentProviderManager; query composed from parameters
- When pagination enabled, component performs a count pass, then a paged fetch.
