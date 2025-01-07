// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Shared.Extensions;

/// <summary>
/// Provides extension methods for the Maybe type.
/// </summary>
public static class MaybeExtensions
{
    /// <summary>
    /// Converts a value to an instance of <see cref="IMaybe{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <returns>An instance of <see cref="IMaybe{T}"/> containing the value if it is not null; otherwise, an instance of <see cref="Nothing{T}"/>.</returns>
    public static IMaybe<T> AsMaybe<T>(this T? value) => value is null
        ? new Nothing<T>()
        : new Something<T>(value);

    /// <summary>
    /// Chains the current instance of <typeparamref name="TIn"/> to a new instance of <typeparamref name="TOut"/> using the provided function.
    /// </summary>
    /// <typeparam name="TIn">The type of the input value.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="this">The current instance of <typeparamref name="TIn"/>.</param>
    /// <param name="func">The function to transform the input value to the output value.</param>
    /// <returns>An instance of <see cref="IMaybe{TOut}"/> containing the transformed value.</returns>
    public static IMaybe<TOut> Chain<TIn, TOut>(
        this IMaybe<TIn> @this,
        Func<TIn, TOut> func) =>
        @this switch
        {
            Something<TIn> s => new Something<TOut>(func.Invoke(s.Value)),
            _ => new Nothing<TOut>(),
        };
}
