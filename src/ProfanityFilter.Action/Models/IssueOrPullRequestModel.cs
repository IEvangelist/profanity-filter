// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Models;

public class IssueOrPullRequestModel
{
    public ID Id { get; set; }

    public string Title { get;set; }

    public string Body { get;set; }

    public string EditorLogin { get;set; }

    public string BaseRefName { get; set; }

    public int Number { get; set; }

    public List<LabelModel> Labels { get; set; }
}
