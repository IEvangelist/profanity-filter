// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.WebApi.Hubs;

public static partial class Log
{
    [LoggerMessage(
        Message = """
        Starting a live stream for: {ConnectionId}
        """)]
    public static partial void LogStartedStream(
        this ILogger logger,
        string connectionId,
        LogLevel logLevel = LogLevel.Information);

    //
    [LoggerMessage(
    Message = """
        Ending live stream for: {ConnectionId}.
        """)]
    public static partial void LogEndingStream(
    this ILogger logger,
    string connectionId,
    LogLevel logLevel = LogLevel.Information);

    [LoggerMessage(
        Message = """
        ({ConnectionId}) Filter result was either null or its final output was null.
        """)]
    public static partial void LogInvalidFilterResult(
        this ILogger logger,
        string connectionId,
        LogLevel logLevel = LogLevel.Debug);
}
