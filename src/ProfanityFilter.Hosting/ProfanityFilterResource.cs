// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Aspire.Hosting.ApplicationModel;

public sealed class ProfanityFilterResource([ResourceName] string name) :
    ContainerResource(name),
    IResourceWithConnectionString
{
#pragma warning disable CS9264 // Non-nullable property must contain a non-null value when exiting constructor. Consider adding the 'required' modifier, or declaring the property as nullable, or adding '[field: MaybeNull, AllowNull]' attributes.
    public EndpointReference HttpsEndpoint =>
        field ??= new(this, "https");
#pragma warning restore CS9264 // Non-nullable property must contain a non-null value when exiting constructor. Consider adding the 'required' modifier, or declaring the property as nullable, or adding '[field: MaybeNull, AllowNull]' attributes.

    ReferenceExpression IResourceWithConnectionString.ConnectionStringExpression =>
        ReferenceExpression.Create(
            $"https://{HttpsEndpoint.Property(EndpointProperty.Host)}:{HttpsEndpoint.Property(EndpointProperty.Port)}"
        );
}
