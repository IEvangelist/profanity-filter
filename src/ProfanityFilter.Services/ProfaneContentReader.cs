// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

internal sealed class ProfaneContentReader
{
    private static readonly Lazy<Matcher> s_matcher = new(() =>
    {
        var matcher = new Matcher();
        matcher.AddInclude("Data/*.txt");
        return matcher;
    });

    /// <summary>
    /// Gets an array of file names that match the profanity content file pattern.
    /// </summary>
    /// <returns>An array of file names.</returns>
    public static string[] GetFileNames() =>
        s_matcher.Value
            .GetResultsInFullPath(".")
            .ToArray();

    /// <summary>
    /// Reads the contents of the embedded resource as a <c>string</c>
    /// corresponding to the given <paramref fileName="fileName"/>.
    /// </summary>
    /// <param fileName="fileName">The fileName of the embedded resource to read.</param>
    /// <param fileName="cancellationToken">The token used to manage async cancellations.</param>
    /// <returns>Returns a <c>string</c> representation of the embedded resource.
    /// If there isn't a resource matching the given <paramref fileName="fileName"/>,
    /// an empty <c>string</c> is returned.</returns>
    public static async ValueTask<string> ReadAsync(
        string fileName, CancellationToken cancellationToken = default)
    {
        return await ReadFileAsync(fileName, cancellationToken);
    }

    private static async ValueTask<string> ReadFileAsync(
        string fileName, CancellationToken cancellationToken)
    {
        if (fileName.EndsWith(".txt") is false)
        {
            Console.WriteLine($"""
                Unable to read: '{fileName}'.
                The file name must end with '.txt', no attempt was made to read it.
                """);

            return "";
        }

        await using var stream = File.OpenRead(fileName);

        if (stream is null)
        {
            Console.WriteLine($"""
                Unable to open {fileName} stream for reading.
                """);

            return "";
        }

        using var reader = new StreamReader(stream);

        var text = await reader.ReadToEndAsync(cancellationToken)
            .ConfigureAwait(false);

        if (text is null or { Length: 0 })
        {
            Console.WriteLine($"""
                Unable to read the file contents, either null or empty.
                  {fileName}
                """);
        }
        else
        {
            Console.WriteLine($"""
                File contents: {fileName}
                {text}
                """);
        }

        return text ?? "";
    }
}
