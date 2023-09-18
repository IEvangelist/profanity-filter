// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Extensions;

internal static class CharExtensions
{
    private static readonly char[] s_vowels = [
        'ā',
        'á',
        'ǎ',
        'à',
        'ē',
        'é',
        'ě',
        'è',
        'ī',
        'í',
        'ǐ',
        'ì',
        'ō',
        'ó',
        'ǒ',
        'ò',
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
