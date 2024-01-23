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
        services.AddOctokitServices();

        return services;
    }

    private static IServiceCollection AddProfanityFilter(
        this IServiceCollection services)
    {
        services.AddSingleton<ActionProcessor>();

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

                var client = provider.GetRequiredService<GitHubClient>();
                var context = provider.GetRequiredService<Context>();

            return (repository.Owner, repository.Repo, token);
        }
    }
}
