---
title: "Getting Started with Osirion.Blazor"
author: "Dejan Demonjić"
date: "2025-04-19"
description: "A comprehensive guide to getting started with Osirion.Blazor components and tools."
tags: [Blazor, .NET, Web Development, Tutorial]
categories: [Tutorials, Blazor]
slug: "getting-started-with-osirion-blazor"
is_featured: true
featured_image: "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80"
---

# Getting Started with Osirion.Blazor

Osirion.Blazor is a powerful library of components and utilities designed to enhance your Blazor development experience. In this tutorial, we'll explore how to set up and use Osirion.Blazor in your projects.

## Prerequisites

Before we begin, ensure you have the following:

- .NET 8.0 SDK or newer
- Visual Studio 2022 or VS Code
- Basic knowledge of Blazor

## Installation

To install Osirion.Blazor, add the package to your project using NuGet:

```bash
dotnet add package Osirion.Blazor
```

Alternatively, you can use the NuGet Package Manager in Visual Studio.

## Basic Setup

After installing the package, update your `Program.cs` file to register Osirion.Blazor services:

```csharp
using Osirion.Blazor.Extensions;

// Add Osirion.Blazor services
builder.Services.AddOsirionBlazor();

// Add GitHub CMS if needed
builder.Services.AddGitHubCms(options =>
{
    options.Owner = "your-github-username";
    options.Repository = "your-content-repo";
    options.Branch = "main";
});
```

## Using Navigation Components

Osirion.Blazor provides enhanced navigation components for Blazor. Add the following to your `_Imports.razor`:

```razor
@using Osirion.Blazor.Components.Navigation
```

Then, in your `App.razor` or layout file:

```razor
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
```

This will ensure smooth scrolling behavior during navigation.

## Analytics Integration

To add analytics tracking:

```razor
@using Osirion.Blazor.Components.Analytics
@using Osirion.Blazor.Components.Analytics.Options

<ClarityTracker Options="new ClarityOptions { 
    TrackerUrl = 'https://www.clarity.ms/tag/', 
    SiteId = 'your-clarity-id',
    Track = true 
}" />

<MatomoTracker Options="new MatomoOptions {
    TrackerUrl = '//analytics.example.com/',
    SiteId = '1',
    Track = true
}" />
```

## Using GitHub CMS

To implement a content management system using GitHub:

```razor
@using Osirion.Blazor.Components.GitHubCms

<!-- Display a list of posts -->
<ContentList Directory="blog" />

<!-- Display posts by category -->
<ContentList Category="tutorials" />

<!-- Display posts by tag -->
<ContentList Tag="blazor" />

<!-- Show featured posts -->
<ContentList FeaturedCount="3" />

<!-- View a single post -->
<ContentView Path="blog/my-post.md" />

<!-- Display categories and tags -->
<CategoriesList />
<TagCloud MaxTags="15" />

<!-- Add search functionality -->
<SearchBox Placeholder="Search blog..." />
```

## Conclusion

Osirion.Blazor provides a rich set of components and utilities to enhance your Blazor applications. By following this guide, you've learned how to set up and use some of the key features.

For more information, check out the [GitHub repository](https://github.com/obrana-boranija/Osirion.Blazor) and the detailed documentation.