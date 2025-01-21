// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Common.Monads;

/// <summary>
/// Represents a container that contains a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
/// <param name="Value">The value contained in the <see cref="Available{T}"/>.</param>
public readonly record struct Available<T>(T Value) : IMaybe<T>
{
    /// <summary>
    /// Implicitly converts the <see cref="Available{T}"/> to its contained value of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="some">The <see cref="Available{T}"/> instance to convert.</param>
    /// <returns>The value contained in the <see cref="Available{T}"/>.</returns>
    public static implicit operator T(Available<T> some) => some.Value;

    /// <summary>
    /// Implicitly converts the <paramref name="value"/> to a new <see cref="Available{T}"/>.
    /// </summary>
    /// <param name="value">The <typeparamref name="T"/> value.</param>
    /// <returns>The new instance of <see cref="Available{T}"/>.</returns>
    public static implicit operator Available<T>(T value) => new(value);
}
