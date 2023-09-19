// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Extensions;

internal static class CharExtensions
{
    private static readonly char[] s_vowels =
    [
        'a',
        'ā',
        'á',
        'ǎ',
        'à',
        'e',
        'ē',
        'é',
        'ě',
        'è',
        'i',
        'ī',
        'í',
        'ǐ',
        'ì',
        'o',
        'ō',
        'ó',
        'ǒ',
        'ò',
        'u',
        'ū',
        'ú',
        'ǔ',
        'ù',
        'ǖ',
        'ǘ',
        'ǚ',
        'ǜ',
        'ü'
        ];

    /// <summary>
    /// Determines whether the specified character is a vowel.
    /// </summary>
    /// <param name="char">The character to check.</param>
    /// <returns>Returns <c>true</c> if the specified character is 
    /// a vowel; otherwise, <c>false</c>.</returns>
    internal static bool IsVowel(this char @char) =>
        char.IsLetter(@char) &&
        s_vowels.Contains(char.ToLowerInvariant(@char));
}
