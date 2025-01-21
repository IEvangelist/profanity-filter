// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.WebApi.Compliance;

internal sealed class CharacterRedactor(char redactionCharacter = '█') : Redactor
{
    public override int GetRedactedLength(ReadOnlySpan<char> input) => input.Length;

    public override int Redact(ReadOnlySpan<char> source, Span<char> destination)
    {
        for (var i = 0; i < source.Length; i++)
        {
            destination[i] = redactionCharacter;
        }

        return source.Length;
    }
}
