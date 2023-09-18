// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

internal sealed class ActionProcessor(
    GitHubRestClient gitHubRestClient,
    GitHubGraphQLClient gitHubGraphQLClient,
    IProfaneContentCensorService profaneContentCensor,
    ICoreService core)
{
    public async Task ProcessAsync()
    {
        var success = true;

        try
        {
            if (TryGetContext(out var context) is false)
            {
                return;
            }

            var (isIssue, isPullRequest) =
                (context?.Payload?.Issue is not null, context?.Payload?.PullRequest is not null);

            Func<long, LabelModel?, Task> handler = (isIssue, isPullRequest) switch
            {
                (true, _) => HandleIssueAsync,
                (_, true) => HandlePullRequestAsync,

                _ => throw new Exception(
                    "The profanity filter GitHub Action only works with issues or pull requests.")
            };

            var label = await gitHubGraphQLClient.GetLabelAsync();
            if (label is null)
            {
                core.Warning("""
                    The expected label isn't present, a label with the following name is would have been applied if found.
                        'profane content 🤬'
                    """);
            }

            var number = (context?.Payload?.Issue ?? context?.Payload?.PullRequest)!.Number;

            await handler(number, label);
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

    private bool TryGetContext(out Context context)
    {
        context = Context.Current;
        core.Info(context.ToString() ?? "Unknown context");

        var isValidAction = context.Action switch
        {
            "profanity-filter" or "opened" or "edited" or "reopened" => true,

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

    private async Task HandleIssueAsync(long issueNumber, LabelModel? label)
    {
        var clientId = Guid.NewGuid().ToString();
        core.StartGroup($"Evaluating issue #{issueNumber} for profanity (Client mutation: {clientId})");

        try
        {
            var issue = await gitHubGraphQLClient.GetIssueAsync((int)issueNumber);
            if (issue is null)
            {
                core.Error($"Unable to get issue with number: {issueNumber}");
                return;
            }

            var replacementType = GetInputReplacementType();

            var filterResult = await ApplyProfanityFilterAsync(
                issue.Title, issue.Body, replacementType);

            if (filterResult.IsFiltered)
            {
                var issueUpdate = new IssueUpdate
                {
                    Body = filterResult.Body,
                    Title = filterResult.Title
                };

                if (label is not null)
                {
                    issueUpdate.AddLabel(label.Name);
                }

                await gitHubRestClient.UpdateIssueAsync(issue.Number, issueUpdate);

                await gitHubGraphQLClient.AddReactionAsync(
                    issue.Id.Value, ReactionContent.Confused, clientId);
            }
        }
        finally
        {
            core.EndGroup();
        }
    }

    private async Task HandlePullRequestAsync(long pullRequestNumber, LabelModel? label)
    {
        var clientId = Guid.NewGuid().ToString();
        core.StartGroup($"Evaluating pull request #{pullRequestNumber} for profanity (Client mutation: {clientId})");

        try
        {
            var pullRequest = await gitHubGraphQLClient.GetPullRequestAsync((int)pullRequestNumber);
            if (pullRequest is null)
            {
                core.Error($"Unable to get PR with number: {pullRequestNumber}");
                return;
            }

            var replacementType = GetInputReplacementType();

            var filterResult = await ApplyProfanityFilterAsync(
                pullRequest.Title, pullRequest.Body, replacementType);

            if (filterResult.IsFiltered)
            {
                var issueUpdate = new PullRequestUpdate
                {
                    Body = filterResult.Body,
                    Title = filterResult.Title
                };

                await gitHubRestClient.UpdatePullRequestAsync(
                    pullRequest.Number, issueUpdate, label?.Name);

                await gitHubGraphQLClient.AddReactionAsync(
                    pullRequest.Id.Value, ReactionContent.Confused, clientId);
            }
        }
        finally
        {
            core.EndGroup();
        }
    }

    private async ValueTask<FilterResult> ApplyProfanityFilterAsync(
        string title, string body, ReplacementType replacementType)
    {
        if (string.IsNullOrWhiteSpace(title) &&
            string.IsNullOrWhiteSpace(body))
        {
            return FilterResult.NotFiltered;
        }

        var (resultingTitle, isTitleFiltered) = await TryApplyFilterAsync(title, replacementType);
        var (resultingBody, isBodyFiltered) = await TryApplyFilterAsync(body, replacementType);

        return new FilterResult(
            resultingTitle,
            isTitleFiltered,
            resultingBody,
            isBodyFiltered);
    }

    private async ValueTask<(string text, bool isFiltered)> TryApplyFilterAsync(
        string text, ReplacementType replacementType)
    {
        var filterText = await profaneContentCensor.ContainsProfanityAsync(text);
        var resultingText =
            filterText
                ? await profaneContentCensor.CensorProfanityAsync(text, replacementType)
                : text;

        if (filterText)
        {
            core.Info($"""
                Original text: {text}
                Filtered text: {resultingText}
                """);
        }

        return (resultingText, filterText);
    }

    private ReplacementType GetInputReplacementType() =>
        core.GetInput("replacement-type") is string value &&
        Enum.TryParse<ReplacementType>(value, out var type)
            ? type
            : ReplacementType.Asterisk;
}

internal readonly record struct FilterResult(
    string Title,
    bool IsTitleFiltered,
    string Body,
    bool IsBodyFiltered)
{
    internal static FilterResult NotFiltered { get; } =
        new(string.Empty, false, string.Empty, false);

    internal bool IsFiltered => IsTitleFiltered || IsBodyFiltered;
}
