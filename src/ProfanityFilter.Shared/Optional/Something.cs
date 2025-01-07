// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Shared.Optional;

/// <summary>
/// Represents a container that contains a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
/// <param name="Value">The value contained in the <see cref="Something{T}"/>.</param>
public readonly record struct Something<T>(T Value) : IMaybe<T>
{
    /// <summary>
    /// Implicitly converts the <see cref="Something{T}"/> to its contained value of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="something">The <see cref="Something{T}"/> instance to convert.</param>
    /// <returns>The value contained in the <see cref="Something{T}"/>.</returns>
    public static implicit operator T(Something<T> something) => something.Value;

    /// <summary>
    /// Implicitly converts the <paramref name="value"/> to a new <see cref="Something{T}"/>.
    /// </summary>
    /// <param name="value">The <typeparamref name="T"/> value.</param>
    /// <returns>The new instance of <see cref="Something{T}"/>.</returns>
    public static implicit operator Something<T>(T value) => new(value);
}
