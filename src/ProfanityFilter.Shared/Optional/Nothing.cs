// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Shared.Optional;

/// <summary>
/// Represents a type that signifies the absence of a value.
/// </summary>
/// <typeparam name="T">The type of the value that is absent.</typeparam>
public readonly record struct Nothing<T> : IMaybe<T>;
