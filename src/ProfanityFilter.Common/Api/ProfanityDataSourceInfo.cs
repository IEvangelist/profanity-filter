// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Common.Api;

/// <summary>
/// Represents the statistics for a single profane content data source (word list).
/// </summary>
/// <param name="Name">The friendly name of the data source, for example the
/// language or list name such as <c>"ItalianSwearWords"</c>.</param>
/// <param name="WordCount">The number of profane words or phrases contained in the source.</param>
public sealed record class ProfanityDataSourceInfo(
    string Name,
    int WordCount);
