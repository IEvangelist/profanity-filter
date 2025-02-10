// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Aspire.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Represents metadata for the:
/// </summary>
/// <remarks>
/// <inheritdoc cref="Registry"/>/<inheritdoc cref="Image"/>:<inheritdoc cref="Tag"/> container image.
/// </remarks>
internal static class ProfanityFilterContainerImageDetails
{
    /// <remarks>ghcr.io</remarks>
    internal const string Registry = "ghcr.io";

    /// <remarks>ievangelist/profanity-filter-api</remarks>
    internal const string Image = "ievangelist/profanity-filter-api";

    /// <remarks>9.0.1</remarks>
    internal const string Tag = "9.0.1";
}
