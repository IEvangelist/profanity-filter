// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddProfanityFilter(
        this IServiceCollection services)
    {
        services.AddProfanityFilterServices();

        return services;
    }

    internal static IServiceCollection AddOctokitServices(
        this IServiceCollection services)
    {
        services.AddSingleton(static provider =>
        {
            var core = provider.GetRequiredService<ICoreService>();

            var owner = core.GetInput("owner");
            var repo = core.GetInput("repo");
            var token = core.GetInput("token");

            return new GitHubGraphQLClient(owner, repo, token);
        });

        return services;
    }
}
