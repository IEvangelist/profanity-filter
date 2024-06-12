// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.WebApi.Serialization;

[JsonSourceGenerationOptions(
    defaults: JsonSerializerDefaults.Web,
    WriteIndented = true,
    UseStringEnumConverter = true,
    AllowTrailingCommas = true,
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    PropertyNameCaseInsensitive = false,
    IncludeFields = true)]
[JsonSerializable(typeof(ProfanityFilterRequest))]
[JsonSerializable(typeof(ProfanityFilterResponse))]
[JsonSerializable(typeof(StrategyResponse))]
[JsonSerializable(typeof(StrategyResponse[]))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}