// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Aspire.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

internal static class DistributedApplicationBuilderExtensions
{
    internal static string GetAspireTempPath(this IDistributedApplicationBuilder builder)
    {
        var appNameHash = builder.Configuration["AppHost:Sha256"]![..10];

        return Path.Combine(Path.GetTempPath(), $"aspire.{appNameHash}");
    }
}
