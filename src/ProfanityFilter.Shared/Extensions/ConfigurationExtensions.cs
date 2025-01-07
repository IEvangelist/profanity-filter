// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Configuration;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ConfigurationExtensions
{
    /// <summary>
    /// Determines whether the application is running inside a container.
    /// </summary>
    /// <param name="configuration">The configuration instance to check.</param>
    /// <returns>
    /// <c>true</c> if the application is running inside a container; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsRunningInContainer(this IConfiguration configuration)
    {
        return configuration.GetValue<bool>("DOTNET_RUNNING_IN_CONTAINER")
            || Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")
               is "true" or "TRUE" or "True" or "1";
    }
}
