// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

internal sealed partial class ProfanityProcessor
{
    private async Task<FiltrationResult> HandleIssueCommentAsync(
        long issueCommentId,
        Label? label)
    {
        // We don't apply labels to issue comments, discard it...
        _ = label;

        core.StartGroup($"Evaluating issue comment id #{issueCommentId} for profanity");

        var filterResult = FiltrationResult.NotFiltered;

        try
        {
            var issueComment = await client.GetIssueCommentAsync(issueCommentId);
            if (issueComment is null)
            {
                core.WriteError($"Unable to get issue comment with id: {issueCommentId}");
                return filterResult;
            }

            var replacementStrategy = core.GetReplacementStrategy();

            var additionalFilters = await GetAdditionalFiltersAsync();

            var bodyFilterResult = await TryApplyFilterAsync(
                issueComment.Body ?? "", new(replacementStrategy, FilterTarget.Comment)
                {
                    AdditionalFilterSources = additionalFilters
                });

            filterResult = new FiltrationResult(BodyResult: bodyFilterResult);

            if (bodyFilterResult.IsFiltered)
            {
                var finalBodyUpdate = core.GetFinalResultText(bodyFilterResult)
                    ?? bodyFilterResult.FinalOutput;

                await client.UpdateIssueCommentAsync(issueCommentId, finalBodyUpdate);

                if (core.IncludeConfusedReaction())
                {
                    var issueNumber = (int)_context!.Payload!.Issue!.Number;
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
