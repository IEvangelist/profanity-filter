// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Common.Monads;

/// <summary>
/// Represents a type that signifies the absence of a value.
/// </summary>
/// <typeparam name="T">The type of the value that is absent.</typeparam>
public readonly record struct Absent<T> : IMaybe<T>
{
    /// <summary>
    /// Implicitly converts the <see cref="Absent{T}"/> to its contained value of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="_">The <see cref="Absent{T}"/> instance to convert.</param>
    /// <returns>The value contained in the <see cref="Absent{T}"/>.</returns>
    public static implicit operator T?(Absent<T> _) => default;

    /// <summary>
    /// Implicitly converts the <paramref name="_"/> to a new <see cref="Absent{T}"/>.
    /// </summary>
    /// <param name="_">The <typeparamref name="T"/> value.</param>
    /// <returns>The new instance of <see cref="Absent{T}"/>.</returns>
    public static implicit operator Absent<T>(T _) => new();
}
