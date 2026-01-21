// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Configuration;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides extension methods for configuration.
/// </summary>
public static class ConfigurationExtensions
{
    private const string DotNetRunningInContainerKey = "DOTNET_RUNNING_IN_CONTAINER";

    /// <summary>
    /// Determines whether the application is running inside a container.
    /// </summary>
    /// <param name="configuration">The configuration instance to check.</param>
    /// <returns>
    /// <c>true</c> if the application is running inside a container; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsRunningInContainer(this IConfiguration configuration)
    {
        return configuration.GetValue<bool>(DotNetRunningInContainerKey)
            || Environment.GetEnvironmentVariable(DotNetRunningInContainerKey)
               is "true" or "TRUE" or "True" or "1";
    }
}
