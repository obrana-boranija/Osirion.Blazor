using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Osirion.Blazor.Cms.Core.Extensions;

namespace Osirion.Blazor.Cms.Core.Components.Editor;

/// <summary>
/// Initializes the Markdown Editor JavaScript module
/// </summary>
public class MarkdownEditorInitializer : IComponent, IAsyncDisposable
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private ILogger<MarkdownEditorInitializer> Logger { get; set; } = default!;

    /// <inheritdoc/>
    public void Attach(RenderHandle renderHandle)
    {
        // This is a non-rendering component, so we don't need to do anything with the render handle
    }

    /// <inheritdoc/>
    public async Task SetParametersAsync(ParameterView parameters)
    {
        // Initialize JavaScript module when the component is first created
        try
        {
            if (OperatingSystem.IsBrowser())
            {
                await JSRuntime.RegisterMarkdownEditorModuleAsync();
                Logger.LogInformation("Markdown Editor JavaScript module registered successfully");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to initialize Markdown Editor JavaScript module");
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        // No resources to dispose
        await Task.CompletedTask;
    }
}