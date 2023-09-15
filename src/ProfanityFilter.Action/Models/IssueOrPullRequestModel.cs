// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Models;

public class IssueOrPullRequestModel
{
    public ID Id { get; set; }

    public required string Title { get;set; }

    public required string Body { get;set; }

    public required string EditorLogin { get;set; }

    public string? BaseRefName { get; set; }

    public int Number { get; set; }

    public List<LabelModel> Labels { get; set; } = new();
}
