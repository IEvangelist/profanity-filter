// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Tests;

[TestClass]
public class ServiceCollectionExtensionsTests
{
    [TestMethod]
    public void AddProfanityFilterServices_AddsServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddProfanityFilterServices();

        // Assert
        var provider = services.BuildServiceProvider();
        var censorService = provider.GetService<IProfaneContentFilterService>();

        Assert.IsNotNull(censorService);
        Assert.IsInstanceOfType<DefaultProfaneContentFilterService>(censorService);
    }
}
