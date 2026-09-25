// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using System.Collections.Frozen;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Text.RegularExpressions;
global using ActionsToolkit.Core.Extensions;
global using ActionsToolkit.Core.Markdown;
global using ActionsToolkit.Core.Services;
global using ActionsToolkit.Core.Summaries;
global using ActionsToolkit.Octokit;
global using ActionsToolkit.Octokit.Extensions;
global using GitHub;
global using GitHub.Models;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using ProfanityFilter.Action;
global using ProfanityFilter.Action.Clients;
global using ProfanityFilter.Action.Extensions;
global using ProfanityFilter.Action.Models;
global using ProfanityFilter.Common;
global using ProfanityFilter.Services;
global using ContextSummaryPair = (
    ActionsToolkit.Octokit.Context Context,
    ActionsToolkit.Core.Summaries.Summary Summary);
global using Env = System.Environment;
global using IssueUpdate = GitHub.Repos.Item.Item.Issues.Item.WithIssue_numberPatchRequestBody;
global using PullRequestUpdate = GitHub.Repos.Item.Item.Pulls.Item.WithPull_numberPatchRequestBody;
global using ReactionContent = GitHub.Repos.Item.Item.Issues.Item.Reactions.ReactionsPostRequestBody_content;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(
    assemblyName: "ProfanityFilter.Action.Tests")]