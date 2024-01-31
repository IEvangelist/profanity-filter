// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Action.Tests;

public class ProfanityProcessorTests
{
    [Theory]
    [InlineData("emoji", ReplacementStrategy.Emoji)]
    [InlineData("EMOJI", ReplacementStrategy.Emoji)]
    [InlineData("eMoJi", ReplacementStrategy.Emoji)]
    [InlineData("anger-emoji", ReplacementStrategy.AngerEmoji)]
    [InlineData("angerEmoji", ReplacementStrategy.AngerEmoji)]
    [InlineData("AngerEmoji", ReplacementStrategy.AngerEmoji)]
    [InlineData("first-letter-then-asterisk", ReplacementStrategy.FirstLetterThenAsterisk)]
    [InlineData(nameof(ReplacementStrategy.FirstLetterThenAsterisk), ReplacementStrategy.FirstLetterThenAsterisk)]
    public void ProfanityProcessorCorrectlyParsesReplacementStrategy(string input, ReplacementStrategy expected)
    {
        ICoreService sut = new TestCoreService(new()
        {
            ["replacement-strategy"] = input
        });

        var actual = sut.GetReplacementStrategy();

        Assert.Equal(expected, actual);
    }
}

internal class TestCoreService(Dictionary<string, string> testInputs) : ICoreService
{
    public Summary Summary { get; } = new();
    public bool IsDebug { get; } = true;

    public ValueTask AddPathAsync(string inputPath) => throw new NotImplementedException();

    public void Debug(string message) => throw new NotImplementedException();

    public void EndGroup() => throw new NotImplementedException();

    public void Error(string message, AnnotationProperties? properties = null) => throw new NotImplementedException();

    public ValueTask ExportVariableAsync(string name, string value) => throw new NotImplementedException();

    public bool GetBoolInput(string name, InputOptions? options = null) => throw new NotImplementedException();

    public string GetInput(string name, InputOptions? options = null) => testInputs.TryGetValue(name, out var value) ? value : "";

    public string[] GetMultilineInput(string name, InputOptions? options = null) => throw new NotImplementedException();

    public string GetState(string name) => throw new NotImplementedException();

    public ValueTask<T> GroupAsync<T>(string name, Func<ValueTask<T>> action) => throw new NotImplementedException();

    public void Info(string message) => throw new NotImplementedException();

    public void Notice(string message, AnnotationProperties? properties = null) => throw new NotImplementedException();

    public ValueTask SaveStateAsync<T>(string name, T value, JsonTypeInfo<T>? typeInfo = null) => throw new NotImplementedException();

    public void SetCommandEcho(bool enabled) => throw new NotImplementedException();

    public void SetFailed(string message) => throw new NotImplementedException();

    public ValueTask SetOutputAsync<T>(string name, T value, JsonTypeInfo<T>? typeInfo = null) => throw new NotImplementedException();

    public void SetSecret(string secret) => throw new NotImplementedException();

    public void StartGroup(string name) => throw new NotImplementedException();

    public void Warning(string message, AnnotationProperties? properties = null) => throw new NotImplementedException();
}