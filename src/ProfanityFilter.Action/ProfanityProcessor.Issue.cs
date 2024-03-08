// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

internal sealed partial class ProfanityProcessor
{
    private async Task<FiltrationResult> HandleIssueAsync(
        long issueNumber,
        Label? label)
    {
        core.StartGroup($"Evaluating issue #{issueNumber} for profanity");

        var filterResult = FiltrationResult.NotFiltered;

        try
        {
            var issue = await client.GetIssueAsync((int)issueNumber);
            if (issue is null)
            {
                core.WriteError($"Unable to get issue with number: {issueNumber}");
                return filterResult;
            }

            var replacementStrategy = core.GetReplacementStrategy();

            filterResult = await ApplyProfanityFilterAsync(
                issue.Title ?? "", issue.Body ?? "", replacementStrategy);

            if (filterResult.IsFiltered)
            {
                var issueUpdate = new IssueUpdate
                {
                    Body = core.GetFinalResultText(filterResult.BodyResult) ?? filterResult.Body,
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

                await client.UpdateIssueAsync((int)issueNumber, issueUpdate);

                if (core.IncludeConfusedReaction())
                {
                    await client.AddReactionAsync(issueNumber, ReactionContent.Confused);
                }
            }
        }
        finally
        {
            core.EndGroup();
        }

        return filterResult;
    }
}
