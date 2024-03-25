// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Testing.Framework;
using Microsoft.Testing.Platform.Builder;

namespace ProfanityFilter.Action.Tests;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var builder = await TestApplication.CreateBuilderAsync(args);
        // Registers TestFramework, with tree of test nodes 
        // that are generated into your project by source generator.
        builder.AddTestFramework(new SourceGeneratedTestNodesBuilder());            
        var app = await builder.BuildAsync();
        return await app.RunAsync();
    }
}