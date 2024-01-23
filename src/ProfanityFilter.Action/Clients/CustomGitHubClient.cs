// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Clients;

internal sealed class CustomGitHubClient(
    string owner,
    string repo,
    string token)
{
    private readonly GitHubClient _client = GitHubClientFactory.Create(token);

    public Task<Reaction?> AddReactionAsync(long issueOrPullRequestId, ReactionContent reaction)
    {
        return _client.Repos[owner][repo].Issues[(int)issueOrPullRequestId].Reactions.PostAsync(new()
        {
            Content = reaction
        });
    }

    public Task<Label?> CreateLabelAsync()
    {
        return _client.Repos[owner][repo].Labels.PostAsync(new()
        {
            Color = DefaultLabel.Color,
            Name = DefaultLabel.Name,
            Description = DefaultLabel.Description
        });
    }

    public Task<Issue?> GetIssueAsync(int issueNumber)
    {
        return _client.Repos[owner][repo].Issues[issueNumber].GetAsync();
    }

    public Task<IssueComment?> GetIssueCommentAsync(long issueCommentId)
    {
        return _client.Repos[owner][repo].Issues.Comments[(int)issueCommentId].GetAsync();
    }

    public Task<List<Label>?> GetIssueLabelsAsync(int issueNumber)
    {
        return _client.Repos[owner][repo].Issues[issueNumber].Labels.GetAsync();
    }

    public async Task<Label?> GetLabelAsync()
    {
        var labels = await _client.Repos[owner][repo].Labels.GetAsync();

        return labels?.FirstOrDefault(label => label.Name == DefaultLabel.Name);
    }

    public Task<PullRequest?> GetPullRequestAsync(int pullRequestNumber)
    {
        return _client.Repos[owner][repo].Pulls[pullRequestNumber].GetAsync();
    }

    public Task<List<Label>?> GetPullRequestLabelsAsync(int pullRequestNumber)
    {
        return _client.Repos[owner][repo].Issues[pullRequestNumber].Labels.GetAsync();
    }

    public Task UpdateIssueAsync(int number, IssueUpdate body)
    {
        return _client.Repos[owner][repo].Issues[number].PatchAsync(body);
    }

    public Task UpdateIssueCommentAsync(long issueCommentId, string updatedComment)
    {
        return _client.Repos[owner][repo].Issues.Comments[(int)issueCommentId].PatchAsync(new()
        {
            Body = updatedComment
        });
    }

    public async Task UpdatePullRequestAsync(int number, PullRequestUpdate body, string? label)
    {
        await _client.Repos[owner][repo].Pulls[number].PatchAsync(body);

        if (label is not null)
        {
            await _client.Repos[owner][repo].Issues[number].Labels.PutAsync(new()
            {
                LabelsPutRequestBodyString = label,
            });
        }
    }
}
