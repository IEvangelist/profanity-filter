// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Aspire.Hosting;

internal static class DistributedApplicationBuilderExtensions
{
    internal static string GetAspireTempPath(this IDistributedApplicationBuilder builder)
    {
        var appNameHash = builder.Configuration["AppHost:Sha256"]![..10];

        return Path.Combine(Path.GetTempPath(), $"aspire.{appNameHash}");
    }
}
