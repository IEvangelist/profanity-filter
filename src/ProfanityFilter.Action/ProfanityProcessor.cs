// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

internal sealed partial class ProfanityProcessor(
    CustomGitHubClient client,
    IProfaneContentFilterService profaneContentFilter,
    ICoreService core)
{
    public async Task ProcessProfanityAsync()
    {
        var success = true;

        Summary summary = new();

        try
        {
            if (TryGetContext(out var context) is false)
            {
                return;
            }

            ContextSummaryPair pair = (context, summary);

            var isIssueComment = context.Payload?.Comment is not null;
            var isIssue = context.Payload?.Issue is not null;
            var isPullRequest = context.Payload?.PullRequest is not null;

            Func<long, ContextSummaryPair, Label?, Task> handler = (isIssueComment, isIssue, isPullRequest) switch
            {
                (true, _, _) => HandleIssueCommentAsync,
                (_, true, _) => HandleIssueAsync,
                (_, _, true) => HandlePullRequestAsync,

                _ => throw new Exception(
                    "The profanity filter GitHub Action only works with issues or pull requests.")
            };

            var label = isIssueComment
                ? null
                : await client.GetLabelAsync() ?? await client.CreateLabelAsync();

            if (label is null && isIssueComment is false)
            {
                core.Warning("""
                    The expected label isn't present, a label with the following name would have been applied if found.
                        'profane content 🤬'
                    """);
            }

            var numberOrId = (context.Payload?.Comment?.Id
                ?? context.Payload?.Issue?.Number
                ?? context.Payload?.PullRequest?.Number
                ?? 0L)!;

            await handler(numberOrId, pair, label ?? null);

            if (!summary.IsBufferEmpty)
            {
                await summary.WriteAsync(new() { Overwrite = true });
            }
        }
        catch (Exception ex)
        {
            success = false;

            core.SetFailed(ex.ToString());
            Env.Exit(Env.ExitCode);
        }
        finally
        {
            if (success)
            {
                core.Info("Profanity filter completed successfully.");
                Env.Exit(0);
            }
        }
    }

    private bool TryGetContext([NotNullWhen(true)] out Context? context)
    {
        try
        {
            context = Context.Current;
            core.Info(context.ToString() ?? "Unknown context");

            var isValidAction = context.Action switch
            {
                "profanity-filter" or
                "opened" or "reopened" or
                "created" or "edited" => true,

                _ when (context.Action ?? "").StartsWith("__run") => true,

                _ => false
            };

            if (isValidAction is false)
            {
                core.Warning($"The action '{context.Action}' is not supported.");

                return false;
            }

            var isValidActor = context.Actor switch
            {
                "bot" or
                "dependabot" or "dependabot[bot]" or
                "github-actions" or "github-actions[bot]" => false,

                _ => true
            };

            if (isValidActor is false)
            {
                core.Info($"Ignored as {context.Actor} triggered this...");

                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            core.Error($"""
                Error attempting to get the context:
                  {ex}
                """);
        }

        context = null;

        return false;
    } 

    private async ValueTask<FiltrationResult> ApplyProfanityFilterAsync(
        string title, string body, ReplacementStrategy replacementStrategy, ContextSummaryPair contextSummaryPair)
    {
        if (string.IsNullOrWhiteSpace(title) &&
            string.IsNullOrWhiteSpace(body))
        {
            return FiltrationResult.NotFiltered;
        }

        var (resultingTitle, isTitleFiltered) =
            await TryApplyFilterAsync(title, new(replacementStrategy, FilterTarget.Title), contextSummaryPair);
        var (resultingBody, isBodyFiltered) =
            await TryApplyFilterAsync(body, new(replacementStrategy, FilterTarget.Body), contextSummaryPair);

        return new FiltrationResult(
            resultingTitle,
            isTitleFiltered,
            resultingBody,
            isBodyFiltered);
    }

    private async ValueTask<(string text, bool isFiltered)> TryApplyFilterAsync(
        string text, FilterParameters parameters, ContextSummaryPair contextSummaryPair)
    {
        var result = await profaneContentFilter.FilterProfanityAsync(
            text, parameters);

        if (result.IsFiltered)
        {
            SummarizeAppliedFilter(result, parameters, contextSummaryPair);

            core.Info($"""
                Original text: {text}
                Filtered text: {result.FinalOutput}
                """);
        }

        return (result.FinalOutput ?? result.Input, result.IsFiltered);
    }

    private static void SummarizeAppliedFilter(
        FilterResult result, FilterParameters parameters, ContextSummaryPair contextSummaryPair)
    {
        var (context, summary) = contextSummaryPair;

        summary.AddHeading("🤬 Profanity filter applied", 2);

        var replacement = parameters.Strategy.ToSummaryString();

        // TODO: add details about the issue or pr, with links, etc.
        // Log the offending actor too...

        summary.AddRawMarkdown($"""
                The following table details the _original_ text and the resulting text after it was _filtered_ using the configured "{replacement}" replacement strategy:
                """, true);

        List<SummaryTableRow> rows =
        [
            new SummaryTableRow(Cells: [
                new(Data: "Original text"),
                new(Data: result.Input)
            ]),
        ];

        foreach (var step in result.Steps ?? [])
        {
            if (step.IsFiltered)
            {
                rows.Add(new SummaryTableRow(Cells: [
                    new(Data: $"After _{step.ProfaneSourceData}_ filter"),
                    new(Data: step.Output)
                ]));
            }
        }

        var table = new SummaryTable(
            Heading: new([
                new("", Alignment: TableColumnAlignment.Right),
                new("Values", Alignment: TableColumnAlignment.Left),
            ]),
            Rows: [..rows]);

        summary.AddMarkdownTable(table);

        summary.AddNewLine();

        summary.AddRawMarkdown($"""
                For more information on configuring replacement types, see [Profanity Filter: 😵 Replacement strategies](https://github.com/IEvangelist/profanity-filter?tab=readme-ov-file#-replacement-strategies).
                """);

        summary.AddDetails("Context Details", $"""
            {Env.NewLine}{Env.NewLine}
            ```json
            {context}
            ```{Env.NewLine}{Env.NewLine}
            """);
    }
}
