// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddGitHubActionsCore();
builder.Services.AddProfanityFilter();
builder.Services.AddOctokitServices();

var app = builder.Build();

// TODO: get an action processor and run it to apply the profanity filter.

app.Run();