// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Extensions;

internal static class CoreServiceExtensions
{
    public static ReplacementStrategy GetReplacementStrategy(this ICoreService core)
    {
        // The replacement-strategy input is optional, so we default to asterisk.
        // An example valid values is:
        //  - anger-asterisk
        //  - AngerAsterisk
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

    public static bool ShouldIncludeUpdatedNote(this ICoreService core)
    {
        return core.GetBoolInput("include-updated-note");
    }
}
