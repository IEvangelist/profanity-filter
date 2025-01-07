// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Shared.Tests;

[TestClass]
public sealed class NothingTests
{
    [TestMethod]
    public void NothingIsRepresentedAsSuch()
    {
        var sut = new Nothing<DateTime>();

        Assert.IsInstanceOfType<Nothing<DateTime>>(sut);
        Assert.IsNotInstanceOfType<DateTime>(sut);
    }
}
