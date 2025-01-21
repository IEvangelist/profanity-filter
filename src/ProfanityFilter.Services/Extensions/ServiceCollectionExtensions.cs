﻿// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides extension methods for configuring services related to profanity filtering.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the necessary services for the profanity filter to the specified <see cref="IServiceCollection"/>.
    /// Specifically, see <see cref="IProfaneContentFilterService"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddProfanityFilterServices(
        this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddSingleton<IProfaneContentFilterService, DefaultProfaneContentFilterService>();

        return services;
    }
}
