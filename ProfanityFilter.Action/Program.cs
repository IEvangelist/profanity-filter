// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddGitHubActionsCore();
builder.Services.AddProfanityFilter();

var app = builder.Build();

// TODO: Dispatch work from invocation context...

app.Run();