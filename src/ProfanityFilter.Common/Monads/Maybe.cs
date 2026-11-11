// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Common.Monads;

/// <summary>
/// Represents a union of <see cref="Available{T}"/> and <see cref="Absent{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the value that may be present.</typeparam>
/// <remarks>
/// The default value represents absence. Case values are stored inline and can be
/// inspected with union patterns without boxing.
/// </remarks>
[System.Runtime.CompilerServices.Union]
public readonly record struct Maybe<T> : Maybe<T>.IUnionMembers
{
    private readonly Available<T> _available;
    private readonly bool _isAvailable;

    private Maybe(Available<T> value) => (_available, _isAvailable) = (value, true);

    /// <summary>
    /// Provides the compiler-recognized union members. Prefer pattern matching on
    /// <see cref="Maybe{T}"/> rather than accessing this interface directly.
    /// </summary>
    public interface IUnionMembers
    {
#pragma warning disable CA1000 // C# union member providers require static factories.
        /// <summary>
        /// Creates a union containing the available case.
        /// </summary>
        /// <param name="value">The available case.</param>
        /// <returns>The union containing the case.</returns>
        public static Maybe<T> Create(Available<T> value) => new(value);

        /// <summary>
        /// Creates a union containing the absent case.
        /// </summary>
        /// <param name="_">The absent case.</param>
        /// <returns>The union representing absence.</returns>
        public static Maybe<T> Create(Absent<T> _) => default;
#pragma warning restore CA1000

        /// <summary>
        /// Gets the boxed case for object-based union access.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets whether the union contains a non-null case. Both available and
        /// absent cases are non-null, including the default union value.
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// Gets the available case without boxing, if present.
        /// </summary>
        /// <param name="value">The available case.</param>
        /// <returns>Whether the union contains the available case.</returns>
        public bool TryGetValue(out Available<T> value);

        /// <summary>
        /// Gets the absent case without boxing, if present.
        /// </summary>
        /// <param name="value">The absent case.</param>
        /// <returns>Whether the union contains the absent case.</returns>
        public bool TryGetValue(out Absent<T> value);
    }

    object IUnionMembers.Value => _isAvailable ? _available : (object)default(Absent<T>);

    bool IUnionMembers.HasValue => true;

    bool IUnionMembers.TryGetValue(out Available<T> value)
    {
        value = _available;
        return _isAvailable;
    }

    bool IUnionMembers.TryGetValue(out Absent<T> value)
    {
        value = default;
        return !_isAvailable;
    }
}
