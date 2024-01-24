// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

internal sealed class ProfaneContentReader
{
    private static readonly Assembly s_assembly = Assembly.GetCallingAssembly();

    /// <summary>
    /// Gets an array of file names that match the profanity content file pattern.
    /// </summary>
    /// <returns>An array of file names.</returns>
    public static string[] GetFileNames() => s_assembly.GetManifestResourceNames();

    /// <summary>
    /// Reads the contents of the embedded resource as a <c>string</c>
    /// corresponding to the given <paramref name="fileName"/>.
    /// </summary>
    /// <param name="fileName">The name of the embedded resource to read.</param>
    /// <param name="cancellationToken">The token used to manage async cancellations.</param>
    /// <returns>Returns a <c>string</c> representation of the embedded resource.
    /// If there isn't a resource matching the given <paramref name="fileName"/>,
    /// an empty <c>string</c> is returned.</returns>
    public static async ValueTask<string> ReadAsync(
        string fileName, CancellationToken cancellationToken = default)
    {
        return await ReadResourceAsync(fileName, cancellationToken);
    }

    private static async Task<string> ReadResourceAsync(
        string name, CancellationToken cancellationToken)
    {
        if (name.EndsWith(".txt") is false)
        {
            Console.WriteLine($"""
                Unable to read: '{name}'.
                The file name must end with '.txt', no attempt was made to read it.
                """);

            return "";
        }

        await using var stream = s_assembly.GetManifestResourceStream(name);

        if (stream is null)
        {
            Console.WriteLine($"""
                Unable to open {name} stream for reading.
                """);

            return "";
        }

        using StreamReader reader = new(stream);

        var text = await reader.ReadToEndAsync(cancellationToken)
            .ConfigureAwait(false);

        if (text is null or { Length: 0 })
        {
            Console.WriteLine($"""
                Unable to read the file contents, either null or empty.
                  {name}
                """);
        }
        else
        {
            Console.WriteLine($"""
                File contents: {name}
                {text}
                """);
        }

        return text ?? "";
    }
}
