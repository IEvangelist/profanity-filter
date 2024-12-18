// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.WebApi.Services;

public sealed class BaseAddressResolver(
    IConfiguration configuration,
    IServerAddressesFeature serverAddresses,
    NavigationManager navigationManager)
{
    private bool IsRunningInContainer => configuration.GetValue<bool>("DOTNET_RUNNING_IN_CONTAINER");

    public string GetBaseAddress()
    {
        return IsRunningInContainer switch
        {
            // When running in a container, use the internal container port.
            true => "https://localhost:8081",

            // Otherwise, rely on the server address or the base URI.
            _ => serverAddresses.Addresses.FirstOrDefault(address => address.StartsWith("https"))
                ?? navigationManager.BaseUri
        };
    }
}
