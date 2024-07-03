// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.WebApi.Models;

public sealed record class FilterTargetResponse(
    string Name,
    int Value,
    string Description)
{
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
