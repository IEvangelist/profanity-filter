// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

internal sealed partial class ProfanityProcessor(
    ICustomGitHubClient client,
    IProfaneContentFilterService profaneContentFilter,
    ICoreService core)
{
    private Context? _context;

    public async Task ProcessProfanityAsync(Context? context)
    {
        var startingTimestamp = Stopwatch.GetTimestamp();

        try
        {
            if (TryValidateContext(context) is false)
            {
                return;
            }

            var payloadType = GetPayloadTypeFrom(context);

            core.WriteInfo($"Processing as 'PayloadType.{payloadType}'.");

            Func<long, Label?, Task<FiltrationResult>> handler = payloadType switch
            {
                PayloadType.PullRequest => HandlePullRequestAsync,
                PayloadType.Issue => HandleIssueAsync,
                _  => HandleIssueCommentAsync
            };

            var label = payloadType is PayloadType.IssueComment
                ? null
                : await client.GetLabelAsync() ?? await client.CreateLabelAsync();

            if (label is null && payloadType is not PayloadType.IssueComment)
            {
                core.WriteWarning("""
                    The expected label isn't present, a label with the following name would have been applied if found.
                        'profane content 🤬'
                    """);
            }

            var numberOrId = (context.Payload?.Comment?.Id
                ?? context.Payload?.Issue?.Number
                ?? context.Payload?.PullRequest?.Number
                ?? 0L)!;

            var result = await handler(numberOrId, label ?? null);

            ContextSummaryPair contextSummaryPair = (context, core.Summary);

            var elapsedTime = Stopwatch.GetElapsedTime(startingTimestamp);

            await SummarizeAppliedFiltersAsync(result, contextSummaryPair, elapsedTime);
        }
        catch (Exception ex)
        {
            core.SetFailed(ex.ToString());
            Env.Exit(Env.ExitCode);
        }
        finally
        {
            var elapsedTime = Stopwatch.GetElapsedTime(startingTimestamp);
            core.WriteInfo($"Profanity filter completed successfully in {elapsedTime.TotalSeconds:#,0.##}s.");
            Env.Exit(0);
        }
    }

    private static PayloadType GetPayloadTypeFrom(Context context)
    {
        var isIssueComment = context.Payload?.Comment is not null;
        var isIssue = context.Payload?.Issue is not null;
        var isPullRequest = context.Payload?.PullRequest is not null;

        if (isIssueComment)
        {
            return PayloadType.IssueComment;
        }

        if (isIssue)
        {
            return PayloadType.Issue;
        }

        if (isPullRequest)
        {
            return PayloadType.PullRequest;
        }

        throw new Exception(
            "The profanity filter GitHub Action only works with issues, issue comments, or pull requests.");
    }

    [MemberNotNullWhen(true, nameof(_context))]
    private bool TryValidateContext([NotNullWhen(true)] Context? context)
    {
        try
        {
            if (context is null)
            {
                return false;
            }

            core.StartGroup("Initializing context");
            core.WriteInfo(context.ToString() ?? "Unknown context");

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
                core.WriteWarning($"The action '{context.Action}' is not supported.");

                return false;
            }

            var isValidActor = context.Actor switch
            {
                "bot" or
                "azure-sdk" or "azure-sdk[bot]" or
                "dependabot" or "dependabot[bot]" or
                "github-actions" or "github-actions[bot]" => false,

                _ => true
            };

            if (isValidActor is false)
            {
                core.WriteInfo($"Ignored as {context.Actor} triggered this...");

                return false;
            }

            _context = context;

            return true;
        }
        catch (Exception ex)
        {
            core.WriteError($"""
                Error attempting to get the context:
                  {ex}
                """);
        }
        finally
        {
            core.EndGroup();
        }

        return false;
    }

    private async ValueTask<HashSet<ProfaneSourceFilter>?> GetAdditionalFiltersAsync()
    {
        var manualProfaneWords = core.GetManualProfaneWords();
        var customProfaneWords = await core.GetCustomProfaneWordsAsync();

        if (manualProfaneWords is null && customProfaneWords is null)
        {
            return null;
        }

        var filters = new HashSet<ProfaneSourceFilter>();

        if (manualProfaneWords is { Length: > 0 })
        {
            filters.Add(new(
                SourceName: $"ManualProfaneWords.raw({manualProfaneWords.Length})",
                ProfaneWords: manualProfaneWords.ToFrozenSet()));
        }

        if (customProfaneWords is { Length: > 0 })
        {
            filters.Add(new(
                SourceName: $"CustomProfaneWords.url({customProfaneWords.Length})",
                ProfaneWords: customProfaneWords.ToFrozenSet()));
        }

        return filters;
    }

    private async ValueTask<FiltrationResult> ApplyProfanityFilterAsync(
        string title, string body, ReplacementStrategy replacementStrategy)
    {
        if (string.IsNullOrWhiteSpace(title) &&
            string.IsNullOrWhiteSpace(body))
        {
            return FiltrationResult.NotFiltered;
        }

        var additionalFilters = await GetAdditionalFiltersAsync();

        var titleResult = await TryApplyFilterAsync(
            title, parameters: new(replacementStrategy, FilterTarget.Title)
            {
                AdditionalFilterSources = additionalFilters
            });

        var bodyResult = await TryApplyFilterAsync(
            body, parameters: new(replacementStrategy, FilterTarget.Body)
            {
                AdditionalFilterSources = additionalFilters
            });

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

            core.WriteInfo($"""
                Original {type} text: {text}
                Filtered {type} text: {result.FinalOutput}
                """);
        }

        return result;
    }
}
