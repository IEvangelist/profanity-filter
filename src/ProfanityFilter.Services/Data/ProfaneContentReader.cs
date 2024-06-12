// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

internal sealed class ProfaneContentReader
{
    private static readonly Lazy<GlobOptions> s_globOptions = new(() =>
    {
        EnsureWorkingDirectory();

        var builder = new GlobOptionsBuilder()
            .WithPattern("**/Data/*.txt");

        return builder.Build();
    });

    /// <summary>
    /// Gets an array of file names that match the profanity content file pattern.
    /// </summary>
    /// <returns>An array of file names.</returns>
    public static string[] GetFileNames() =>
        s_globOptions.Value
            .GetMatchingFileInfos()
            .Select(static file => file.FullName)
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

        return text ?? "";
    }

    private static void EnsureWorkingDirectory()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        // When running as a GitHub Action, we need to be in the /app dir.
        if (currentDirectory is "/github/workspace")
        {
            Directory.SetCurrentDirectory("/app");
        }
    }
}
