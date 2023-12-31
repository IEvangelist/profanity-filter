﻿// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddActionProcessorServices(this IServiceCollection services)
    {
        services.AddSingleton<ActionProcessor>();

        services.AddProfanityFilterServices();

        services.AddGitHubActionsCore();

        var inputToken = Env.GetEnvironmentVariable("INPUT_TOKEN");

        ArgumentException.ThrowIfNullOrWhiteSpace(inputToken);

        services.AddGitHubClientServices(inputToken);

        services.AddSingleton(static provider =>
        {
            var core = provider.GetRequiredService<ICoreService>();

            try
            {
                core.StartGroup("Initializing context");

                var client = provider.GetRequiredService<GitHubClient>();
                var context = provider.GetRequiredService<Context>();

                var repository = context.Repo;

                var config = (repository.Owner, repository.Repo);

                core.Info($"Repository: {config.Owner}/{config.Repo}");

                return new GitHubRestClient(client, core, config);
            }
            finally
            {
                core.EndGroup();
            }
        });

        return services;
    }
}
