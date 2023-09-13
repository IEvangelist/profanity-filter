// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddProfanityFilterServices_AddsServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddProfanityFilterServices();

        // Assert
        var provider = services.BuildServiceProvider();
        var censorService = provider.GetService<IProfaneContentCensorService>();

        Assert.NotNull(censorService);
        Assert.IsType<DefaultProfaneContentCensorService>(censorService);
    }
}
