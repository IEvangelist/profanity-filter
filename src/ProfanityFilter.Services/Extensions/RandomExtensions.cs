// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Extensions;

internal static class RandomExtensions
{
    /// <summary>
    /// Gets a random subset of items from the source array, with the
    /// specified length, while respecting the limits.
    /// </summary>
    internal static TItem[] RandomItemsWithLimitToOne<TItem>(
        this TItem[] array,
        int length,
        TItem limit) where TItem : notnull
    {
        var choices = array.Except([limit]).ToArray();

        var items = Random.Shared.GetItems(choices, length);

        var index = Random.Shared.Next(0, items.Length);

        items[index] = limit;

        return items;
    }
}
