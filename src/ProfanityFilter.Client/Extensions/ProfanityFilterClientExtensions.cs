// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Extension methods for adding the <see cref="ProfanityFilterClient"/>, <see cref="IRestClient"/>,
/// and <see cref="IRealtimeClient"/> to the service collection.
/// </summary>
public static class ProfanityFilterClientExtensions
{
    /// <summary>
    /// Adds the <see cref="ProfanityFilterClient"/>, <see cref="IRestClient"/>,
    /// and <see cref="IRealtimeClient"/> to the service collection.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> instance.</param>
    /// <param name="connectionName">The name of the connection string.</param>
    /// <param name="configureOptions">An optional action to configure the <see cref="ProfanityFilterOptions"/>.</param>
    public static void AddProfanityFilterClient(
        this IHostApplicationBuilder builder,
        string connectionName,
        Action<ProfanityFilterOptions>? configureOptions = null) =>
        AddProfanityFilterClient(builder, configureOptions, connectionName, serviceKey: null);

    /// <summary>
    /// Adds a keyed <see cref="ProfanityFilterClient"/>, <see cref="IRestClient"/>,
    /// and <see cref="IRealtimeClient"/> to the service collection.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> instance.</param>
    /// <param name="name">The name of the keyed service.</param>
    /// <param name="configureOptions">An optional action to configure the <see cref="ProfanityFilterOptions"/>.</param>
    public static void AddKeyedProfanityFilterClient(
        this IHostApplicationBuilder builder,
        string name,
        Action<ProfanityFilterOptions>? configureOptions = null)
    {
        ArgumentNullException.ThrowIfNull(name);

        AddProfanityFilterClient(builder, configureOptions, connectionName: name, serviceKey: name);
    }

    private static void AddProfanityFilterClient(
        IHostApplicationBuilder builder,
        Action<ProfanityFilterOptions>? configure,
        string connectionName,
        object? serviceKey)
    {
        ArgumentNullException.ThrowIfNull(builder);

        ProfanityFilterOptions options = new();

        var configSection = builder.Configuration.GetSection(ProfanityFilterOptions.SectionName);
        var namedConfigSection = configSection.GetSection(connectionName);

        configSection.Bind(options);
        namedConfigSection.Bind(options);

        if (builder.Configuration.GetConnectionString(connectionName) is string connectionString &&
            Uri.TryCreate(connectionString, UriKind.Absolute, out var baseAddress))
        {
            options.ApiBaseAddress = baseAddress;
        }

        configure?.Invoke(options);

        builder.Services.AddOptions<ProfanityFilterOptions>();
        builder.Services.AddLogging();

        builder.Services.AddScoped<IRestClient, DefaultRestClient>();
        builder.Services.AddScoped<IRealtimeClient, DefaultRealtimeClient>();

        if (serviceKey is null)
        {
            builder.Services.AddScoped<ProfanityFilterClient>();
        }
        else
        {
            builder.Services.AddKeyedScoped<ProfanityFilterClient>(serviceKey);
        }
    }
}
