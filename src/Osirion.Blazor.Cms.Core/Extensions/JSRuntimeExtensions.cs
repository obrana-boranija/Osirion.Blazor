using Microsoft.JSInterop;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Extensions
{
    /// <summary>
    /// Extension methods for IJSRuntime
    /// </summary>
    public static class JSRuntimeExtensions
    {
        /// <summary>
        /// Extension method to serialize strings for JavaScript
        /// </summary>
        /// <param name="jsRuntime">JSRuntime instance</param>
        /// <param name="value">String to serialize</param>
        /// <returns>JSON serialized string</returns>
        public static string SerializeForJs(this IJSRuntime jsRuntime, string value)
        {
            return JsonSerializer.Serialize(value);
        }
    }
}