// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

var builder = await TestApplication.CreateBuilderAsync(args);

// Instead of manually referencing the extensions, we are using the MTP MSBuild feature that
// auto-detects extensions exposing hooks and hide them all under this extension method.
builder.AddSelfRegisteredExtensions(args);

var app = await builder.BuildAsync();

return await app.RunAsync();