// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

internal static class MatchRegistry
{
    private static readonly ConcurrentDictionary<FilterParameters, List<string>> s_registry = new();

    /// <summary>
    /// Registers a given <paramref name="match"/> for the given <paramref name="parameters"/>.
    /// </summary>
    internal static void Register(FilterParameters parameters, Match match)
    {
        if (s_registry.TryGetValue(parameters, out var list))
        {
            list.Add(match.Value);
        }
        else
        {
            s_registry[parameters] = [match.Value];
        }
    }

    /// <summary>
    /// Removes the list of matches from the registry, and returns it for the given <paramref name="parameters"/>.
    /// </summary>
    internal static List<string>? PurgeMatches(FilterParameters parameters)
    {
        return s_registry.TryRemove(parameters, out var list) ? list : null;
    }
}
