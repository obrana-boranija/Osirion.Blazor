---
title: "Changelog"
author: "Dejan Demonjić"
date: 2025-04-23
---
# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [3.0.1] - 2026-09-24

### Added
- Mega menu building blocks in Osirion.Blazor.Navigation:
  - `MenuPanel` + `MenuPanelLink`: high-contrast promotional aside with pill links ("Platform highlights", "Build your business case"); brandable via `--osirion-menu-panel-background` / `--osirion-menu-panel-color`
  - `MenuImageCard`: linked image card with overlay caption for mega menu asides
  - `MenuItem.SubmenuAside` render fragment: dedicated right-hand aside column for mega submenus
  - `MenuGroup.Description`: column subtitle with divider (mega menu header style)
  - `MenuGroup.FooterText`/`FooterHref`/`FooterTarget`: "Explore X →" footer links per group
- bUnit coverage for all new mega menu components and parameters

### Changed
- Mega submenus now center under the whole navigation bar on desktop, use a responsive auto-fit column grid with column headers, and keep an invisible hover bridge so the pointer can travel from the trigger into the panel
- Mega submenu asides and group descriptions are hidden in the collapsed drawer and mobile views to keep the drawer scannable
- Collapsed drawer submenu toggle no longer paints an opaque hover background over the row label

### Fixed
- `MenuGroup` without an `Id` generated a new identifier on every read, so the items container's `aria-labelledby` pointed at an element that did not exist and the label and items ids changed on every render; the generated identifier is now created once per instance
- Navigation tests updated to the current `Menu`, `MenuItem` and `MenuGroup` markup, and `NavigationService` log assertions no longer try to match the `LogInformation` extension method with NSubstitute

## [1.5.0] - 2025-04-21

### Added
- Improved service registration with fluent API through `AddOsirionBlazor` method
- New `IOsirionBlazorBuilder` interface for better extensibility
- `OsirionStyleOptions` class separated from `GitHubCmsOptions` for better separation of concerns
- CSS framework integration support for Bootstrap, FluentUI, MudBlazor, and Radzen
- CssFramework enum for specifying framework integration options
- Integration classes for mapping Osirion variables to CSS framework variables
- Updated OsirionStyles component with FrameworkIntegration parameter
- Framework integration documentation with examples for each supported framework
- Comprehensive unit tests for framework integration

### Changed
- Refactored service collection extensions for better organization
- Renamed `osirion-cms.css` to `osirion.css` for better clarity on its broader purpose
- Enhanced the styling documentation to include framework integration examples
- Updated the README.md and Quick Reference with CSS framework integration examples and fluent API usage
- Styling options moved from `GitHubCmsOptions` to dedicated `OsirionStyleOptions` class

## [1.4.0] - 2025-04-20

### Added
- ScrollToTop component for easy navigation back to the top of the page
- ButtonPosition enum for positioning UI elements
- ScrollToTopOptions for DI-based configuration
- Extension methods for registering ScrollToTop configuration
- Comprehensive unit tests for ScrollToTop component
- Updated documentation for navigation components
- CSS variables for styling the ScrollToTop button

### Changed
- Enhanced the navigation documentation to include ScrollToTop
- Updated the README.md and Quick Reference with ScrollToTop examples
- Improved the usability of the navigation components

## [1.3.0] - 2025-04-20

### Added
- GitHub CMS service and components for markdown-based content management
- Support for frontmatter parsing in markdown files
- Content caching with configurable duration
- Category and tag management components
- Full-text search functionality
- Directory-based navigation
- Default styling with customizable CSS variables
- OsirionStyles component for theme customization
- Comprehensive unit tests for GitHub CMS
- Detailed documentation and examples

### Changed
- Updated package metadata to include CMS capabilities
- Enhanced documentation with GitHub CMS examples
- Added Markdig dependency for markdown processing

## [1.2.0] - 2025-04-20

### Added
- ClarityTracker component for Microsoft Clarity analytics integration
- MatomoTracker component for Matomo analytics integration
- TrackerBaseOptions abstract class for common analytics properties
- AnalyticsServiceCollectionExtensions for easy dependency injection configuration
- Comprehensive unit tests for all analytics components
- Detailed documentation for analytics components
- Quick reference guide for component usage
- Migration guide for upgrading

### Changed
- Updated package description to include analytics components
- Added analytics-related tags to package metadata
- Improved README with proper usage examples for all components

## [1.1.0] - 2025-01-18

### Added
- EnhancedNavigationInterceptor component for automatic scroll behavior
- SSR compatibility for all components
- Support for .NET 8, .NET 9, and future versions
- Navigation documentation
- Enhanced examples and usage patterns

### Changed
- Improved package metadata and documentation
- Added navigation-related tags to package metadata

## [1.0.0] - 2025-01-15

### Added
- Core package structure
- Basic documentation and licensing
- Service collection extensions
- Initial project setup
