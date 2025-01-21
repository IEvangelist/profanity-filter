# Profanity Filter: Hosting

This is a .NET library intended to be used as a hosting library as part of the .NET Aspire app host project. It exposes the `ProfanityFilterResource` which is a container resource, that resolves to `ghcr.io/ievangelist/profanity-filter-api:latest` container image.

## Get started

To install the [📦 ProfanityFilter.Hosting](https://www.nuget.org/packages/ProfanityFilter.Hosting) NuGet package:

```bash
dotnet add package ProfanityFilter.Hosting
```

To register the profanity filter resource in your application, call `AddProfanityFilter` on an `IDistributedApplication` instance:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var filter = builder.AddProfanityFilter("profanity-filter")
    .WithCustomDataBindMount("./CustomData");

builder.AddProject<Projects.ProfanityFilter_Api>("api")
    .WithReference(filter)
    .WaitFor(filter);

builder.Build().Run();
```

This library pairs nicely with the [📦 ProfanityFilter.Client](https://www.nuget.org/packages/ProfanityFilter.Client) library, which provides a client for the Profanity Filter API.
