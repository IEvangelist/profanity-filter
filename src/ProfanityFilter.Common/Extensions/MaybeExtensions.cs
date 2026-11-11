// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Common.Extensions;

/// <summary>
/// Provides extension methods for the Maybe type.
/// </summary>
public static class MaybeExtensions
{
    /// <summary>
    /// Converts a value to an instance of <see cref="Maybe{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <returns>A union containing <see cref="Available{T}"/> if the value is not null; otherwise, <see cref="Absent{T}"/>.</returns>
    public static Maybe<T> AsMaybe<T>(this T? value) => value is null
        ? new Absent<T>()
        : new Available<T>(value);

    /// <summary>
    /// Chains the current instance of <typeparamref name="TIn"/> to a new instance of <typeparamref name="TOut"/> using the provided function.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="this">The current instance of <typeparamref name="TIn"/>.</param>
    /// <param name="func">The function to transform the input value to the output value.</param>
    /// <returns>A union containing the transformed available case, or the absent case.</returns>
    public static Maybe<TOut> Chain<TIn, TOut>(
        this Maybe<TIn> @this,
        Func<TIn, TOut> func) =>
        @this switch
        {
            Available<TIn> s => new Available<TOut>(func.Invoke(s.Value)),
            Absent<TIn> => new Absent<TOut>(),
        };
}
