// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Clients;

internal sealed class CustomGitHubClient(
    ICoreService core,
    GitHubClient client,
    string owner,
    string repo) : ICustomGitHubClient
{
    public Task<Reaction?> AddReactionAsync(long issueNumber, ReactionContent reaction)
    {
        return TryClientRequestAsync(
            () => client.Repos[owner][repo].Issues[(int)issueNumber].Reactions.PostAsync(new()
            {
                Content = reaction
            }));
    }

    public Task<Label?> CreateLabelAsync()
    {
        return TryClientRequestAsync(
            () => client.Repos[owner][repo].Labels.PostAsync(new()
            {
                Color = DefaultLabel.Color,
                Name = DefaultLabel.Name,
                Description = DefaultLabel.Description
            }));
    }

    public Task<Issue?> GetIssueAsync(int issueNumber)
    {
        return TryClientRequestAsync(
            () => client.Repos[owner][repo].Issues[issueNumber].GetAsync());
    }

    public Task<IssueComment?> GetIssueCommentAsync(long issueCommentId)
    {
        return TryClientRequestAsync(
            () => client.Repos[owner][repo].Issues.Comments[(int)issueCommentId].GetAsync());
    }

    public Task<List<Label>?> GetIssueLabelsAsync(int issueNumber)
    {
        return TryClientRequestAsync(
            () => client.Repos[owner][repo].Issues[issueNumber].Labels.GetAsync());
    }

    public Task<Label?> GetLabelAsync()
    {
        return TryClientRequestAsync(
            async () =>
            {
                var label = await GetLabelByNameAsync(DefaultLabel.Name);

                if (label is null)
                {
                    core.WriteWarning($"Unable to get label for {owner}/{repo}");
                }

                return label;

                async Task<Label?> GetLabelByNameAsync(string labelName)
                {
                    Label? label = null;

                    const int PageCount = 100;
                    var page = 1;

                    while (label is null)
                    {
                        var labels = await client.Repos[owner][repo].Labels.GetAsync(
                            parameters =>
                            {
                                parameters.QueryParameters.Page = page;
                                parameters.QueryParameters.PerPage = PageCount;
                            });

                        if (labels is null or { Count: 0 })
                        {
                            break;
                        }

                        label = labels?.FirstOrDefault(l => l.Name == labelName);
                        page++;
                    }

                    return label;
                }
            });
    }

    public Task<PullRequest?> GetPullRequestAsync(int pullRequestNumber)
    {
        return TryClientRequestAsync(
            () => client.Repos[owner][repo].Pulls[pullRequestNumber].GetAsync());
    }

    public Task<List<Label>?> GetPullRequestLabelsAsync(int pullRequestNumber)
    {
        return TryClientRequestAsync(
            () => client.Repos[owner][repo].Issues[pullRequestNumber].Labels.GetAsync());
    }

    public Task UpdateIssueAsync(int number, IssueUpdate body)
    {
        return TryClientRequestAsync(
            () => client.Repos[owner][repo].Issues[number].PatchAsync(body));
    }

    public Task UpdateIssueCommentAsync(long issueCommentId, string updatedComment)
    {
        return TryClientRequestAsync(
            () => client.Repos[owner][repo].Issues.Comments[(int)issueCommentId].PatchAsync(new()
            {
                Body = updatedComment
            }));
    }

    public async Task UpdatePullRequestAsync(int number, PullRequestUpdate body, string? label)
    {
        await TryClientRequestAsync(
            async () =>
            {
                await client.Repos[owner][repo].Pulls[number].PatchAsync(body);

                if (label is not null)
                {
                    await client.Repos[owner][repo].Issues[number].Labels.PutAsync(new()
                    {
                        LabelsPutRequestBodyMember1 = new()
                        {
                            Labels = [label]
                        },
                    });
                }
            });
    }

    private async Task<T?> TryClientRequestAsync<T>(Func<Task<T?>> requestAsync)
    {
        try
        {
            return await requestAsync();
        }
        catch (Exception ex)
        {
            // Only warn, as we don't want to fail the entire action
            core.WriteWarning(ex.ToString());
        }

        return default;
    }

    private async Task TryClientRequestAsync(Func<Task> requestAsync)
    {
        try
        {
            await requestAsync();
        }
        catch (Exception ex)
        {
            // Only warn, as we don't want to fail the entire action
            core.WriteWarning(ex.ToString());
        }
    }
}
