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

        summary.AddMarkdownHeading("🤬 Profanity filter applied", 2);

        if (context.ToContextualHeaderSummary() is { } header)
        {
            summary.AddRawMarkdown(header, true);
        }

        SummarizeFilterResult(summary, "Title Changes", result.TitleResult);
        SummarizeFilterResult(summary, "Body Changes", result.BodyResult);

        summary.AddRawMarkdown($"""
                For more information on configuring replacement types, see [Profanity Filter: 😵 Replacement strategies](https://github.com/IEvangelist/profanity-filter?tab=readme-ov-file#-replacement-strategies).
                """);

        summary.AddDetails("Context Details", $"""
            {Env.NewLine}{Env.NewLine}
            ```json
            {context}
            ```{Env.NewLine}{Env.NewLine}
            """);

        summary.AddNewLine();
        summary.AddNewLine();

        summary.AddRawMarkdown($"> The _Potty Mouth_ profanity filter ran in {elapsedTime:g}.");

        if (!summary.IsBufferEmpty)
        {
            await summary.WriteAsync(new() { Overwrite = true });
        }
    }

    private static void SummarizeFilterResult(Summary summary, string sectionHeading, FilterResult? result)
    {
        if (result is null or { IsFiltered: false })
        {
            return;
        }

        summary.AddMarkdownHeading(sectionHeading, 3);

        var replacement = result.Parameters.Strategy.ToSummaryString();

        summary.AddRawMarkdown($"""
                The following table details the _original_ text and the resulting text after each matching filter was applied using the configured "{replacement}" replacement strategy:
                """, true);

        List<SummaryTableRow> rows =
        [
            new SummaryTableRow(Cells: [
                new(Data: "Original text"),
                new(Data: ReplaceNewLinesWithHtmlBreaks(result.Input))
            ]),
        ];

        static string ReplaceNewLinesWithHtmlBreaks(string content)
        {
            return NewLineRegex().Replace(content, "<br>");
        }

        foreach (var step in result.Steps ?? [])
        {
            if (step.IsFiltered)
            {
                rows.Add(new SummaryTableRow(Cells: [
                    new(Data: $"After _{step.ProfaneSourceData}_ filter"),
                    new(Data: ReplaceNewLinesWithHtmlBreaks(step.Output))
                ]));
            }
        }

        var table = new SummaryTable(
            Heading: new([
                new("", Alignment: TableColumnAlignment.Right),
                new("Values", Alignment: TableColumnAlignment.Left),
            ]),
        Rows: [.. rows]);

        summary.AddMarkdownTable(table);

        summary.AddNewLine();

        if (result is { Matches.Count: > 0 })
        {
            summary.AddRawMarkdown("The following words (or phrases) were considered profane:", true);

            summary.AddMarkdownList(result.Matches.Select(match => $"\"{match}\""));

            summary.AddNewLine();
        }
    }

    [GeneratedRegex(@"\r\n?|\n")]
    private static partial Regex NewLineRegex();
}
