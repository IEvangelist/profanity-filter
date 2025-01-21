// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

#pragma warning disable IDE0130 // Namespace does not match folder structure
using System.Diagnostics.CodeAnalysis;

namespace Aspire.Hosting.ApplicationModel;
/// <summary>
/// Represents a resource for the profanity filter with a specified name.
/// </summary>
/// <param name="name">The name of the resource.</param>
public sealed class ProfanityFilterResource([ResourceName] string name) :
    ContainerResource(name),
    IResourceWithConnectionString
{
    /// <summary>
    /// Gets the HTTPS endpoint reference for the resource.
    /// </summary>
    /// <remarks>
    /// This property is lazily initialized.
    /// </remarks>
    [field: AllowNull]
    public EndpointReference HttpsEndpoint =>
        field ??= new(this, "https");

    /// <summary>
    /// Gets the connection string expression for the resource.
    /// </summary>
    /// <returns>
    /// A <see cref="ReferenceExpression"/> representing the connection string.
    /// </returns>
    ReferenceExpression IResourceWithConnectionString.ConnectionStringExpression =>
        ReferenceExpression.Create(
            $"https://{HttpsEndpoint.Property(EndpointProperty.Host)}:{HttpsEndpoint.Property(EndpointProperty.Port)}"
        );
}
