// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Clients;

internal sealed class GitHubRestClient(string owner, string repo, string token)
    : GitHubBaseClient(owner, repo, token)
{
    private readonly GitHubClient _client =
        new(
            connection: new RestConnection(
                productInformation: new RestProductHeaderValue(
                    name: ProductID, version: ProductVersion))
            {
                Credentials = new Credentials(token)
            });

    public async ValueTask UpdateIssueAsync(int number, IssueUpdate input) =>
        await _client.Issue.Update(_config.Owner, _config.Repo, number, input);

    public async ValueTask UpdatePullRequestAsync(int number, PullRequestUpdate input, string? label)
    {
        await _client.PullRequest.Update(_config.Owner, _config.Repo, number, input);

        if (label is not null)
        {
            // Add a label to the pull request
            await _client.Issue.Labels.AddToIssue(
                _config.Owner, _config.Repo, number, [label]);
        }
    }

    public async ValueTask<RestIssueComment> GetIssueCommentAsync(long issueCommentId) =>
        await _client.Issue.Comment.Get(_config.Owner, _config.Repo, (int)issueCommentId);

    public async ValueTask UpdateIssueCommentAsync(long issueCommentId, string updatedComment)
    {
        await _client.Issue.Comment.Update(
            _config.Owner, _config.Repo, (int)issueCommentId, updatedComment);

        // Add a reaction to the issue comment
        await _client.Reaction.IssueComment.Create(
            _config.Owner, _config.Repo, (int)issueCommentId, new NewReaction(ReactionType.Confused));
    }
}
