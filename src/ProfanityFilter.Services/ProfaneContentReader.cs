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
    /// corresponding to the given <paramref name="fileName"/>.
    /// </summary>
    /// <param name="fileName">The name of the embedded resource to read.</param>
    /// <param name="cancellationToken">The token used to manage async cancellations.</param>
    /// <returns>Returns a <c>string</c> representation of the embedded resource.
    /// If there isn't a resource matching the given <paramref name="fileName"/>,
    /// an empty <c>string</c> is returned.</returns>
    public static async ValueTask<ProfaneContent> ReadAsync(
        string fileName, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"""
            Trying to read {fileName}.
            """);

        if (File.Exists(fileName) is false)
        {
            Console.WriteLine($"""
                The {fileName} doesn't exist.
                """);

            return ProfaneContent.Empty;
        }

        using var resourceStream = File.OpenRead(fileName);

        if (resourceStream is null)
        {
            Console.WriteLine($"""
                Unable to open {fileName} stream for reading.
                """);

            return ProfaneContent.Empty;
        }

        using var readStream = new StreamReader(resourceStream);

        var json = await readStream.ReadToEndAsync(cancellationToken);

        return json?.FromJson(ProfaneContentContext.Default.ProfaneContent)
            ?? ProfaneContent.Empty;
    }
}
