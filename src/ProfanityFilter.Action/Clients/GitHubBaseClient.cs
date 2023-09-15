// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Clients;

internal class GitHubBaseClient(string owner, string repo, string token)
{
    protected readonly RepoConfig _config = (owner, repo, token);

    protected const string ProductID = "ievangelist-profanity-filter";
    protected const string ProductVersion = "1.0";
}
