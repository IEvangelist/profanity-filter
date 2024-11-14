// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.WebApi.Endpoints;

internal static partial class ProfanityFilterEndpoints
{
    private const string FileExtensionMapKey = "file-extension-map";

    internal static async Task<IReadOnlyDictionary<string, string>> GetProfaneContentNamesAsync(
        this IMemoryCache cache,
        IReadOnlyCollection<string> filePaths)
    {
        var filePathToNameMap = await cache.GetOrCreateAsync(
            key: FileExtensionMapKey,
            factory: _ =>
            {
                var result = filePaths.ToDictionary(
                    static path => path,
                    static path => Path.GetFileNameWithoutExtension(path) ?? "")
                    ?? [];

                return Task.FromResult(result);
            });

        return filePathToNameMap ?? [];
    }
}