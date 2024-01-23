// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Clients;

internal sealed class CustomGitHubClient(
    GitHubClient client,
    string owner,
    string repo)
{
    public Task<Reaction?> AddReactionAsync(long issueOrPullRequestId, ReactionContent reaction)
    {
        return client.Repos[owner][repo].Issues[(int)issueOrPullRequestId].Reactions.PostAsync(new()
        {
            Content = reaction
        });
    }

    public Task<Label?> CreateLabelAsync()
    {
        return client.Repos[owner][repo].Labels.PostAsync(new()
        {
            Color = DefaultLabel.Color,
            Name = DefaultLabel.Name,
            Description = DefaultLabel.Description
        });
    }

    public Task<Issue?> GetIssueAsync(int issueNumber)
    {
        return client.Repos[owner][repo].Issues[issueNumber].GetAsync();
    }

    public Task<IssueComment?> GetIssueCommentAsync(long issueCommentId)
    {
        return client.Repos[owner][repo].Issues.Comments[(int)issueCommentId].GetAsync();
    }

    public Task<List<Label>?> GetIssueLabelsAsync(int issueNumber)
    {
        return client.Repos[owner][repo].Issues[issueNumber].Labels.GetAsync();
    }

    public async Task<Label?> GetLabelAsync()
    {
        var labels = await client.Repos[owner][repo].Labels.GetAsync();

        return labels?.FirstOrDefault(label => label.Name == DefaultLabel.Name);
    }

    public Task<PullRequest?> GetPullRequestAsync(int pullRequestNumber)
    {
        return client.Repos[owner][repo].Pulls[pullRequestNumber].GetAsync();
    }

    public Task<List<Label>?> GetPullRequestLabelsAsync(int pullRequestNumber)
    {
        return client.Repos[owner][repo].Issues[pullRequestNumber].Labels.GetAsync();
    }

    public Task UpdateIssueAsync(int number, IssueUpdate body)
    {
        return client.Repos[owner][repo].Issues[number].PatchAsync(body);
    }

    public Task UpdateIssueCommentAsync(long issueCommentId, string updatedComment)
    {
        return client.Repos[owner][repo].Issues.Comments[(int)issueCommentId].PatchAsync(new()
        {
            Body = updatedComment
        });
    }

    public async Task UpdatePullRequestAsync(int number, PullRequestUpdate body, string? label)
    {
        await client.Repos[owner][repo].Pulls[number].PatchAsync(body);

        if (label is not null)
        {
            await client.Repos[owner][repo].Issues[number].Labels.PutAsync(new()
            {
                LabelsPutRequestBodyString = label,
            });
        }
    }
}
