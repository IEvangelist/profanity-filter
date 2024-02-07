// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using GitHub.Models;
using GitHub.Repos.Item.Item.Issues.Item;
using GitHub.Repos.Item.Item.Issues.Item.Reactions;
using GitHub.Repos.Item.Item.Pulls.Item;

namespace ProfanityFilter.Action.Tests;

internal sealed class TestCustomGitHubClient : ICustomGitHubClient
{
    public Task<Reaction?> AddReactionAsync(long issueNumber, ReactionsPostRequestBody_content reaction)
    {
        throw new NotImplementedException();
    }

    public Task<Label?> CreateLabelAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Issue?> GetIssueAsync(int issueNumber)
    {
        throw new NotImplementedException();
    }

    public Task<IssueComment?> GetIssueCommentAsync(long issueCommentId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Label>?> GetIssueLabelsAsync(int issueNumber)
    {
        throw new NotImplementedException();
    }

    public Task<Label?> GetLabelAsync()
    {
        throw new NotImplementedException();
    }

    public Task<PullRequest?> GetPullRequestAsync(int pullRequestNumber)
    {
        throw new NotImplementedException();
    }

    public Task<List<Label>?> GetPullRequestLabelsAsync(int pullRequestNumber)
    {
        throw new NotImplementedException();
    }

    public Task UpdateIssueAsync(int number, WithIssue_numberPatchRequestBody body)
    {
        throw new NotImplementedException();
    }

    public Task UpdateIssueCommentAsync(long issueCommentId, string updatedComment)
    {
        throw new NotImplementedException();
    }

    public Task UpdatePullRequestAsync(int number, WithPull_numberPatchRequestBody body, string? label)
    {
        throw new NotImplementedException();
    }
}
