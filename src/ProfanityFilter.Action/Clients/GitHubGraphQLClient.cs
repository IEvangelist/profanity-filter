// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Clients;

internal sealed class GitHubGraphQLClient(string owner, string repo, string token)
    : GitHubBaseClient(owner, repo, token)
{
    private readonly GraphQLConnection _connection = new(
        new GraphQLProductHeaderValue(ProductID, ProductVersion), token);

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

    private async ValueTask<List<LabelModel>> GetIssueLabelsAsync(int issueNumber)
    {
        var query = new Query()
            .Repository(_config.Repo, _config.Owner)
            .Issue(issueNumber)
            .Labels(10, null, null, null, null)
            .Nodes
            .Select(label => new
            {
                label.Id,
                label.Name
            })
            .Compile();

        var issue = await _connection.Run(query);
        return issue?.Select(label => new LabelModel(label.Name, label.Id))
               ?.ToList() ?? new();
    }

    private async ValueTask<List<LabelModel>> GetPullRequestLabelsAsync(int pullRequestNumber)
    {
        var query = new Query()
            .Repository(_config.Repo, _config.Owner)
            .PullRequest(pullRequestNumber)
            .Labels(10, null, null, null, null)
            .Nodes
            .Select(label => new
            {
                label.Id,
                label.Name
            })
            .Compile();

        var issue = await _connection.Run(query);
        return issue?.Select(label => new LabelModel(label.Name, label.Id))
               ?.ToList() ?? new();
    }

    public async ValueTask<LabelModel?> GetLabelAsync()
    {
        var name = "profane content 🤬";

        var query =
            new Query()
                .Repository(_config.Repo, _config.Owner)
                .Label(name)
                .Select(label => new
                {
                    label.Name,
                    label.Color,
                    label.Id,
                    label.Description
                })
                .Compile();

        var label = await _connection.Run(query);
        return label is null
            ? null
            : new LabelModel(label.Name, label.Id, label.Color, label.Description);
    }

    public async ValueTask<GraphQLLabel?> CreateLabelAsync(string clientId)
    {
        var repositoryId = await _connection.Run(
            new Query()
                .Repository(_config.Repo, _config.Owner)
                .Select(repository => repository.Id)
                .Compile());

        var mutation = $$"""
            mutation  {
              createLabel(input: {
                clientMutationId: {{clientId}}
                color: "{{DefaultLabel.Color}}"
                description: "{{DefaultLabel.Description}}"
                name: "{{DefaultLabel.Name}}"
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

    public async ValueTask<IssueOrPullRequestModel?> GetIssueAsync(int issueNumber)
    {
        var query =
            new Query()
                .Repository(_config.Repo, _config.Owner)
                .Issue(issueNumber)
                .Select(issue => new
                {
                    issue.Id,
                    issue.Title,
                    issue.Body,
                    issue.Number
                })
                .Compile();

        var labels = await GetIssueLabelsAsync(issueNumber);

        var issue = await _connection.Run(query);
        return issue is null
            ? null
            : new IssueOrPullRequestModel
            {
                Id = issue.Id,
                Title = issue.Title,
                Body = issue.Body,
                Number = issue.Number,
                Labels = labels
            };
    }

    public async ValueTask<IssueOrPullRequestModel?> GetPullRequestAsync(int pullRequestNumber)
    {
        var query =
            new Query()
                .Repository(_config.Repo, _config.Owner)
                .PullRequest(pullRequestNumber)
                .Select(pullRequest => new
                {
                    pullRequest.Id,
                    pullRequest.Title,
                    pullRequest.Body,
                    pullRequest.Number,
                    pullRequest.BaseRefName
                })
                .Compile();

        var labels = await GetPullRequestLabelsAsync(pullRequestNumber);

        var pullRequest = await _connection.Run(query);
        return pullRequest is null
            ? null
            : new IssueOrPullRequestModel
            {
                Id = pullRequest.Id,
                Title = pullRequest.Title,
                Body = pullRequest.Body,
                Number = pullRequest.Number,
                Labels = labels
            };
    }
}
