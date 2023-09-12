// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

/// <summary>
/// Defines methods for checking and censoring profane content.
/// </summary>
public interface IProfaneContentCensorService
{
    /// <summary>
    /// Determines whether the specified content contains profanity.
    /// </summary>
    /// <param name="content">The content to check for profanity.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous 
    /// operation, containing a <see cref="bool"/> indicating whether the content contains profanity.</returns>
    ValueTask<bool> ContainsProfanityAsync(string content);

    /// <summary>
    /// Censors any profanity in the specified content.
    /// </summary>
    /// <param name="content">The content to censor.</param>
    /// <param name="replacementType">The type of replacement to use for censoring.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous 
    /// operation, containing the censored content.</returns>
    ValueTask<string> CensorProfanityAsync(string content, ReplacementType replacementType);
}
