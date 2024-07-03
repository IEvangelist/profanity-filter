namespace ProfanityFilter.WebApi.Extensions;

public static partial class StringExtensions
{
    private static readonly MarkdownPipeline s_pipeline = new MarkdownPipelineBuilder()
        .ConfigureNewLine("\n")
        .UseAdvancedExtensions()
        .UseEmojiAndSmiley()
        .UseSoftlineBreakAsHardlineBreak()
        .UseBootstrap()
        .Build();

    public static string ToHtml(this string markdown) => string.IsNullOrWhiteSpace(markdown) is false
        ? Markdown.ToHtml(markdown, s_pipeline)
        : "";

    public static string? SplitCamelCase(this string? camelCaseText)
    {
        return camelCaseText switch
        {
            { Length: > 0 } => string.Join("", SplitString(camelCaseText)),
            _ => camelCaseText
        };

        static IEnumerable<char> SplitString(string value)
        {
            var span = value.ToCharArray();

            for (var i = 0; i < span.Length; ++i)
            {
                var @char = span[i];
                if (char.IsUpper(@char))
                {
                    yield return ' ';
                }

                yield return @char;
            }
        }
    }
}