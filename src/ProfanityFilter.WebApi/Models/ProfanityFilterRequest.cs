// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.WebApi.Models;

/// <summary>
/// A representation of a profanity-filter request.
/// </summary>
/// <param name="Text">The text to evaluate for profanity.</param>
/// <param name="Strategy">The desired replacement strategy to use. Defaults to <c>*</c>.</param>
public sealed record class ProfanityFilterRequest(
    string Text,
    ReplacementStrategy Strategy = ReplacementStrategy.Asterisk);
