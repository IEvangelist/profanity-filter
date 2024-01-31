// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

internal sealed class ProfanityProcessor(
    CustomGitHubClient client,
    IProfaneContentCensorService profaneContentCensor,
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

            var isIssueComment = context.Payload?.Comment is not null;
            var isIssue = context.Payload?.Issue is not null;
            var isPullRequest = context.Payload?.PullRequest is not null;

            Func<long, Summary, Label?, Task> handler = (isIssueComment, isIssue, isPullRequest) switch
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

            await handler(numberOrId, summary, label ?? null);

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

    private async Task HandleIssueCommentAsync(long issueCommentId, Summary summary, Label? label)
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

            var replacementStrategy = GetInputReplacementStrategy();

            var (text, isFiltered) = await TryApplyFilterAsync(
                issueComment.Body ?? "", replacementStrategy, summary);

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

    private async Task HandleIssueAsync(long issueNumber, Summary summary, Label? label)
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

            var replacementStrategy = GetInputReplacementStrategy();

            var filterResult = await ApplyProfanityFilterAsync(
                issue.Title ?? "", issue.Body ?? "", replacementStrategy, summary);

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

                await client.AddReactionAsync(
                    issue.Id.GetValueOrDefault(), ReactionContent.Confused);
            }
        }
        finally
        {
            core.EndGroup();
        }
    }

    private async Task HandlePullRequestAsync(long pullRequestNumber, Summary summary, Label? label)
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

            var replacementStrategy = GetInputReplacementStrategy();

            var filterResult = await ApplyProfanityFilterAsync(
                pullRequest.Title ?? "", pullRequest.Body ?? "", replacementStrategy, summary);

            if (filterResult.IsFiltered)
            {
                var issueUpdate = new PullRequestUpdate
                {
                    Body = filterResult.Body,
                    Title = filterResult.Title
                };

                await client.UpdatePullRequestAsync(
                    pullRequest.Number.GetValueOrDefault(), issueUpdate, label?.Name);

                await client.AddReactionAsync(
                    pullRequest.Id.GetValueOrDefault(), ReactionContent.Confused);
            }
        }
        finally
        {
            core.EndGroup();
        }
    }

    private async ValueTask<Models.FilterResult> ApplyProfanityFilterAsync(
        string title, string body, ReplacementStrategy replacementStrategy, Summary summary)
    {
        if (string.IsNullOrWhiteSpace(title) &&
            string.IsNullOrWhiteSpace(body))
        {
            return Models.FilterResult.NotFiltered;
        }

        var (resultingTitle, isTitleFiltered) =
            await TryApplyFilterAsync(title, replacementStrategy, summary);
        var (resultingBody, isBodyFiltered) =
            await TryApplyFilterAsync(body, replacementStrategy, summary);

        return new Models.FilterResult(
            resultingTitle,
            isTitleFiltered,
            resultingBody,
            isBodyFiltered);
    }

    private async ValueTask<(string text, bool isFiltered)> TryApplyFilterAsync(
        string text, ReplacementStrategy replacementStrategy, Summary summary)
    {
        var result =
            await profaneContentCensor.CensorProfanityAsync(text, replacementStrategy);

        if (result.IsFiltered)
        {
            SummarizeAppliedFilter(result, replacementStrategy, summary);

            core.Info($"""
                Original text: {text}
                Filtered text: {result.FinalOutput}
                """);
        }

        return (result.FinalOutput ?? result.Input, result.IsFiltered);
    }

    private static void SummarizeAppliedFilter(
        Services.Results.FilterResult result, ReplacementStrategy replacementStrategy, Summary summary)
    {
        summary.AddHeading("🤬 Profanity filter applied", 2);

        var replacement = replacementStrategy switch
        {
            ReplacementStrategy.Emoji => "emoji",
            ReplacementStrategy.MiddleSwearEmoji => "middle swear emoji",
            ReplacementStrategy.RandomAsterisk => "random asterisk",
            ReplacementStrategy.MiddleAsterisk => "middle asterisk",
            ReplacementStrategy.VowelAsterisk => "vowel asterisk",
            ReplacementStrategy.AngerEmoji => "anger emoji",
            _ => "asterisk",
        };

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
    }

    internal ReplacementStrategy GetInputReplacementStrategy()
    {
        // The replacement-strategy input is optional, so we default to asterisk.
        // An example valid values is:
        //  - anger-asterisk
        //  - AngerAsterisk
        return core.GetInput("replacement-strategy") is string value
            && Enum.TryParse<ReplacementStrategy>(
                value: NormalizeEnumString(value),
                ignoreCase: true,
                result: out var strategy)
                    ? strategy
                    : ReplacementStrategy.Asterisk;

        // The values are case-insensitive, and we normalize them to remove "-".
        static string NormalizeEnumString(string enumValue)
        {
            return enumValue.Replace("-", "");
        }
    }
}
