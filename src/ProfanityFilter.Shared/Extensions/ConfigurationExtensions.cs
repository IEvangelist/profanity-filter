// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Configuration;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ConfigurationExtensions
{
    public static bool IsRunningInContainer(this IConfiguration configuration)
    {
        return configuration.GetValue<bool>("DOTNET_RUNNING_IN_CONTAINER");
    }
}
