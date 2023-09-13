// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action;

internal sealed class ActionProcessor(
    GitHubGraphQLClient gitHubGraphQLClient,
    IProfaneContentCensorService profaneContentCensor,
    ICoreService core)
{
    public async Task ProcessAsync()
    {
        try
        {
            // 
        }
        catch (Exception ex)
        {
            core.Error(ex.Message);
        }
    }
}
