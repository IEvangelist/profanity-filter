// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using System.Diagnostics.CodeAnalysis;

global using Actions.Core.Extensions;
global using Actions.Core.Services;
global using Actions.Octokit;
global using Actions.Octokit.Extensions;

global using GitHub;
global using GitHub.Models;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;

global using ProfanityFilter.Action;
global using ProfanityFilter.Action.Clients;
global using ProfanityFilter.Action.Extensions;
global using ProfanityFilter.Action.Models;
global using ProfanityFilter.Services;
global using ProfanityFilter.Services.Extensions;

global using Env = System.Environment;

global using IssueUpdate = GitHub.Repos.Item.Item.Issues.Item.WithIssue_numberPatchRequestBody;
global using PullRequestUpdate = GitHub.Repos.Item.Item.Pulls.Item.WithPull_numberPatchRequestBody;
global using ReactionContent = GitHub.Repos.Item.Item.Issues.Item.Reactions.ReactionsPostRequestBody_content;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(
    assemblyName: "ProfanityFilter.Action.Tests")]