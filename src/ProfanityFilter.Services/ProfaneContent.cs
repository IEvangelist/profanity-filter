// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

internal readonly record struct ProfaneContent(
    [property: JsonPropertyName("words")] string[] Words);
