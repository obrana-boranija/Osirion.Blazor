# Performance Guide

[![Documentation](https://img.shields.io/badge/Documentation-Performance-orange)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/docs/PERFORMANCE.md)
[![Version](https://img.shields.io/nuget/v/Osirion.Blazor)](https://www.nuget.org/packages/Osirion.Blazor)

This guide provides comprehensive information about optimizing performance when using Osirion.Blazor in your applications.

## Table of Contents

1. [Performance Principles](#performance-principles)
2. [Server-Side Rendering Optimization](#server-side-rendering-optimization)
3. [Content Management Performance](#content-management-performance)
4. [Component Optimization](#component-optimization)
5. [Caching Strategies](#caching-strategies)
6. [Memory Management](#memory-management)
7. [Network Optimization](#network-optimization)
8. [Monitoring and Diagnostics](#monitoring-and-diagnostics)
9. [Best Practices](#best-practices)

## Performance Principles

### SSR-First Design
Osirion.Blazor is built with Server-Side Rendering as the primary target, ensuring:
- **Fast initial page loads**
- **SEO-friendly content**
- **Reduced JavaScript payload**
- **Better Core Web Vitals scores**

### Progressive Enhancement
Components work without JavaScript and enhance functionality when available:
- **Core functionality works server-side**
- **JavaScript adds interactivity**
- **Graceful degradation**
- **Improved reliability**

### Efficient Resource Usage
- **Minimal memory footprint**
- **Optimized rendering paths**
- **Lazy loading strategies**
- **Smart caching mechanisms**

## Server-Side Rendering Optimization

### Rendering Performance

#### Component Hierarchy Optimization
```csharp
// ? Good: Flat component structure
<HeroSection Title="@title" />
<ContentList Directory="blog" Count="5" />

// ? Avoid: Deep nesting
<div>
    <div>
        <div>
            <HeroSection Title="@title" />
        </div>
    </div>
</div>
```

#### Conditional Rendering
```razor
@* ? Good: Early returns for null checks *@
@if (content == null)
{
    <OsirionPageLoading />
    return;
}

<ContentView Path="@content.Path" />

@* ? Avoid: Deep conditional nesting *@
@if (content != null)
{
    @if (content.IsPublished)
    {
        @if (!string.IsNullOrEmpty(content.Title))
        {
            <ContentView Path="@content.Path" />
        }
    }
}
```

#### Async Component Loading
```csharp
public partial class BlogPage : ComponentBase
{
    [Inject] private IContentProvider ContentProvider { get; set; } = default!;
    
    private List<ContentItem> articles = new();
    private bool isLoading = true;
    
    protected override async Task OnInitializedAsync()
    {
        // ? Load content asynchronously
        var query = new ContentQuery
        {
            Directory = "blog",
            Count = 10,
            SortBy = SortField.Date,
            SortDirection = SortDirection.Descending
        };
        
        articles = (await ContentProvider.GetItemsByQueryAsync(query)).ToList();
        isLoading = false;
        StateHasChanged();
    }
}
```

### Pre-rendering Optimization

#### Static Site Generation
```csharp
// Configure for optimal SSG performance
builder.Services.AddOsirionBlazor(osirion => {
    osirion
        .AddGitHubCms(options => {
            options.Owner = "username";
            options.Repository = "content";
            options.CacheExpirationMinutes = 1440; // 24 hours for SSG
            options.EnablePrecompilation = true;
        })
        .AddOsirionStyle(CssFramework.Bootstrap);
});
```

#### Route Pre-generation
```csharp
// Generate static routes for content
public class ContentRouteProvider : IStaticRouteProvider
{
    public async Task<IEnumerable<string>> GetRoutesAsync()
    {
        var contentProvider = // Get content provider
        var articles = await contentProvider.GetItemsByQueryAsync(new ContentQuery());
        
        return articles.Select(a => $"/blog/{a.Slug}");
    }
}
```

## Content Management Performance

### GitHub API Optimization

#### Efficient API Usage
```csharp
public class OptimizedGitHubOptions : GitHubOptions
{
    public OptimizedGitHubOptions()
    {
        // ? Optimize for performance
        CacheExpirationMinutes = 60;           // Cache for 1 hour
        UseConditionalRequests = true;         // Use ETags
        MaxConcurrentRequests = 5;            // Limit concurrent requests
        RequestTimeout = TimeSpan.FromSeconds(30);
        EnableCompression = true;
    }
}
```

#### Batch API Calls
```csharp
public class BatchContentLoader
{
    public async Task<IEnumerable<ContentItem>> LoadContentBatchAsync(
        IEnumerable<string> paths)
    {
        // ? Load multiple items in parallel with limits
        var semaphore = new SemaphoreSlim(5, 5); // Max 5 concurrent
        var tasks = paths.Select(async path =>
        {
            await semaphore.WaitAsync();
            try
            {
                return await contentProvider.GetItemAsync(path);
            }
            finally
            {
                semaphore.Release();
            }
        });
        
        var results = await Task.WhenAll(tasks);
        return results.Where(r => r != null);
    }
}
```

### Content Caching

#### Multi-Level Caching Strategy
```csharp
services.Configure<GitHubCmsOptions>(options =>
{
    // ? Implement multi-level caching
    options.EnableMemoryCache = true;
    options.MemoryCacheSize = 100; // MB
    options.EnableDistributedCache = true;
    options.CacheExpirationMinutes = 60;
    options.SlidingExpiration = TimeSpan.FromMinutes(15);
});

// Add distributed cache
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});
```

#### Cache Warming
```csharp
public class CacheWarmupService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // ? Warm up cache with frequently accessed content
        var frequentPaths = new[]
        {
            "blog/latest-post.md",
            "pages/about.md",
            "blog/popular-article.md"
        };
        
        foreach (var path in frequentPaths)
        {
            await contentProvider.GetItemAsync(path);
            await Task.Delay(100, stoppingToken); // Rate limiting
        }
    }
}
```

## Component Optimization

### Rendering Performance

#### Memo Components
```csharp
// ? Use memo for expensive computations
public partial class ExpensiveComponent : ComponentBase
{
    [Parameter] public List<ContentItem> Items { get; set; } = new();
    
    private List<ContentItem> processedItems = new();
    private int lastItemsHash;
    
    protected override void OnParametersSet()
    {
        var currentHash = Items.GetHashCode();
        if (currentHash != lastItemsHash)
        {
            processedItems = ProcessItems(Items);
            lastItemsHash = currentHash;
        }
    }
    
    private List<ContentItem> ProcessItems(List<ContentItem> items)
    {
        // Expensive processing here
        return items.Select(ProcessItem).ToList();
    }
}
```

#### Virtual Scrolling
```razor
@* ? Use virtual scrolling for large lists *@
<div class="virtual-list" style="height: 400px; overflow-y: auto;">
    @for (int i = startIndex; i < endIndex && i < Items.Count; i++)
    {
        var item = Items[i];
        <div class="list-item" data-index="@i">
            <ContentCard Item="@item" />
        </div>
    }
</div>

@code {
    private int startIndex = 0;
    private int endIndex = 10;
    private const int ItemHeight = 100;
    private const int VisibleItems = 10;
    
    private void OnScroll(ScrollEventArgs e)
    {
        startIndex = (int)(e.ScrollTop / ItemHeight);
        endIndex = startIndex + VisibleItems;
        StateHasChanged();
    }
}
```

### Component Lifecycle Optimization

#### Efficient State Management
```csharp
public partial class OptimizedComponent : ComponentBase, IDisposable
{
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private Timer? refreshTimer;
    
    protected override async Task OnInitializedAsync()
    {
        // ? Use cancellation tokens
        await LoadDataAsync(cancellationTokenSource.Token);
        
        // ? Set up efficient refresh
        refreshTimer = new Timer(async _ => await RefreshDataAsync(), 
            null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }
    
    public void Dispose()
    {
        // ? Clean up resources
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        refreshTimer?.Dispose();
    }
}
```

## Caching Strategies

### HTTP Response Caching

#### Static Content Caching
```csharp
// Configure response caching
app.UseResponseCaching();

// Cache static content aggressively
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/blog") && 
        context.Request.Method == "GET")
    {
        context.Response.Headers.CacheControl = "public, max-age=3600";
        context.Response.Headers.ETag = GenerateETag(context.Request.Path);
    }
    
    await next();
});
```

#### Dynamic Content Caching
```csharp
[ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "category", "tag" })]
public class BlogController : Controller
{
    public async Task<IActionResult> Index(string? category = null)
    {
        // Cache varies by category parameter
        var content = await contentService.GetBlogPostsAsync(category);
        return View(content);
    }
}
```

### In-Memory Caching

#### Content Item Caching
```csharp
public class CachedContentProvider : IContentProvider
{
    private readonly IMemoryCache cache;
    private readonly IContentProvider baseProvider;
    
    public async Task<ContentItem?> GetItemAsync(string path)
    {
        return await cache.GetOrCreateAsync($"content:{path}", async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(15));
            entry.SetAbsoluteExpiration(TimeSpan.FromHours(1));
            
            return await baseProvider.GetItemAsync(path);
        });
    }
}
```

#### Query Result Caching
```csharp
public async Task<IEnumerable<ContentItem>> GetItemsByQueryAsync(ContentQuery query)
{
    var cacheKey = $"query:{query.GetHashCode()}";
    
    return await cache.GetOrCreateAsync(cacheKey, async entry =>
    {
        entry.SetSlidingExpiration(TimeSpan.FromMinutes(5));
        entry.SetPriority(CacheItemPriority.High);
        
        return await baseProvider.GetItemsByQueryAsync(query);
    });
}
```

### CDN Integration

#### Asset Optimization
```csharp
// Configure CDN for static assets
services.Configure<StaticFileOptions>(options =>
{
    options.OnPrepareResponse = ctx =>
    {
        // ? Set aggressive caching for assets
        ctx.Context.Response.Headers.CacheControl = 
            "public, max-age=31536000"; // 1 year
        
        if (ctx.File.Name.EndsWith(".woff2") || ctx.File.Name.EndsWith(".woff"))
        {
            ctx.Context.Response.Headers.CacheControl = 
                "public, max-age=31536000, immutable";
        }
    };
});
```

## Memory Management

### Object Lifecycle Management

#### Dispose Pattern Implementation
```csharp
public class ContentService : IContentService, IDisposable
{
    private readonly HttpClient httpClient;
    private readonly Timer refreshTimer;
    private bool disposed = false;
    
    public ContentService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        this.refreshTimer = new Timer(RefreshCache, null, 
            TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                httpClient?.Dispose();
                refreshTimer?.Dispose();
            }
            disposed = true;
        }
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
```

#### Weak References for Caching
```csharp
public class WeakReferenceCache<TKey, TValue> where TValue : class
{
    private readonly ConcurrentDictionary<TKey, WeakReference<TValue>> cache = new();
    
    public bool TryGet(TKey key, out TValue? value)
    {
        value = null;
        
        if (cache.TryGetValue(key, out var weakRef))
        {
            if (weakRef.TryGetTarget(out value))
            {
                return true;
            }
            else
            {
                // Remove dead reference
                cache.TryRemove(key, out _);
            }
        }
        
        return false;
    }
    
    public void Set(TKey key, TValue value)
    {
        cache[key] = new WeakReference<TValue>(value);
    }
}
```

### Memory Monitoring

#### Memory Usage Tracking
```csharp
public class MemoryMonitoringService : BackgroundService
{
    private readonly ILogger<MemoryMonitoringService> logger;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var gcMemory = GC.GetTotalMemory(false);
            var workingSet = Process.GetCurrentProcess().WorkingSet64;
            
            logger.LogInformation(
                "Memory usage - GC: {GCMemory:N0} bytes, Working Set: {WorkingSet:N0} bytes",
                gcMemory, workingSet);
            
            // Force GC if memory usage is high
            if (gcMemory > 500_000_000) // 500MB
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
```

## Network Optimization

### HTTP Client Optimization

#### Connection Pooling
```csharp
services.AddHttpClient<IGitHubApiClient, GitHubApiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("Osirion.Blazor/1.0");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    MaxConnectionsPerServer = 10,
    PooledConnectionLifetime = TimeSpan.FromMinutes(15)
});
```

#### Request Compression
```csharp
services.AddHttpClient<IGitHubApiClient, GitHubApiClient>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    });
```

### Image Optimization

#### Responsive Images
```razor
@* ? Use responsive images *@
<picture>
    <source media="(min-width: 768px)" srcset="@GetImageUrl(item.FeaturedImage, 800)">
    <source media="(min-width: 480px)" srcset="@GetImageUrl(item.FeaturedImage, 600)">
    <img src="@GetImageUrl(item.FeaturedImage, 400)" 
         alt="@item.Title" 
         loading="lazy"
         decoding="async" />
</picture>

@code {
    private string GetImageUrl(string? imageUrl, int width)
    {
        if (string.IsNullOrEmpty(imageUrl)) return "";
        
        // Use image processing service for optimization
        return $"{imageUrl}?w={width}&q=80&f=webp";
    }
}
```

#### Image Lazy Loading
```razor
@* ? Implement intersection observer for lazy loading *@
<img src="@GetPlaceholderImage()" 
     data-src="@item.FeaturedImage"
     alt="@item.Title"
     class="lazy-image"
     loading="lazy" />

<script>
// Intersection Observer for lazy loading
const imageObserver = new IntersectionObserver((entries, observer) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            const img = entry.target;
            img.src = img.dataset.src;
            img.classList.remove('lazy-image');
            observer.unobserve(img);
        }
    });
});

document.querySelectorAll('.lazy-image').forEach(img => {
    imageObserver.observe(img);
});
</script>
```

## Monitoring and Diagnostics

### Performance Metrics

#### Core Web Vitals Tracking
```csharp
public class WebVitalsTracker
{
    private readonly IAnalyticsService analyticsService;
    
    public async Task TrackCoreWebVitals(
        double lcp, // Largest Contentful Paint
        double fid, // First Input Delay
        double cls) // Cumulative Layout Shift
    {
        await analyticsService.TrackEventAsync("Performance", "CoreWebVitals", null, new
        {
            LCP = lcp,
            FID = fid,
            CLS = cls
        });
    }
}
```

#### Application Performance Monitoring
```csharp
public class PerformanceMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<PerformanceMiddleware> logger;
    
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            if (stopwatch.ElapsedMilliseconds > 1000) // Log slow requests
            {
                logger.LogWarning(
                    "Slow request: {Method} {Path} took {ElapsedMs}ms",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
```

### Diagnostic Tools

#### Memory Profiling
```csharp
#if DEBUG
public class MemoryProfiler
{
    public static void ProfileComponent<T>() where T : ComponentBase
    {
        var before = GC.GetTotalMemory(true);
        
        // Create and render component
        using var testContext = new TestContext();
        var component = testContext.RenderComponent<T>();
        
        var after = GC.GetTotalMemory(true);
        var allocated = after - before;
        
        Console.WriteLine($"Component {typeof(T).Name} allocated {allocated:N0} bytes");
    }
}
#endif
```

#### Performance Profiling
```csharp
public static class ProfilerExtensions
{
    public static async Task<T> ProfileAsync<T>(
        this Task<T> task, 
        string operationName,
        ILogger logger)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await task;
            stopwatch.Stop();
            
            logger.LogInformation(
                "Operation {OperationName} completed in {ElapsedMs}ms",
                operationName, stopwatch.ElapsedMilliseconds);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex,
                "Operation {OperationName} failed after {ElapsedMs}ms",
                operationName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
```

## Best Practices

### Development Best Practices

1. **Profile Early and Often**
   - Use browser dev tools to identify bottlenecks
   - Monitor memory usage during development
   - Test with realistic data volumes

2. **Optimize Critical Rendering Path**
   - Minimize initial JavaScript payload
   - Use CSS-in-JS sparingly
   - Prioritize above-the-fold content

3. **Implement Progressive Loading**
   - Load critical content first
   - Use skeleton screens for loading states
   - Implement pagination for large datasets

### Production Best Practices

1. **Monitor Performance Continuously**
   - Set up alerts for performance degradation
   - Track Core Web Vitals
   - Monitor server response times

2. **Optimize for Your Use Case**
   - Profile with production data
   - Optimize based on user behavior
   - A/B test performance improvements

3. **Plan for Scale**
   - Design with horizontal scaling in mind
   - Use appropriate caching strategies
   - Monitor resource usage patterns

### Performance Testing

#### Load Testing
```csharp
[Fact]
public async Task ContentProvider_ShouldHandleHighLoad()
{
    var tasks = Enumerable.Range(0, 100)
        .Select(async i => await contentProvider.GetItemAsync($"test-{i}.md"));
    
    var stopwatch = Stopwatch.StartNew();
    var results = await Task.WhenAll(tasks);
    stopwatch.Stop();
    
    Assert.True(stopwatch.ElapsedMilliseconds < 5000); // Should complete in 5 seconds
    Assert.Equal(100, results.Length);
}
```

#### Memory Leak Testing
```csharp
[Fact]
public void Component_ShouldNotLeakMemory()
{
    var initialMemory = GC.GetTotalMemory(true);
    
    // Create and dispose many components
    for (int i = 0; i < 1000; i++)
    {
        using var context = new TestContext();
        var component = context.RenderComponent<TestComponent>();
        component.Dispose();
    }
    
    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();
    
    var finalMemory = GC.GetTotalMemory(true);
    var memoryIncrease = finalMemory - initialMemory;
    
    Assert.True(memoryIncrease < 1_000_000, // Less than 1MB increase
        $"Memory leak detected: {memoryIncrease:N0} bytes allocated");
}
```

By following these performance optimization guidelines, you can ensure that your Osirion.Blazor applications deliver excellent performance and user experience across all deployment scenarios.