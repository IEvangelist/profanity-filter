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
        var issueUrl = context.Payload!.Issue!.HtmlUrl;

        var linkedIssueOrPullRequest = context.EventName switch
        {
            "pull_request" => $"pull request [#{number}]({issueUrl}#{number})",
            "issues" => $"issue [#{number}]({issueUrl}#{number})",
            "issue_comment" => $"issue [#{number} (comment)]({issueUrl}#issuecomment-{context.Payload!.Comment!.Id})",

            _ => "issue or pull request"
        };

        var eventName = context.EventName;
        var action = context.Action;

        var headerSummary = $"The **:octocat: [IEvangelist/profanity-filter](https://github.com/IEvangelist/profanity-filter)** " +
            $"GitHub Action ran as part of the \"_{eventName}_\" event with the \"_{action}_\" action. " +
            $"In {linkedIssueOrPullRequest}, the user @{context.Actor} wrote content that triggered " +
            "the configured replacement to filter profane content.";

        return headerSummary;
    }
}
