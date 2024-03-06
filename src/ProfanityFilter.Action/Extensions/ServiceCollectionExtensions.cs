// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddActionProcessorServices(this IServiceCollection services)
    {
        services.AddSingleton<ProfanityProcessor>();

        services.AddProfanityFilterServices();

        services.AddGitHubActionsCore();

        var inputToken = Env.GetEnvironmentVariable("INPUT_TOKEN");

        ArgumentException.ThrowIfNullOrWhiteSpace(inputToken);

        services.AddGitHubClientServices(inputToken);

        services.AddSingleton<ICustomGitHubClient>(static provider =>
        {
            var core = provider.GetRequiredService<ICoreService>();
            var client = provider.GetRequiredService<GitHubClient>();
            var context = provider.GetRequiredService<Context>();

            var repository = context.Repo;

            var (owner, repo) = (repository.Owner, repository.Repo);

            core.WriteInfo($"Repository: {owner}/{repo}");

            return new CustomGitHubClient(core, client, owner, repo);
            
        });

        return services;
    }
}
