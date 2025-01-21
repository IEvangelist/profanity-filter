// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Common;

/// <summary>
/// <inheritdoc cref="JsonSerializerContext" />
/// Including the following types:
/// <list type="bullet">
/// <item><see cref="Common.FilterParameters"/></item>
/// <item><see cref="Common.FilterResult"/></item>
/// <item><see cref="Common.FilterStep"/></item>
/// <item><see cref="Common.FilterTarget"/></item>
/// <item><see cref="Common.ProfaneSourceFilter"/></item>
/// <item><see cref="Api.ProfanityFilterRequest"/></item>
/// <item><see cref="Api.ProfanityFilterResponse"/></item>
/// <item><see cref="Common.ReplacementStrategy"/></item>
/// <item><see cref="Api.StrategyResponse"/></item>
/// </list>
/// </summary>
[JsonSourceGenerationOptions(
    defaults: JsonSerializerDefaults.Web,
    WriteIndented = true,
    UseStringEnumConverter = true,
    AllowTrailingCommas = true,
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    PropertyNameCaseInsensitive = false,
    IncludeFields = true)]
[JsonSerializable(typeof(FilterParameters))]
[JsonSerializable(typeof(FilterResult))]
[JsonSerializable(typeof(FilterStep))]
[JsonSerializable(typeof(FilterStep[]))]
[JsonSerializable(typeof(FilterTarget))]
[JsonSerializable(typeof(FilterTargetResponse[]))]
[JsonSerializable(typeof(ProfaneSourceFilter))]
[JsonSerializable(typeof(ProfanityFilterRequest))]
[JsonSerializable(typeof(ProfanityFilterResponse))]
[JsonSerializable(typeof(ProfanityFilterStep[]))]
[JsonSerializable(typeof(ReplacementStrategy))]
[JsonSerializable(typeof(StrategyResponse))]
[JsonSerializable(typeof(StrategyResponse[]))]
public sealed partial class JsonSerializationContext : JsonSerializerContext;
