// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddActionProcessorServices(
        this IServiceCollection services) =>
        services.AddSingleton<ActionProcessor>()
            .AddProfanityFilter()
            .AddOctokitServices();

    private static IServiceCollection AddProfanityFilter(
        this IServiceCollection services)
    {
        services.AddProfanityFilterServices();

        return services;
    }

    private static IServiceCollection AddOctokitServices(
        this IServiceCollection services)
    {
        services.AddGitHubActionsCore();

        static RepoConfig GetRepositoryConfiguration(IServiceProvider provider)
        {
            var core = provider.GetRequiredService<ICoreService>();

            var context = Context.Current;
            var repository = context.Repo;

            var token = core.GetInput("token");

            return (repository.Owner, repository.Repo, token);
        }

        services.AddSingleton(static provider =>
        {
            var (owner, repo, token) = GetRepositoryConfiguration(provider);

            return new GitHubRestClient(owner, repo, token);
        });

        services.AddSingleton(static provider =>
        {
            var (owner, repo, token) = GetRepositoryConfiguration(provider);

            return new GitHubGraphQLClient(owner, repo, token);
        });

        return services;
    }
}
