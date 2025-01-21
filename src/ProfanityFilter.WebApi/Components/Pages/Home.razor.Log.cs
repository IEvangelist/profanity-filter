// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

#pragma warning disable SYSLIB1013

namespace ProfanityFilter.WebApi.Components.Pages;

internal static partial class Log
{
    [LoggerMessage(
        Message = """
            Hub connection started.
            """)]
    public static partial void HubStarted(
        this ILogger logger,
        LogLevel logLevel = LogLevel.Information);

    [LoggerMessage(
        Message = """
            Hub connection closed: {Exception}
            """)]
    public static partial void HubConnectionClosed(
        this ILogger logger,
        Exception? exception,
        LogLevel logLevel = LogLevel.Information);

    [LoggerMessage(
        Message = """
            Hub connection reconnecting: {Exception}
            """)]
    public static partial void HubReconnecting(
        this ILogger logger,
        Exception? exception,
        LogLevel logLevel = LogLevel.Information);

    [LoggerMessage(
        Message = """
            Hub connection reconnected: {Arg}.
            """)]
    public static partial void HubReconnected(
        this ILogger logger,
        string? arg,
        LogLevel logLevel = LogLevel.Information);

    [LoggerMessage(
        Message = """
            Not connected to the profanity filter hub.
            """)]
    public static partial void NotConnectedToHub(
        this ILogger logger,
        LogLevel logLevel = LogLevel.Information);

    [LoggerMessage(
        Message = """
            Error processing profanity: {Exception}.
            """)]
    public static partial void ErrorProcessProfanity(
        this ILogger logger,
        Exception? exception,
        LogLevel logLevel = LogLevel.Information);
}

#pragma warning restore SYSLIB1013