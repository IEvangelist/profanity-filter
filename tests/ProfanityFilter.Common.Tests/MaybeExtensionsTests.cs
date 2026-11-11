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

        Assert.AreEqual(value, result switch
        {
            Available<string> available => available.Value,
            Absent<string> => null
        });
    }

    [TestMethod]
    public void AsMaybeWithNullValueReturnsNothing()
    {
        string? value = null;
        var result = value.AsMaybe();

        Assert.IsTrue(result is Absent<string>);
    }

    [TestMethod]
    public void ChainWithSomethingReturnsTransformedSomething()
    {
        Maybe<int> initial = new Available<int>(5);
        var result = initial.Chain(x => x * 2).Chain(x => x * 3.1);

        Assert.IsTrue(result is Available<double> { Value: 31d });

        var stringSomething = result.Chain(x => (x / 2).ToString(CultureInfo.InvariantCulture));

        Assert.IsTrue(stringSomething is Available<string> { Value: "15.5" });
    }

    [TestMethod]
    public void ChainWithNothingReturnsNothing()
    {
        Maybe<int> initial = new Absent<int>();
        var result = initial.Chain(x => x * 2);

        Assert.IsTrue(result is Absent<int>);
    }

    [TestMethod]
    public void ChainWithDefaultDoesNotInvokeCallback()
    {
        Maybe<int> initial = default;
        var invoked = false;

        var result = initial.Chain(value =>
        {
            invoked = true;
            return value * 2;
        });

        Assert.IsFalse(invoked);
        Assert.IsTrue(result is Absent<int>);
    }

    [TestMethod]
    public void ChainWithNullResultPreservesAvailableCase()
    {
        Maybe<int> initial = new Available<int>(5);

        var result = initial.Chain(static _ => (string?)null);

        Assert.IsTrue(result is Available<string?> { Value: null });
    }
}
