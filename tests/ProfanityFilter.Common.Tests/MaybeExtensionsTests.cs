// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Common.Tests;

[TestClass]
public class MaybeExtensionsTests
{
    [TestMethod]
    public void AsMaybeWithNonNullValueReturnsSomething()
    {
        var value = "test";
        var result = value.AsMaybe();

        Assert.IsInstanceOfType<Available<string>>(result);
        Assert.AreEqual(value, ((Available<string>)result).Value);
    }

    [TestMethod]
    public void AsMaybeWithNullValueReturnsNothing()
    {
        string? value = null;
        var result = value.AsMaybe();

        Assert.IsInstanceOfType<Absent<string>>(result);
    }

    [TestMethod]
    public void ChainWithSomethingReturnsTransformedSomething()
    {
        var initial = new Available<int>(5);
        var result = initial.Chain(x => x * 2).Chain(x => x * 3.1);

        Assert.IsInstanceOfType<Available<double>>(result);
        Assert.AreEqual(31d, ((Available<double>)result).Value);

        var stringSomething = result.Chain(x => (x / 2).ToString(CultureInfo.InvariantCulture));

        Assert.IsInstanceOfType<Available<string>>(stringSomething);
        Assert.AreEqual("15.5", ((Available<string>)stringSomething).Value);
    }

    [TestMethod]
    public void ChainWithNothingReturnsNothing()
    {
        var initial = new Absent<int>();
        var result = initial.Chain(x => x * 2);

        Assert.IsInstanceOfType<Absent<int>>(result);
    }
}

