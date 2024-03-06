// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

internal sealed partial class ProfanityProcessor
{
    private async Task<FiltrationResult> HandlePullRequestAsync(
        long pullRequestNumber,
        Label? label)
    {
        core.StartGroup($"Evaluating pull request #{pullRequestNumber} for profanity");

        var filterResult = FiltrationResult.NotFiltered;

        try
        {
            var pullRequest = await client.GetPullRequestAsync((int)pullRequestNumber);
            if (pullRequest is null)
            {
                core.WriteError($"Unable to get PR with number: {pullRequestNumber}");
                return filterResult;
            }

            var replacementStrategy = core.GetReplacementStrategy();

            filterResult = await ApplyProfanityFilterAsync(
                pullRequest.Title ?? "", pullRequest.Body ?? "", replacementStrategy);

            if (filterResult.IsFiltered)
            {
                var pullRequestUpdate = new PullRequestUpdate
                {
                    Body = core.GetFinalResultText(filterResult.BodyResult) ?? filterResult.Body,
                    Title = filterResult.Title
                };

                await client.UpdatePullRequestAsync(
                    (int)pullRequestNumber, pullRequestUpdate, label?.Name);

                if (core.IncludeConfusedReaction())
                {
                    await client.AddReactionAsync(
                    pullRequestNumber, ReactionContent.Confused);
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
