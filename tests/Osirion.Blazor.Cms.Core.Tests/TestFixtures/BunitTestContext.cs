using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Core.Tests.TestFixtures
{
    public class BunitTestContext : TestContext
    {
        public BunitTestContext()
        {
            // Register required services with mocked implementations
            JSInterop = Substitute.For<IJSRuntime>();
            Services.AddSingleton(JSInterop);

            MarkdownRenderer = Substitute.For<IMarkdownRendererService>();
            Services.AddSingleton(MarkdownRenderer);

            // Configure default behavior for the markdown renderer
            MarkdownRenderer.RenderToHtml(default).ReturnsForAnyArgs(callInfo => $"<p>{callInfo.Arg<string>()}</p>");
        }

        public IJSRuntime JSInterop { get; }
        public IMarkdownRendererService MarkdownRenderer { get; }

        /// <summary>
        /// Setup JSInterop for basic editor functionality
        /// </summary>
        public void SetupJSInteropForEditor()
        {
            // Mock import to return a JS reference
            var jsModule = Substitute.For<IJSObjectReference>();
            JSInterop.InvokeAsync<IJSObjectReference>("import", default)
                .ReturnsForAnyArgs(jsModule);

            // Mock eval calls that editor components make - specify CancellationToken explicitly
            JSInterop.InvokeAsync<object>("eval", CancellationToken.None, default)
                .ReturnsForAnyArgs(callInfo => new ValueTask<object>(Task.FromResult<object>(null!)));

            JSInterop.InvokeVoidAsync("eval", CancellationToken.None, default)
                .ReturnsForAnyArgs(callInfo => new ValueTask());
        }

        /// <summary>
        /// Setup JSInterop for scroll-related functionality
        /// </summary>
        public void SetupJSInteropForScrolling(double scrollPosition = 0.5)
        {
            // Use ReturnsForAnyArgs instead of Arg.Is to avoid issues with unbound arg matchers
            JSInterop.InvokeAsync<double>("eval", CancellationToken.None, default)
                .ReturnsForAnyArgs(callInfo => new ValueTask<double>(scrollPosition));
        }
    }
}