// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Clients;

internal interface ICustomGitHubClient
{
    Task<Reaction?> AddReactionAsync(long issueNumber, ReactionContent reaction);
    Task<Label?> CreateLabelAsync();
    Task<Issue?> GetIssueAsync(int issueNumber);
    Task<IssueComment?> GetIssueCommentAsync(long issueCommentId);
    Task<List<Label>?> GetIssueLabelsAsync(int issueNumber);
    Task<Label?> GetLabelAsync();
    Task<PullRequest?> GetPullRequestAsync(int pullRequestNumber);
    Task<List<Label>?> GetPullRequestLabelsAsync(int pullRequestNumber);
    Task UpdateIssueAsync(int number, IssueUpdate body);
    Task UpdateIssueCommentAsync(long issueCommentId, string updatedComment);
    Task UpdatePullRequestAsync(int number, PullRequestUpdate body, string? label);
}