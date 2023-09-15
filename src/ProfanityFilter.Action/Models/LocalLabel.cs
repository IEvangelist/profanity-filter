// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Models;

public record class LocalLabel(
    string Name, string Color, ID Id, string Description)
{
    public static explicit operator LocalLabel(GraphQLLabel label) =>
        new(label.Name, label.Color, label.Id, label.Description);
}
