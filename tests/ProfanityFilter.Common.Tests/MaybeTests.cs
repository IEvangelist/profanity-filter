// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Common.Tests;

[TestClass]
public sealed class MaybeTests
{
    [TestMethod]
    public void DefaultRepresentsAbsent()
    {
        Maybe<string> value = default;

        Assert.IsTrue(value is Absent<string>);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void SwitchHandlesBothCases(bool isAvailable)
    {
        Maybe<int> value = isAvailable
            ? new Available<int>(42)
            : new Absent<int>();

        var actual = value switch
        {
            Available<int> available => available.Value,
            Absent<int> => -1
        };

        Assert.AreEqual(isAvailable ? 42 : -1, actual);
    }

    [TestMethod]
    public void AvailableDefaultValueIsNotAbsent()
    {
        Maybe<int> value = new Available<int>(0);

        Assert.IsTrue(value is Available<int> { Value: 0 });
    }

    [TestMethod]
    public void AvailableNullReferenceIsNotAbsent()
    {
        Maybe<string?> value = new Available<string?>(null);

        Assert.IsTrue(value is Available<string?> { Value: null });
        Assert.AreNotEqual(default, value);
    }

    [TestMethod]
    public void AvailableNullNullableValueIsNotAbsent()
    {
        Maybe<int?> value = new Available<int?>(null);

        Assert.IsTrue(value is Available<int?> { Value: null });
    }

    [TestMethod]
    public void AvailableUnionMembersAgreeWithPatterns()
    {
        Maybe<int> value = new Available<int>(42);
        var members = (Maybe<int>.IUnionMembers)value;

        Assert.IsTrue(members.HasValue);
        Assert.IsTrue(members.TryGetValue(out Available<int> available));
        Assert.AreEqual(42, available.Value);
        Assert.IsFalse(members.TryGetValue(out Absent<int> _));
        Assert.IsInstanceOfType<Available<int>>(members.Value);
    }

    [TestMethod]
    public void AbsentUnionMembersAgreeWithPatterns()
    {
        Maybe<int> value = default;
        var members = (Maybe<int>.IUnionMembers)value;

        Assert.IsTrue(members.HasValue);
        Assert.IsFalse(members.TryGetValue(out Available<int> _));
        Assert.IsTrue(members.TryGetValue(out Absent<int> _));
        Assert.IsInstanceOfType<Absent<int>>(members.Value);
        Assert.AreEqual(value, (Maybe<int>)new Absent<int>());
    }
}
