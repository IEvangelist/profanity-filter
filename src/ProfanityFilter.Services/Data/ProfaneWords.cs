// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Data;

internal static partial class ProfaneWords
{
    internal static Dictionary<string, string[]> AllSwearWords { get; } = new()
    {
        ["Arabic"] = ArabicSwearWords!,
        ["British"] = BritishSwearWords!,
        ["Chinese"] = ChineseSwearWords!,
        ["French"] = FrenchSwearWords!,
        ["German"] = GermanSwearWords!,
        ["GoogleBanned"] = GermanSwearWords!,
        ["Indonesian"] = IndonesianSwearWords!,
        ["Italian"] = ItalianSwearWords!,
        ["Spanish"] = SpanishSwearWords!,
    };
}
