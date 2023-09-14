// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using Actions.Core.Extensions;
global using Actions.Core.Services;

global using Actions.Octokit;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;

global using Octokit;
global using Octokit.GraphQL;
global using Octokit.GraphQL.Model;

global using ProfanityFilter.Action;
global using ProfanityFilter.Action.Clients;
global using ProfanityFilter.Action.Extensions;

global using ProfanityFilter.Services;
global using ProfanityFilter.Services.Extensions;

global using GraphQLConnection = Octokit.GraphQL.Connection;
global using IGraphQLConnection = Octokit.GraphQL.IConnection;
global using GraphQLLabel = Octokit.GraphQL.Model.Label;
global using GraphQLIssue = Octokit.GraphQL.Model.Issue;
global using GraphQLPullRequest = Octokit.GraphQL.Model.PullRequest;
global using GraphQLProductHeaderValue = Octokit.GraphQL.ProductHeaderValue;
