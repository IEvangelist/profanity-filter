// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddGitHubActionsCore();
builder.Services.AddProfanityFilter();
builder.Services.AddOctokitServices();
builder.Services.AddSingleton<ActionProcessor>();

var app = builder.Build();

var processor = app.Services.GetRequiredService<ActionProcessor>();

await processor.ProcessAsync();
await app.RunAsync();