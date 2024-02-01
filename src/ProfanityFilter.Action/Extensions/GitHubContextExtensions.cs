// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Extensions;

internal static class GitHubContextExtensions
{
    internal static string? ToContextualHeaderSummary(this Context context)
    {
        if (context is null)
        {
            return null;
        }

        if (context is { Payload: null, EventName: null })
        {
            return null;
        }

        var number = context.Issue.Number;
        var linkedIssueOrPullRequest = context.EventName switch
        {
            "pull_request" => $"pull request #{number}",
            "issues" => $"issue #{number}",
            "issue_comment" => $"issue {context.Payload!.Issue!.HtmlUrl}#issuecomment-{context.Payload!.Comment!.Id}",

            _ => "issue or pull request"
        };

        var eventName = context.EventName;
        var action = context.Action;
        var repository = context.Payload!.Repository!;

        return $"""
            The :octocat: [{repository.FullName}]({repository.HtmlUrl}) GitHub Action ran as part of the "_{eventName}_" event with the "_{action}_" action.
            In the {linkedIssueOrPullRequest}, the user @{context.Actor} wrote content that triggered the configured replacement to filter profane content.
            """;
    }
}
