// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using Actions.Core.Extensions;
global using Actions.Core.Services;

global using Actions.Octokit;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;

global using Newtonsoft.Json;
global using Newtonsoft.Json.Serialization;

global using Octokit;
global using Octokit.GraphQL;
global using Octokit.GraphQL.Model;

global using ProfanityFilter.Action;
global using ProfanityFilter.Action.Clients;
global using ProfanityFilter.Action.Extensions;
global using ProfanityFilter.Action.Models;

global using ProfanityFilter.Services;
global using ProfanityFilter.Services.Extensions;

global using Env = System.Environment;

global using RestConnection = Octokit.Connection;
global using GraphQLConnection = Octokit.GraphQL.Connection;
global using GraphQLLabel = Octokit.GraphQL.Model.Label;
global using RestProductHeaderValue = Octokit.ProductHeaderValue;
global using GraphQLProductHeaderValue = Octokit.GraphQL.ProductHeaderValue;

global using RepoConfig = (string Owner, string Repo, string Token);

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(
    assemblyName: "ProfanityFilter.Action.Tests")]