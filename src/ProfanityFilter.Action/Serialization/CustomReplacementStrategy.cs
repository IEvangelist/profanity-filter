// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

/// <summary>
/// 
/// </summary>
/// <param name="TitleStrategy"></param>
/// <param name="BodyStrategy"></param>
public sealed record CustomReplacementStrategy(
    CustomStrategy? TitleStrategy,
    CustomStrategy? BodyStrategy);
