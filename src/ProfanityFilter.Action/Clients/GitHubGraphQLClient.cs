// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Clients;

internal sealed class GitHubGraphQLClient(string owner, string repo, string token)
{
    private const string ProductID = "ievangelist-profanity-filter";
    private const string ProductVersion = "1.0";

    private readonly GraphQLConnection _connection = new(
        new GraphQLProductHeaderValue(ProductID, ProductVersion), token);

    private readonly (string Owner, string Repo, string Token) _config = (owner, repo, token);

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

    //public async ValueTask<string> UpdateIssueAsync(int number, IssueUpdate input)
    //{
    //   var result = await _client.Issue.Update(_config.Owner, _config.Repo, number, input);
    //   return result.NodeId;
    //}

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

    //public async ValueTask<string> UpdatePullRequestAsync(int number, PullRequestUpdate input)
    //{
    //   var result = await _client.PullRequest.Update(_config.Owner, _config.Repo, number, input);
    //   return result.NodeId;
    //}

    public async ValueTask<GraphQLLabel?> GetLabelAsync()
    {
        var name = "profane content 🤬";

        var query =
            new Query()
                .Repository(_config.Repo, _config.Owner)
                .Label(name)
                .Select(label => label)
                .Compile();

        return await _connection.Run(query);
    }

    public async ValueTask<GraphQLLabel?> CreateLabelAsync(string clientId)
    {
        const string LabelName = "profane content 🤬";
        const string LabelDescription = "Either the title or body text contained profanity";
        const string LabelColor = "512bd4";

        var repositoryId = await _connection.Run(
            new Query()
                .Repository(_config.Repo, _config.Owner)
                .Select(repository => repository.Id)
                .Compile());

        var mutation = $$"""
            mutation  {
              createLabel(input: {
                clientMutationId: {{clientId}}
                color: "{{LabelColor}}"
                description: "{{LabelDescription}}"
                name: "{{LabelName}}"
                repositoryId: "{{repositoryId}}"
              }) {
              label {
                id
                name
                description
                color
              }
            }
            """;

        var json = await _connection.Run(mutation);

        var newLabel = JsonConvert.DeserializeObject<GraphQLLabel>(
            json,
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

        return newLabel;
    }

    public async ValueTask<GraphQLIssue?> GetIssueAsync(int issueNumber)
    {
       var query =
           new Query()
               .Repository(_config.Repo, _config.Owner)
               .Issue(issueNumber)
               .Select(issue => issue)
               .Compile();

       var issue = await _connection.Run(query);
       return issue;
    }

    public async ValueTask<GraphQLPullRequest?> GetPullRequestAsync(int pullRequestNumber)
    {
       var query =
           new Query()
               .Repository(_config.Repo, _config.Owner)
               .PullRequest(pullRequestNumber)
               .Select(pr => pr)
               .Compile();

       var pullRequest = await _connection.Run(query);
        return pullRequest;
    }
}
