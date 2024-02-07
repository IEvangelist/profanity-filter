// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddActionProcessorServices();

var app = builder.Build();

var processor = app.Services.GetRequiredService<ProfanityProcessor>();
var context = app.Services.GetRequiredService<Context>();

await processor.ProcessProfanityAsync(context);

await app.RunAsync();