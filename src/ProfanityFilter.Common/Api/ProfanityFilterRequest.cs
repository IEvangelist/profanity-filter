// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Common.Api;

/// <summary>
/// A representation of a profanity-filter request.
/// </summary>
/// <param name="Text">The text to evaluate for profanity.</param>
/// <param name="Strategy">The desired replacement strategy to use. Defaults to <c>*</c>.</param>
/// <param name="Target">The filter target to use. Defaults to body, which is Markdown escaped.</param>
public sealed record class ProfanityFilterRequest(
    string Text,
    ReplacementStrategy Strategy = ReplacementStrategy.Asterisk,
    FilterTarget Target = FilterTarget.Body);
