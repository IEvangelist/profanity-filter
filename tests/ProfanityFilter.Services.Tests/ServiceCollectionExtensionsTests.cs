// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddProfanityFilterServices();

        // Assert
        var provider = services.BuildServiceProvider();
        var censorService = provider.GetService<IProfaneContentFilterService>();

        Assert.IsNotNull(censorService);
        Assert.IsInstanceOfType<DefaultProfaneContentFilterService>(censorService);
    }
}
