// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Client.Tests;

[TestClass]
public class ProfanityFilterClientTests
{
    [TestMethod]
    public void DeconstructCorrectlyMaterializesNullClients()
    {
        var sut = new ProfanityFilterClient(null!, null!);

        var (rest, realtime) = sut;

        Assert.IsNull(rest);
        Assert.IsNull(realtime);
    }

    [TestMethod]
    public void DeconstructCorrectlyMaterializesTargetClients()
    {
        var sut = new ProfanityFilterClient(
            new TestRestClient(), new TestRealTimeClient());

        var (rest, realtime) = sut;

        Assert.IsNotNull(rest);
        Assert.IsInstanceOfType<TestRestClient>(rest);
        Assert.IsNotNull(realtime);
        Assert.IsInstanceOfType<TestRealTimeClient>(realtime);
    }
}
