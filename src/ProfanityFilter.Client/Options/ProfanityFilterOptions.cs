// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Client.Options;

/// <summary>
/// Options for configuring the profanity filter.
/// </summary>
public sealed class ProfanityFilterOptions
{
    /// <summary>
    /// The section name used for binding configuration to <see cref="ProfanityFilterOptions"/>.
    /// </summary>
    public const string SectionName = "ProfanityFilter";

    /// <summary>
    /// Gets or sets the default strategy for replacing profane words.
    /// </summary>
    public ReplacementStrategy DefaultReplacementStrategy { get; set; }

    /// <summary>
    /// Gets or sets the base address of the API.
    /// </summary>
    public Uri? ApiBaseAddress { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the OpenTelemetry tracing is disabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool DisableTracing { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the database health check is disabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool DisableHealthChecks { get; set; }
}
