// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Common.Tests;

[TestClass]
public sealed class NothingTests
{
    [TestMethod]
    public void NothingIsRepresentedAsSuch()
    {
        var sut = new Absent<DateTime>();

        Assert.IsInstanceOfType<Absent<DateTime>>(sut);
        Assert.IsNotInstanceOfType<DateTime>(sut);
    }
}
