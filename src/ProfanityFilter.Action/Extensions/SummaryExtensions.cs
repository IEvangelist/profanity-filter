// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Extensions;

internal static partial class SummaryExtensions
{
    /// <summary>
    /// Adds a markdown level-2 heading: <c>## 🤬 Profanity filter applied</c>.
    /// </summary>
    internal static Summary AddProfanityAppliedHeading(this Summary summary)
    {
        return summary.AddMarkdownHeading("🤬 Profanity filter applied", 2);
    }

    /// <summary>
    /// Adds a markdown paragraph, including links to the context and user.
    /// </summary>
    internal static Summary AddSectionParagraphFor(this Summary summary, Context context)
    {
        if (context.ToContextualHeaderSummary() is { } header)
        {
            summary.AddRawMarkdown(header, true);
        }

        return summary;
    }

    /// <summary>
    /// Adds a markdown level-3 heading with the given <paramref name="sectionHeading"/>,
    /// and summarizes the given <paramref name="result"/> as a markdown table, and unordered list.
    /// </summary>
    internal static Summary AddFilterResultSection(this Summary summary, string sectionHeading, FilterResult? result)
    {
        if (result is null or { IsFiltered: false })
        {
            return summary;
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

        return summary;
    }

    /// <summary>
    /// Adds a markdown blurb on configuring replacement strategies, with a link.
    /// </summary>
    internal static Summary AddReplacementStrategyLink(this Summary summary)
    {
        return summary.AddRawMarkdown($"""
                For more information on configuring replacement types, see [Profanity Filter: 😵 Replacement strategies](https://github.com/IEvangelist/profanity-filter?tab=readme-ov-file#-replacement-strategies).
                """);
    }

    /// <summary>
    /// Adds an HTML collapsible section, enclosing the given
    /// <paramref name="context"/> as a JSON code block.
    /// </summary>
    internal static Summary AddContextAsDetailsJson(this Summary summary, Context context)
    {
        summary.AddDetails("Context Details", $"""
            {Env.NewLine}{Env.NewLine}
            ```json
            {context}
            ```{Env.NewLine}{Env.NewLine}
            """);

        summary.AddNewLine();
        summary.AddNewLine();

        return summary;
    }

    /// <summary>
    /// Adds a markdown quote block, stating the run time of the filter, for example:
    /// <c>"&gt; The _Potty Mouth_ profanity filter ran in 420 milliseconds."</c>
    /// </summary>
    internal static Summary AddQuotedTotalRunTime(this Summary summary, TimeSpan elapsedTime)
    {
        summary.AddRawMarkdown(
            $"> The _Potty Mouth_ profanity filter ran in {ToHumanReadableTimeSpan(elapsedTime)}.");

        return summary;

        static string ToHumanReadableTimeSpan(TimeSpan time)
        {
            return time switch
            {
                { TotalSeconds: >= 2 } => $"{time.Seconds} seconds and {time.Milliseconds} milliseconds",
                { TotalSeconds: > 1 } => $"{time.Seconds} second and {time.Milliseconds} milliseconds",
                _ => $"{time.Milliseconds} milliseconds"
            };
        }
    }

    [GeneratedRegex(@"\r\n?|\n")]
    private static partial Regex NewLineRegex();
}
