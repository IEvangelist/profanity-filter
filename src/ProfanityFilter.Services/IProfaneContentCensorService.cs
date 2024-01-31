// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services;

/// <summary>
/// Defines methods for checking and censoring profane content.
/// </summary>
public interface IProfaneContentCensorService
{
    /// <summary>
    /// Censors any profanity in the specified content.
    /// </summary>
    /// <param name="content">The content to censor.</param>
    /// <param name="replacementStrategy">The type of replacement to use for censoring.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous 
    /// operation, containing the censored content.</returns>
    ValueTask<FilterResult> CensorProfanityAsync(
        string content,
        ReplacementStrategy replacementStrategy);
}
