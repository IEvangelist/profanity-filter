// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Octokit.GraphQL.Model;
using Octokit.GraphQL;
using Octokit;

using IGraphQLConnection = Octokit.GraphQL.IConnection;
using GraphQLConnection = Octokit.GraphQL.Connection;
using GraphQLProductHeaderValue = Octokit.GraphQL.ProductHeaderValue;

using Connection = Octokit.Connection;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace ProfanityFilter.Action.Clients;

internal sealed class GitHubGraphQLClient
{
    const string ProductID = "GitHub Action: Profanity filter";
    const string ProductVersion = "1.0";

    readonly IGraphQLConnection _connection;
    readonly IGitHubClient _client;
    
    readonly (string Owner, string Repo, string Token) _config;

    public GitHubGraphQLClient(string owner, string repo, string token)
    {
        _config = (owner, repo, token);
        _connection = new GraphQLConnection(new GraphQLProductHeaderValue(ProductID, ProductVersion), token);
        _client = new GitHubClient(new Connection(new ProductHeaderValue(ProductID, ProductVersion))
        {
           Credentials = new Credentials(token)
        });
    }

    public async ValueTask<string> AddLabelAsync(string issueOrPullRequestId, string[] labelIds, string clientId)
    {
       var mutation =
           new Mutation()
               .AddLabelsToLabelable(new AddLabelsToLabelableInput
               {
                   ClientMutationId = clientId,
                   LabelableId = issueOrPullRequestId.ToGitHubId(),
                   LabelIds = labelIds.Select(id => id.ToGitHubId()).ToArray()
               })
               .Select(payload => new
               {
                   payload.ClientMutationId
               })
               .Compile();

       var result = await _connection.Run(mutation);
       return result.ClientMutationId;
    }

    public async ValueTask<string> AddReactionAsync(string issueOrPullRequestId, ReactionContent reaction, string clientId)
    {
       var mutation =
           new Mutation()
               .AddReaction(new AddReactionInput
               {
                   ClientMutationId = clientId,
                   SubjectId = issueOrPullRequestId.ToGitHubId(),
                   Content = reaction
               })
               .Select(payload => new
               {
                   payload.ClientMutationId
               })
               .Compile();

       var result = await _connection.Run(mutation);
       return result.ClientMutationId;
    }

    public async ValueTask<string> RemoveLabelAsync(string issueOrPullRequestId, string clientId)
    {
       var mutation =
           new Mutation()
               .ClearLabelsFromLabelable(new ClearLabelsFromLabelableInput
               {
                   ClientMutationId = clientId,
                   LabelableId = issueOrPullRequestId.ToGitHubId()
               })
               .Select(payload => new
               {
                   payload.ClientMutationId
               })
               .Compile();

       var result = await _connection.Run(mutation);
       return result.ClientMutationId;
    }

    public async ValueTask<string> RemoveReactionAsync(string issueOrPullRequestId, ReactionContent reaction, string clientId)
    {
       var mutation =
          new Mutation()
              .RemoveReaction(new RemoveReactionInput
              {
                  ClientMutationId = clientId,
                  SubjectId = issueOrPullRequestId.ToGitHubId(),
                  Content = reaction
              })
              .Select(payload => new
              {
                  payload.ClientMutationId
              })
              .Compile();

       var result = await _connection.Run(mutation);
       return result.ClientMutationId;
    }

    public async ValueTask<string> UpdateIssueAsync(UpdateIssueInput input)
    {
       var mutation =
           new Mutation()
               .UpdateIssue(input)
               .Select(payload => new
               {
                   payload.ClientMutationId
               })
               .Compile();

       var result = await _connection.Run(mutation);
       return result.ClientMutationId;
    }

    public async ValueTask<string> UpdateIssueAsync(int number, IssueUpdate input)
    {
       var result = await _client.Issue.Update(_config.Owner, _config.Repo, number, input);
       return result.NodeId;
    }

    public async ValueTask<string> UpdatePullRequestAsync(UpdatePullRequestInput input)
    {
       var mutation =
           new Mutation()
               .UpdatePullRequest(input)
               .Select(payload => new
               {
                   payload.ClientMutationId
               })
               .Compile();

       var result = await _connection.Run(mutation);
       return result.ClientMutationId;
    }

    public async ValueTask<string> UpdatePullRequestAsync(int number, PullRequestUpdate input)
    {
       var result = await _client.PullRequest.Update(_config.Owner, _config.Repo, number, input);
       return result.NodeId;
    }

    public async ValueTask<(string title, string body)> GetIssueTitleAndBodyAsync(int issueNumber)
    {
       var query =
           new Query()
               .Repository(_config.Repo, _config.Owner)
               .Issue(issueNumber)
               .Select(issue => new
               {
                   issue.Title,
                   issue.Body
               })
               .Compile();

       var result = await _connection.Run(query);
       return (result.Title, result.Body);
    }

    public async ValueTask<(string title, string body)> GetPullRequestTitleAndBodyAsync(int pullRequestNumber)
    {
       var query =
           new Query()
               .Repository(_config.Repo, _config.Owner)
               .PullRequest(pullRequestNumber)
               .Select(pullRequest => new
               {
                   pullRequest.Title,
                   pullRequest.Body
               })
               .Compile();

       var result = await _connection.Run(query);
       return (result.Title, result.Body);
    }
}
