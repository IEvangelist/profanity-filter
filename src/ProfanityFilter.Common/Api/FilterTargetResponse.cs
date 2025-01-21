// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Common.Api;

/// <summary>
/// Represents a response for a filter target.
/// </summary>
/// <param name="Name">The name of the filter target.</param>
/// <param name="Value">The value of the filter target.</param>
/// <param name="Description">The description of the filter target.</param>
public sealed record class FilterTargetResponse(
    string Name,
    int Value,
    string Description)
{
    /// <summary>
    /// Converts a <see cref="FilterTarget"/> to a <see cref="FilterTargetResponse"/>.
    /// </summary>
    /// <param name="target">The filter target to convert.</param>
    /// <returns>A <see cref="FilterTargetResponse"/> representing the filter target.</returns>
    public static implicit operator FilterTargetResponse(FilterTarget target)
    {
        var description = target switch
        {
            FilterTarget.Title => "Used to indicate that the target of the filter should not include escaped Markdown.",
            FilterTarget.Body => "Used to indicate that the target of the filter should include escaped Markdown.",
            _ => "Same as body, used to indicate that the target of the filter should include escaped Markdown.",
        };

        return new FilterTargetResponse(
            Name: Enum.GetName(target) ?? target.ToString(),
            Value: (int)target,
            Description: description);
    }
}
