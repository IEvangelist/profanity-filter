// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for building profanity filter resources.
/// </summary>
public static class ProfanityFilterResourceBuilderExtensions
{
    /// <summary>
    /// Adds a profanity filter resource to the distributed application builder.
    /// </summary>
    /// <remarks>
    /// This version of the package defaults to the
    /// <inheritdoc cref="ProfanityFilterContainerImageDetails.Registry"/>/<inheritdoc cref="ProfanityFilterContainerImageDetails.Image"/>:<inheritdoc cref="ProfanityFilterContainerImageDetails.Tag"/> container image.
    /// </remarks>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="devCertPassword">The optional password resource builder.</param>
    /// <returns>An <see cref="IResourceBuilder{ProfanityFilterResource}"/> for the profanity filter resource.</returns>
    public static IResourceBuilder<ProfanityFilterResource> AddProfanityFilter(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name,
        IResourceBuilder<ParameterResource>? devCertPassword = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var resource = new ProfanityFilterResource(name);

        var profanityFilter = builder.AddResource(resource)
            .WithImage(ProfanityFilterContainerImageDetails.Image)
            .WithImageRegistry(ProfanityFilterContainerImageDetails.Registry)
            .WithImageTag(ProfanityFilterContainerImageDetails.Tag)
            .WithContainerName(name)
            .WithBindMount(Path.Combine(builder.GetAspireTempPath(), "Data"), @"/var/tmp")
            .WithLifetime(ContainerLifetime.Persistent)
            .RunWithHttpsDevCert(devCertPassword);

        return profanityFilter;
    }

    /// <summary>
    /// Adds a bind mount for the data folder to a <see cref="ProfanityFilterResource"/>.
    /// Added data files should be newline delimited and have a <i>*.txt</i> file extension.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="source">The source directory on the host to mount into the container.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    /// <example>
    /// Add a profanity filter container to the application model and reference it in a .NET project. Additionally, in this
    /// example a bind mount is added to the container to allow data to be persisted across container restarts.
    /// <code lang="csharp">
    /// var builder = DistributedApplication.CreateBuilder(args);
    ///
    /// var profanityFilter = builder.AddElasticsearch("profanity-filter")
    ///    .WithDataBindMount("./data");
    ///
    /// var api = builder.AddProject&lt;Projects.Api&gt;("api")
    ///    .WithReference(profanityFilter);
    ///
    /// builder.Build().Run();
    /// </code>
    /// </example>
    public static IResourceBuilder<ProfanityFilterResource> WithDataBindMount(
        this IResourceBuilder<ProfanityFilterResource> builder,
        string source)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(source);

        return builder.WithBindMount(source, "/app/CustomData", true);
    }
}
