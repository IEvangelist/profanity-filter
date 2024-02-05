// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

internal sealed partial class ProfanityProcessor(
    CustomGitHubClient client,
    IProfaneContentFilterService profaneContentFilter,
    ICoreService core)
{
    private Context? _context;

    public async Task ProcessProfanityAsync()
    {
        var success = true;

        Summary summary = new();
        var startingTimestamp = Stopwatch.GetTimestamp();

        try
        {
            if (TryGetContext(out var context) is false)
            {
                return;
            }

            var isIssueComment = context.Payload?.Comment is not null;
            var isIssue = context.Payload?.Issue is not null;
            var isPullRequest = context.Payload?.PullRequest is not null;

            Func<long, Label?, Task<FiltrationResult>> handler = (isIssueComment, isIssue, isPullRequest) switch
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

            var result = await handler(numberOrId, label ?? null);

            ContextSummaryPair contextSummaryPair = (context, summary);

            var elapsedTime = Stopwatch.GetElapsedTime(startingTimestamp);

            await SummarizeAppliedFiltersAsync(result, contextSummaryPair, elapsedTime);            
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

    [MemberNotNullWhen(true, nameof(_context))]
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

            _context = context;

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
        string title, string body, ReplacementStrategy replacementStrategy)
    {
        if (string.IsNullOrWhiteSpace(title) &&
            string.IsNullOrWhiteSpace(body))
        {
            return FiltrationResult.NotFiltered;
        }

        var titleResult = await TryApplyFilterAsync(
            title, parameters: new(replacementStrategy, FilterTarget.Title));
        var bodyResult = await TryApplyFilterAsync(
            body, parameters: new(replacementStrategy, FilterTarget.Body));

        return new FiltrationResult(titleResult, bodyResult);
    }

    private async ValueTask<FilterResult> TryApplyFilterAsync(
        string text, FilterParameters parameters)
    {
        var result = await profaneContentFilter.FilterProfanityAsync(
            text, parameters);

        if (result.IsFiltered)
        {
            var type = parameters.Target switch
            {
                FilterTarget.Title => "title",
                FilterTarget.Body => "body",
                FilterTarget.Comment => "comment",

                _ => "unknown"
            };

            core.Info($"""
                Original {type} text: {text}
                Filtered {type} text: {result.FinalOutput}
                """);
        }

        return result;
    }
}
