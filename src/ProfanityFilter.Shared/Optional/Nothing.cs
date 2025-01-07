// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Shared.Optional;

/// <summary>
/// Represents a type that signifies the absence of a value.
/// </summary>
/// <typeparam name="T">The type of the value that is absent.</typeparam>
public readonly record struct Nothing<T> : IMaybe<T>
{
    /// <summary>
    /// Implicitly converts the <see cref="Nothing{T}"/> to its contained value of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="nothing">The <see cref="Nothing{T}"/> instance to convert.</param>
    /// <returns>The value contained in the <see cref="Nothing{T}"/>.</returns>
    public static implicit operator T?(Nothing<T> nothing) => default;

    /// <summary>
    /// Implicitly converts the <paramref name="value"/> to a new <see cref="Nothing{T}"/>.
    /// </summary>
    /// <param name="_">The <typeparamref name="T"/> value.</param>
    /// <returns>The new instance of <see cref="Nothing{T}"/>.</returns>
    public static implicit operator Nothing<T>(T _) => new();
}
