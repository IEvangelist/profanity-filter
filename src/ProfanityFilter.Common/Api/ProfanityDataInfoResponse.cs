// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Common.Api;

/// <summary>
/// Represents aggregate statistics describing the profane content dictionaries
/// (word lists) that back the profanity filter.
/// </summary>
/// <param name="TotalSources">The total number of data sources (word-list files).</param>
/// <param name="TotalWords">The total number of profane words and phrases across all sources.</param>
/// <param name="Sources">The per-source breakdown of names and word counts,
/// ordered by descending word count.</param>
public sealed record class ProfanityDataInfoResponse(
    int TotalSources,
    int TotalWords,
    ProfanityDataSourceInfo[] Sources);
