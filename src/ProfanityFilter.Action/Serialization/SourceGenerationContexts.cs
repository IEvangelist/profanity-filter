// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

[JsonSourceGenerationOptions(
    AllowTrailingCommas = true,
    UseStringEnumConverter = true)]
[JsonSerializable(typeof(CustomStrategy))]
[JsonSerializable(typeof(CustomReplacementStrategy))]
internal partial class SourceGenerationContexts : JsonSerializerContext
{
}
