// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

internal sealed class ActionProcessor(
    GitHubGraphQLClient gitHubGraphQLClient,
    IProfaneContentCensorService profaneContentCensor,
    ICoreService core)
{
    public async Task ProcessAsync()
    {
        try
        {
            var context = Context.Current;
            var runFilter = context.Action switch
            {
                "profanity-filter" or "opened" or "edited" or "reopened" => true,

                _ => false
            };

            if (runFilter is false)
            {
                core.Info(context.ToString() ?? "Unknown context");
                core.Warning($"The action '{context.Action}' is not supported.");

                return;
            }

            var (isIssue, isPullRequest) =
                (context?.Payload?.Issue is not null, context?.Payload?.PullRequest is not null);

            Func<long, ID, Task> handler = (isIssue, isPullRequest) switch
            {
                (true, _) => HandleIssueAsync,
                (_, true) => HandlePullRequestAsync,

                _ => throw new Exception(
                    "The profanity filter GitHub Action only works with issues or pull requests.")
            };

            var label = await gitHubGraphQLClient.GetLabelAsync();
            if (label is null)
            {
                core.Error("""
                    The required label isn't present, a label with the following name is required to work.
                        'profane content 🤬'
                    """);

                return;
            }

            var number = (context?.Payload?.Issue ?? context?.Payload?.PullRequest)!.Number;

            await handler(number, label.Id);
        }
        catch (Exception ex)
        {
            core.SetFailed(ex.ToString());
            Env.Exit(Env.ExitCode);
        }
    }

    private async Task HandleIssueAsync(long issueNumber, ID labelId)
    {
        var clientId = Guid.NewGuid().ToString();
        core.StartGroup($"Evaluating issue for profanity: {issueNumber} (Client mutation: {clientId})");

        try
        {
            var issue = await gitHubGraphQLClient.GetIssueAsync((int)issueNumber);
            if (issue is null)
            {
                core.Error($"Unable to get issue with number: {issueNumber}");
                return;
            }

            if (issue.Editor.Login is
                    "dependabot" or
                    "github-actions" or
                    "github-actions[bot]" or
                    "bot")
            {
                core.Info($"Ignoring PR #{issueNumber}, as it was edited by a bot.");
                return;
            }

            var replacementType = GetInputReplacementType();

            var filterResult = await ApplyProfanityFilterAsync(
                issue.Title, issue.Body, replacementType);

            if (filterResult.IsFiltered)
            {
                var existingLabels = issue.Labels()?.Nodes?.Select(l => l.Id).ToList();

                _ = await gitHubGraphQLClient.UpdateIssueAsync(new()
                {
                    Id = issue.Id,
                    Title = filterResult.Title,
                    Body = filterResult.Body,
                    ClientMutationId = clientId,
                    LabelIds = new[] { labelId }.Concat(existingLabels ?? new())
                });

                await gitHubGraphQLClient.AddReactionAsync(
                    issue.Id.Value, ReactionContent.Confused, clientId);
            }
        }
        finally
        {
            core.EndGroup();
        }
    }

    private async Task HandlePullRequestAsync(long pullRequestNumber, ID labelId)
    {
        var clientId = Guid.NewGuid().ToString();
        core.StartGroup($"Evaluating pull request for profanity: {pullRequestNumber} (Client mutation: {clientId})");

        try
        {
            var pullRequest = await gitHubGraphQLClient.GetPullRequestAsync((int)pullRequestNumber);
            if (pullRequest is null)
            {
                core.Error($"Unable to get PR with number: {pullRequestNumber}");
                return;
            }

            if (pullRequest.Editor.Login is
                    "dependabot" or
                    "github-actions" or
                    "github-actions[bot]" or
                    "bot")
            {
                core.Info($"Ignoring PR #{pullRequestNumber}, as it was edited by a bot.");
                return;
            }

            var replacementType = GetInputReplacementType();

            var filterResult = await ApplyProfanityFilterAsync(
                pullRequest.Title, pullRequest.Body, replacementType);

            if (filterResult.IsFiltered)
            {
                var existingLabels = pullRequest.Labels()?.Nodes?.Select(l => l.Id).ToList();

                _ = await gitHubGraphQLClient.UpdatePullRequestAsync(new()
                {
                    PullRequestId = pullRequest.Id,
                    BaseRefName = pullRequest.BaseRefName,
                    Title = filterResult.Title,
                    Body = filterResult.Body,
                    ClientMutationId = clientId,
                    LabelIds = new[] { labelId }.Concat(existingLabels ?? new())
                });

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
            core.Info($"Replaced text: {resultingText}");
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
