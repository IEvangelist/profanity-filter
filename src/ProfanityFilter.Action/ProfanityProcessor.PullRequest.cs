// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

internal sealed partial class ProfanityProcessor
{
    private async Task HandlePullRequestAsync(
        long pullRequestNumber,
        ContextSummaryPair contextSummaryPair,
        Label? label)
    {
        core.StartGroup($"Evaluating pull request #{pullRequestNumber} for profanity");

        try
        {
            var pullRequest = await client.GetPullRequestAsync((int)pullRequestNumber);
            if (pullRequest is null)
            {
                core.Error($"Unable to get PR with number: {pullRequestNumber}");
                return;
            }

            var replacementStrategy = core.GetReplacementStrategy();

            var filterResult = await ApplyProfanityFilterAsync(
                pullRequest.Title ?? "", pullRequest.Body ?? "", replacementStrategy, contextSummaryPair);

            if (filterResult.IsFiltered)
            {
                var issueUpdate = new PullRequestUpdate
                {
                    Body = filterResult.Body,
                    Title = filterResult.Title
                };

                await client.UpdatePullRequestAsync(
                    pullRequest.Number.GetValueOrDefault(), issueUpdate, label?.Name);

                // await client.AddReactionAsync(
                //     pullRequest.Id.GetValueOrDefault(), ReactionContent.Confused);
            }
        }
        finally
        {
            core.EndGroup();
        }
    }
}
