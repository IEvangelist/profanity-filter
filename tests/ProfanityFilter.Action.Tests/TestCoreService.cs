// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Runtime.CompilerServices;

namespace ProfanityFilter.Action.Tests;

internal class TestCoreService(Dictionary<string, string> testInputs) : ICoreService
{
    internal enum BufferType { Info, Debug, Notice, Warning, Error };

    private readonly Dictionary<BufferType, List<string>> _buffers = new()
    {
        [BufferType.Info] = [],
        [BufferType.Debug] = [],
        [BufferType.Notice] = [],
        [BufferType.Warning] = [],
        [BufferType.Error] = [],
    };

    public Summary Summary { get; } = new();
    public bool IsDebug { get; } = true;

    [IndexerName("Buffers")]
    internal List<string> this[BufferType type] => _buffers[type];

    public ValueTask AddPathAsync(string inputPath) => throw new NotImplementedException();

    public void WriteDebug(string message) =>
        _buffers[BufferType.Debug].Add(message);

    public void EndGroup() => throw new NotImplementedException();

    public void WriteError(string message, AnnotationProperties? properties = null) =>
        _buffers[BufferType.Error].Add(message);

    public ValueTask ExportVariableAsync(string name, string value) => throw new NotImplementedException();

    public bool GetBoolInput(string name, InputOptions? options = null) => throw new NotImplementedException();

    public string GetInput(string name, InputOptions? options = null) => testInputs.TryGetValue(name, out var value) ? value : "";

    public string[] GetMultilineInput(string name, InputOptions? options = null) => throw new NotImplementedException();

    public string GetState(string name) => throw new NotImplementedException();

    public ValueTask<T> GroupAsync<T>(string name, Func<ValueTask<T>> action) => throw new NotImplementedException();

    public void WriteInfo(string message) =>
        _buffers[BufferType.Info].Add(message);

    public void WriteNotice(string message, AnnotationProperties? properties = null) =>
        _buffers[BufferType.Notice].Add(message);

    public ValueTask SaveStateAsync<T>(string name, T value, JsonTypeInfo<T>? typeInfo = null) => throw new NotImplementedException();

    public void SetCommandEcho(bool enabled) => throw new NotImplementedException();

    public void SetFailed(string message) => throw new NotImplementedException();

    public ValueTask SetOutputAsync<T>(string name, T value, JsonTypeInfo<T>? typeInfo = null) => throw new NotImplementedException();

    public void SetSecret(string secret) => throw new NotImplementedException();

    public void StartGroup(string name) => throw new NotImplementedException();

    public void WriteWarning(string message, AnnotationProperties? properties = null) =>
        _buffers[BufferType.Warning].Add(message);
}