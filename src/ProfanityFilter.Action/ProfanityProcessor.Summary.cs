// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

internal sealed partial class ProfanityProcessor
{
    private static async Task SummarizeAppliedFiltersAsync(
        FiltrationResult result, ContextSummaryPair contextSummaryPair, TimeSpan elapsedTime)
    {
        if (result.IsFiltered == false)
        {
            return;
        }

        var (context, summary) = contextSummaryPair;

        summary.AddProfanityAppliedHeading()
            .AddSectionParagraphFor(context)
            .AddFilterResultSection("Title Changes", result.TitleResult)
            .AddFilterResultSection("Body Changes", result.BodyResult)
            .AddReplacementStrategyLink()
            .AddContextAsDetailsJson(context)
            .AddQuotedTotalRunTime(elapsedTime);

        if (!summary.IsBufferEmpty)
        {
            await summary.WriteAsync(new() { Overwrite = true });
        }
    }
}
