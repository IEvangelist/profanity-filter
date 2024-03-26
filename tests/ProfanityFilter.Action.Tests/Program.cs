// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

var builder = await TestApplication.CreateBuilderAsync(args);

// Registers TestFramework, with tree of test nodes 
// that are generated into your project by source generator.
builder.AddTestFramework(new SourceGeneratedTestNodesBuilder());

var app = await builder.BuildAsync();

return await app.RunAsync();