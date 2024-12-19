// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Shared;

/// <summary>
/// <inheritdoc cref="JsonSerializerContext" />
/// Including the following types:
/// <list type="bullet">
/// <item><see cref="Shared.FilterParameters"/></item>
/// <item><see cref="Shared.FilterResult"/></item>
/// <item><see cref="Shared.FilterStep"/></item>
/// <item><see cref="Shared.FilterTarget"/></item>
/// <item><see cref="Shared.ProfaneSourceFilter"/></item>
/// <item><see cref="Api.ProfanityFilterRequest"/></item>
/// <item><see cref="Api.ProfanityFilterResponse"/></item>
/// <item><see cref="Shared.ReplacementStrategy"/></item>
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
