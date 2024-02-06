// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

/// <summary>
/// 
/// </summary>
/// <param name="Regex"></param>
/// <param name="Replacement"></param>
/// <param name="IsRandomLength"></param>
/// <param name="Enclose"></param>
public sealed record CustomStrategy(
    string? Regex,
    string? Replacement,
    bool? IsRandomLength,
    string? Enclose);
