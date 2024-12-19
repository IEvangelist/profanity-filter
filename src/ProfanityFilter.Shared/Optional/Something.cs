// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Shared.Optional;

/// <summary>
/// Represents a container that contains a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
/// <param name="Value">The value contained in the <see cref="Something{T}"/>.</param>
public readonly record struct Something<T>(T Value) : IMaybe<T>;
