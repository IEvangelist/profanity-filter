// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Tests;

[TestClass]
public class ServiceCollectionExtensionsTests
{
    private const string INPUT_TOKEN = nameof(INPUT_TOKEN);

    [TestMethod]
    public void AddProfanityFilter_AddsServices()
    {
        Environment.SetEnvironmentVariable(INPUT_TOKEN, "TEST");

        try
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddActionProcessorServices();

            // Assert
            var provider = services.BuildServiceProvider();
            var censorService = provider.GetService<IProfaneContentFilterService>();

            Assert.IsNotNull(censorService);
            Assert.IsInstanceOfType<DefaultProfaneContentFilterService>(censorService);
        }
        finally
        {
            Environment.SetEnvironmentVariable(INPUT_TOKEN, null);
        }
    }
}
