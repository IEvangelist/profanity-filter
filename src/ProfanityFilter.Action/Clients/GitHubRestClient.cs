// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Clients;

internal sealed class GitHubRestClient(GitHubClient client, RepoConfig config)
{
    public async ValueTask<Issue> GetIssueAsync(int issueNumber)
    {
        var issue = await client.Issue.Get(
            config.Owner, config.Repo, issueNumber);

        return issue;
    }

    public async ValueTask UpdateIssueAsync(int number, IssueUpdate input)
    {
        await client.Issue.Update(
            config.Owner, config.Repo, number, input);

        await client.Reaction.Issue.Create(
            config.Owner, config.Repo, number, new NewReaction(ReactionType.Confused));
    }

    public async ValueTask<PullRequest> GetPullRequestAsync(int pullRequestNumber)
    {
        var pullRequest = await client.PullRequest.Get(
            config.Owner, config.Repo, pullRequestNumber);

        return pullRequest;
    }

    public async ValueTask UpdatePullRequestAsync(int number, PullRequestUpdate input, string? label)
    {
        await client.PullRequest.Update(
            config.Owner, config.Repo, number, input);

        if (label is not null)
        {
            // Add a existingLabel to the pull request
            await client.Issue.Labels.AddToIssue(
                config.Owner, config.Repo, number, [label]);
        }

        await client.Reaction.Issue.Create(
            config.Owner, config.Repo, number, new NewReaction(ReactionType.Confused));
    }

    public async ValueTask<RestIssueComment> GetIssueCommentAsync(long issueCommentId) =>
        await client.Issue.Comment.Get(config.Owner, config.Repo, (int)issueCommentId);

    public async ValueTask UpdateIssueCommentAsync(long issueCommentId, string updatedComment)
    {
        await client.Issue.Comment.Update(
            config.Owner, config.Repo, (int)issueCommentId, updatedComment);

        // Add a reaction to the issue comment
        await client.Reaction.IssueComment.Create(
            config.Owner, config.Repo, (int)issueCommentId, new NewReaction(ReactionType.Confused));
    }

    public async ValueTask<IReadOnlyList<Label>> GetIssueLabelsAsync(int issueNumber)
    {
        var labels = await client.Issue.Labels.GetAllForIssue(
            config.Owner, config.Repo, issueNumber);

        return labels;
    }

    public async ValueTask<string> GetOrCreateDefaultLabelAsync()
    {
        try
        {
            var labels = await client.Issue.Labels.GetAllForRepository(
                config.Owner, config.Repo);

            foreach (var existingLabel in labels ?? [])
            {
                if (existingLabel is { Name: DefaultLabel.Name, Color: DefaultLabel.Color })
                {
                    return existingLabel.Name;
                }
            }

            var label = await client.Issue.Labels.Create(
                config.Owner, config.Repo, new(DefaultLabel.Name, DefaultLabel.Color)
                {
                    Description = DefaultLabel.Description
                });

            return label.Name;
        }
        catch
        {
            return DefaultLabel.Name;
        }
    }
}
