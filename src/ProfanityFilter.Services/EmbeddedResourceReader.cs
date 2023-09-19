// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

internal sealed class EmbeddedResourceReader
{
    private static readonly Lazy<Matcher> s_matcher = new(() =>
    {
        var matcher = new Matcher();

        return matcher;
    });

    public static string[] GetResourceNames() => []; // s_assembly.GetManifestResourceNames();

    /// <summary>
    /// Reads the contents of the embedded resource as a <c>string</c>
    /// corresponding to the given <paramref name="resourceName"/>.
    /// </summary>
    /// <param name="resourceName">The name of the embedded resource to read.</param>
    /// <param name="cancellationToken">The token used to manage async cancellations.</param>
    /// <returns>Returns a <c>string</c> representation of the embedded resource.
    /// If there isn't a resource matching the given <paramref name="resourceName"/>,
    /// an empty <c>string</c> is returned.</returns>
    public static async ValueTask<string> ReadAsync(
        string resourceName, CancellationToken cancellationToken = default)
    {
        if (File.Exists(resourceName) is false)
        {
            return "";
        }

        using var resourceStream = File.OpenRead(resourceName);

        if (resourceStream is null)
        {
            return "";
        }

        using var reader = new StreamReader(resourceStream);

        return await reader.ReadToEndAsync(cancellationToken);
    }
}
