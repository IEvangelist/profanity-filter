// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Filters;

internal static class Symbols
{
    /// <summary>
    /// Only permit a single <c>"\$"</c> string within an entire grawlix
    /// replacement, to avoid GitHub from rendering it as a Math expression.
    /// </summary>
    internal const string LimitGrawlixToOne = $"\\{UnescapedLimitGrawlixToOne}";

    /// <summary>
    /// Only permit a single <c>"$"</c> string within an entire grawlix
    /// replacement, to avoid GitHub from rendering it as a Math expression.
    /// </summary>
    internal const string UnescapedLimitGrawlixToOne = "$";

    /// <summary>
    /// An array of hand-selected grawlix replacements for profane words.
    /// See <a href="https://english.stackexchange.com/a/86840/446108"></a>
    /// </summary>
    internal static string[] Grawlixes =
    [
        new Symbol('#'),
        new Symbol('$'),
        new Symbol('@'),
        new Symbol('!'),
        new Symbol('%'),
        new Symbol('&'),
        new Symbol('*'),
        new Symbol('+'),
        new Symbol('?'),
        new Symbol('^'),
    ];

    /// <summary>
    /// An array of unescaped grawlixes.
    /// </summary>
    internal static string[] UnescapedGrawlixes =
    [
        "#",
        "$",
        "@",
        "!",
        "%",
        "&",
        "*",
        "+",
        "?",
        "^"
    ];
}

file readonly record struct Symbol(char Value, bool Escape = true)
{
    /// <summary>
    /// Implicitly converts a <see cref="char"/> to a <see cref="Symbol"/>.
    /// </summary>
    public static implicit operator Symbol(char value) => new(value);

    /// <summary>
    /// Implicitly converts a <see cref="Symbol"/> to a <see cref="string"/>.
    /// </summary>
    public static implicit operator string(Symbol symbol) => symbol.ToString();

    public override string ToString()
    {
        return Escape ? $"\\{Value}" : $"{Value}";
    }
}
