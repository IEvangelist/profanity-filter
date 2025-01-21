// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

var builder = DistributedApplication.CreateBuilder(args);

var filter = builder.AddProfanityFilter("profanity-filter")
    .WithCustomDataBindMount("./CustomData")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithImageTag("latest");

builder.AddProject<Projects.ProfanityFilter_Api>("api")
    .WithReference(filter)
    .WaitFor(filter);

builder.Build().Run();
