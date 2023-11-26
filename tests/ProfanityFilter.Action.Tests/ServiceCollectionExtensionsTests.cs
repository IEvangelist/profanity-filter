// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddProfanityFilter_AddsServices()
    {
        Environment.SetEnvironmentVariable(Keys.GITHUB_TOKEN, "TEST");

        try
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddActionProcessorServices();

            // Assert
            var provider = services.BuildServiceProvider();
            var censorService = provider.GetService<IProfaneContentCensorService>();

            Assert.NotNull(censorService);
            Assert.IsType<DefaultProfaneContentCensorService>(censorService);
        }
        finally
        {
            Environment.SetEnvironmentVariable(Keys.GITHUB_TOKEN, null);
        }
    }
}
