// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddActionProcessorServices(
        this IServiceCollection services)
    {
        services.AddSingleton<ProfanityProcessor>();
        services.AddProfanityFilter();

        return services;
    }

    private static IServiceCollection AddProfanityFilter(
        this IServiceCollection services)
    {
        services.AddProfanityFilterServices();

        services.AddGitHubActionsCore();

        services.AddSingleton(static provider =>
        {
            var (owner, repo, token) = GetRepositoryConfiguration(provider);

            return new CustomGitHubClient(owner, repo, token);
        });

        return services;

        static RepoConfig GetRepositoryConfiguration(IServiceProvider provider)
        {
            var core = provider.GetRequiredService<ICoreService>();

            try
            {
                core.StartGroup("Initializing context");

                var context = provider.GetRequiredService<Context>();

                var repository = context.Repo;

                var (owner, repo) = (repository.Owner, repository.Repo);

                core.Info($"Repository: {owner}/{repo}");

                var token = core.GetInput("token");

                return (owner, repo, token);
            }
            finally
            {
                core.EndGroup();
            }
        }
    }
}
