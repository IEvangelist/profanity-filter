// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Shared.Tests;

[TestClass]
public class MaybeExtensionsTests
{
    [TestMethod]
    public void AsMaybeWithNonNullValueReturnsSomething()
    {
        var value = "test";
        var result = value.AsMaybe();

        Assert.IsInstanceOfType<Something<string>>(result);
        Assert.AreEqual(value, ((Something<string>)result).Value);
    }

    [TestMethod]
    public void AsMaybeWithNullValueReturnsNothing()
    {
        string? value = null;
        var result = value.AsMaybe();

        Assert.IsInstanceOfType<Nothing<string>>(result);
    }

    [TestMethod]
    public void ChainWithSomethingReturnsTransformedSomething()
    {
        var initial = new Something<int>(5);
        var result = initial.Chain(x => x * 2).Chain(x => x * 3.1);

        Assert.IsInstanceOfType<Something<double>>(result);
        Assert.AreEqual(31d, ((Something<double>)result).Value);

        var stringSomething = result.Chain(x => (x / 2).ToString());

        Assert.IsInstanceOfType<Something<string>>(stringSomething);
        Assert.AreEqual("15.5", ((Something<string>)stringSomething).Value);
    }

    [TestMethod]
    public void ChainWithNothingReturnsNothing()
    {
        var initial = new Nothing<int>();
        var result = initial.Chain(x => x * 2);

        Assert.IsInstanceOfType<Nothing<int>>(result);
    }
}

