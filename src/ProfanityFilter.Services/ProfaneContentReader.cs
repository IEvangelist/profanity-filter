// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

internal sealed class ProfaneContentReader
{
    private static readonly Lazy<Matcher> s_matcher = new(() =>
    {
        var matcher = new Matcher();
        matcher.AddInclude("Data/*.json");

        return matcher;
    });

    public static string[] GetFileNames() =>
        s_matcher.Value
            .GetResultsInFullPath(".")
            .ToArray();

    /// <summary>
    /// Reads the contents of the embedded resource as a <c>string</c>
    /// corresponding to the given <paramref name="resourceName"/>.
    /// </summary>
    /// <param name="resourceName">The name of the embedded resource to read.</param>
    /// <param name="cancellationToken">The token used to manage async cancellations.</param>
    /// <returns>Returns a <c>string</c> representation of the embedded resource.
    /// If there isn't a resource matching the given <paramref name="resourceName"/>,
    /// an empty <c>string</c> is returned.</returns>
    public static async ValueTask<ProfaneContent> ReadAsync(
        string resourceName, CancellationToken cancellationToken = default)
    {
        if (File.Exists(resourceName) is false)
        {
            return ProfaneContent.Empty;
        }

        using var resourceStream = File.OpenRead(resourceName);

        if (resourceStream is null)
        {
            return ProfaneContent.Empty;
        }

        var json = await new StreamReader(resourceStream)
            .ReadToEndAsync(cancellationToken);

        return json?.FromJson<ProfaneContent>()
            ?? ProfaneContent.Empty;
    }
}
