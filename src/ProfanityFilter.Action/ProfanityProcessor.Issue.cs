// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

internal sealed partial class ProfanityProcessor
{
    private async Task HandleIssueAsync(
        long issueNumber,
        ContextSummaryPair contextSummaryPair,
        Label? label)
    {
        core.StartGroup($"Evaluating issue #{issueNumber} for profanity");

        try
        {
            var issue = await client.GetIssueAsync((int)issueNumber);
            if (issue is null)
            {
                core.Error($"Unable to get issue with number: {issueNumber}");
                return;
            }

            var replacementStrategy = core.GetReplacementStrategy();

            var filterResult = await ApplyProfanityFilterAsync(
                issue.Title ?? "", issue.Body ?? "", replacementStrategy, contextSummaryPair);

            if (filterResult.IsFiltered)
            {
                var issueUpdate = new IssueUpdate
                {
                    Body = filterResult.Body,
                    Title = new()
                    {
                        String = filterResult.Title
                    }
                };

                if (label is not null and { Name.Length: > 0 })
                {
                    if (issueUpdate.Labels is not null)
                    {
                        issueUpdate.Labels.Add(label.Name);
                    }
                    else
                    {
                        issueUpdate.Labels = [label.Name];
                    }
                }

                await client.UpdateIssueAsync(issue.Number.GetValueOrDefault(), issueUpdate);

                // await client.AddReactionAsync(
                //     issue.Id.GetValueOrDefault(), ReactionContent.Confused);
            }
        }
        finally
        {
            core.EndGroup();
        }
    }
}
