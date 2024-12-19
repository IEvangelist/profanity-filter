// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

var builder = DistributedApplication.CreateBuilder(args);

_ = builder.AddProfanityFilter("profanity-filter")
    .WithDataBindMount("./CustomData");

builder.Build().Run();
