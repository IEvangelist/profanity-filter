// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Clients;

internal sealed class GitHubRestClient(GitHubClient client, ICoreService core, RepoConfig config)
{
    public async ValueTask<Issue> GetIssueAsync(int issueNumber)
    {
        try
        {
            core.Info(
                $"Attempting to get issue #{issueNumber} in {config.Owner}/{config.Repo}.");

            var issue = await client.Issue.Get(
                config.Owner, config.Repo, issueNumber);

            return issue;
        }
        catch (Exception ex)
        {
            core.Error(ex.ToString());

            throw;
        }
    }

    public async ValueTask UpdateIssueAsync(int number, IssueUpdate input)
    {
        try
        {
            core.Info(
                $"Attempting to update issue #{number} in {config.Owner}/{config.Repo}.");

            await client.Issue.Update(
                config.Owner, config.Repo, number, input);

            await client.Reaction.Issue.Create(
                config.Owner, config.Repo, number, new NewReaction(ReactionType.Confused));
        }
        catch (Exception ex)
        {
            core.Error(ex.ToString());

            throw;
        }
    }

    public async ValueTask<PullRequest> GetPullRequestAsync(int pullRequestNumber)
    {
        try
        {
            core.Info(
                $"Attempting to get pull request #{pullRequestNumber} in {config.Owner}/{config.Repo}.");

            var pullRequest = await client.PullRequest.Get(
                config.Owner, config.Repo, pullRequestNumber);

            return pullRequest;
        }
        catch (Exception ex)
        {
            core.Error(ex.ToString());

            throw;
        }
    }

    public async ValueTask UpdatePullRequestAsync(int number, PullRequestUpdate input, string? label)
    {
        try
        {
            core.Info(
                $"Attempting to update pull request #{number} in {config.Owner}/{config.Repo} with label: {label}.");

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
        catch (Exception ex)
        {
            core.Error(ex.ToString());

            throw;
        }
    }

    public async ValueTask<RestIssueComment> GetIssueCommentAsync(long issueCommentId)
    {
        try
        {
            core.Info(
                $"Attempting to get issue comment id {issueCommentId} in {config.Owner}/{config.Repo}.");

            return await client.Issue.Comment.Get(
                config.Owner, config.Repo, (int)issueCommentId);
        }
        catch (Exception ex)
        {
            core.Error(ex.ToString());

            throw;
        }
    }

    public async ValueTask UpdateIssueCommentAsync(long issueCommentId, string updatedComment)
    {
        try
        {
            core.Info(
                $"Attempting to update issue comment id {issueCommentId} in {config.Owner}/{config.Repo}.");

            await client.Issue.Comment.Update(
                config.Owner, config.Repo, (int)issueCommentId, updatedComment);

            // Add a reaction to the issue comment
            await client.Reaction.IssueComment.Create(
                config.Owner, config.Repo, (int)issueCommentId, new NewReaction(ReactionType.Confused));
        }
        catch (Exception ex)
        {
            core.Error(ex.ToString());

            throw;
        }
    }

    public async ValueTask<string> GetOrCreateDefaultLabelAsync()
    {
        try
        {
            core.StartGroup(
                $"Attempting to get all labels in {config.Owner}/{config.Repo}.");

            var labels = await client.Issue.Labels.GetAllForRepository(
                config.Owner, config.Repo);

            foreach (var existingLabel in labels ?? [])
            {
                core.Info($"{existingLabel.Name} {existingLabel.Color}");

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
        catch (Exception ex)
        {
            core.Error(ex.ToString());

            return DefaultLabel.Name;
        }
        finally
        {
            core.EndGroup();
        }
    }
}
