public static partial class Log
{
    [LoggerMessage(
        Message = """
            Source word list for profane content:
            """)]
    public static partial void SourceWordListingLead(
        this ILogger logger,
        LogLevel logLevel = LogLevel.Information);

    [LoggerMessage(
        Message = """
        "{Index} {File}"
        """)]
    public static partial void LogFileIndexAndName(
        this ILogger logger,
        int index,
        string file,
        LogLevel logLevel = LogLevel.Information);
}