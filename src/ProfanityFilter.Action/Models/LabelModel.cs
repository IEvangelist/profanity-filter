// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Models;

public record class LabelModel(
    string Name, ID Id, string? Color = null, string? Description = null)
{
    public static explicit operator LabelModel(GraphQLLabel label) =>
        new(label.Name, label.Id, label.Color, label.Description);
}
