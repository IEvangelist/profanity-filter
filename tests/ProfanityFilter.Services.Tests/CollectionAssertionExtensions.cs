// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Tests;

internal static class CollectionAssertionExtensions
{
    public static void Contains<T>(this CollectionAssert _, IEnumerable<T> collection, Func<T, bool> predicate)
    {
        foreach (var item in collection)
        {
            if (predicate(item))
            {
                return;
            }
        }


        throw new Exception("Expected collection to contain item, but it did not.");
    }

    public static void Single<T>(this CollectionAssert _, IEnumerable<T> collection)
    {
        var count = collection.Count();
        if (count != 1)
        {
            throw new Exception($"Expected collection to contain single item, but it contained {count} items.");
        }
    }
}