// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

internal sealed partial class ProfanityProcessor
{
    private async Task HandleIssueCommentAsync(
        long issueCommentId,
        ContextSummaryPair contextSummaryPair,
        Label? label)
    {
        // We don't apply labels to issue comments, discard it...
        _ = label;

        core.StartGroup($"Evaluating issue comment id #{issueCommentId} for profanity");

        try
        {
            var issueComment = await client.GetIssueCommentAsync(issueCommentId);
            if (issueComment is null)
            {
                core.Error($"Unable to get issue comment with id: {issueCommentId}");
                return;
            }

            var replacementStrategy = core.GetReplacementStrategy();

            var (text, isFiltered) = await TryApplyFilterAsync(
                issueComment.Body ?? "", new(replacementStrategy, FilterTarget.Comment), contextSummaryPair);

            if (isFiltered)
            {
                await client.UpdateIssueCommentAsync(issueCommentId, text);
            }
        }
        finally
        {
            core.EndGroup();
        }
    }
}
