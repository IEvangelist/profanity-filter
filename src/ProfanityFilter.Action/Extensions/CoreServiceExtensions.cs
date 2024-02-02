// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Extensions;

internal static class CoreServiceExtensions
{
    private const string UpdatedNote = "This content was automatically updated to _filter out profanity_. " +
        "If you feel as though this was done in error, please " +
        "[file an issue with the details](https://github.com/IEvangelist/profanity-filter/issues/new).";

    /// <summary>
    /// Gets the final result text based on the given <paramref name="result"/>'s final output
    /// and the <paramref name="core"/> service's input for <c>include-updated-note</c>.
    /// </summary>
    public static string? GetFinalResultText(this ICoreService core, FilterResult? result)
    {
        if (result is null)
        {
            return null;
        }

        if (result.IsFiltered == false)
        {
            return result.Input;
        }

        var finalResult = result.FinalOutput;

        // We do not add updated notes to titles.
        if (result.Parameters.Target is FilterTarget.Title)
        {
            return finalResult;
        }

        // Consumers can opt-out of this feature.
        if (core.GetBoolInput("include-updated-note") is false)
        {
            return finalResult;
        }

        // If the result already contains the note, we don't need to add it again.
        // This can occur if a user has already filed an issue, and we've updated the content.
        if (finalResult.Contains(UpdatedNote))
        {
            return finalResult;
        }

        return $"{finalResult}{Env.NewLine}{Env.NewLine}{UpdatedNote}";
    }

    /// <summary>
    /// The <c>replacement-strategy</c> to use when filtering profane content.
    /// Example valid values: <c>anger-asterisk</c>, <c>AngerAsterisk</c>
    /// </summary>
    public static ReplacementStrategy GetReplacementStrategy(this ICoreService core)
    {
        return core.GetInput("replacement-strategy") is string value
            && Enum.TryParse<ReplacementStrategy>(
                value: NormalizeEnumString(value),
                ignoreCase: true,
                result: out var strategy)
                    ? strategy
                    : ReplacementStrategy.Asterisk;

        // The values are case-insensitive, and we normalize them to remove "-".
        static string NormalizeEnumString(string enumValue)
        {
            return enumValue.Replace("-", "");
        }
    }
}
