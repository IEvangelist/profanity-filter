# Profanity Filter: Hosting

This .NET library adds the Potty Mouth profanity-filter API to an Aspire AppHost as a first-class `ContainerResource`. The default container image is `ghcr.io/ievangelist/profanity-filter-api:13.4.0`.

The integration is built for Aspire 13.4 and uses analyzer-validated `[AspireExport]` metadata, so the same package works from C# AppHosts and generated TypeScript AppHost SDKs.

## Get started

To install the [ProfanityFilter.Hosting](https://www.nuget.org/packages/ProfanityFilter.Hosting) NuGet package:

```bash
aspire add ProfanityFilter.Hosting
```

## C# AppHost

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var filter = builder.AddProfanityFilter("profanity-filter")
    .WithCustomDataBindMount("./CustomData");

builder.AddProject<Projects.MyApi>("api")
    .WithReference(filter)
    .WaitFor(filter);

builder.Build().Run();
```

## TypeScript AppHost

Use `apphost.mts` with the Aspire 13.4 generated SDK under `.aspire/modules`:

```ts
import { createBuilder } from "./.aspire/modules/aspire.mjs";

const builder = await createBuilder();

const filter = await builder
    .addProfanityFilter("profanity-filter")
    .withCustomDataBindMount("./CustomData");

await builder.addProject("api", { project: "../src/MyApi/MyApi.csproj" })
    .withReference(filter)
    .waitFor(filter);

const app = await builder.build();
await app.run();
```

Do not edit files under `.aspire/modules`; Aspire regenerates them when packages are added or restored.

## Exported API

| C# API | TypeScript API | Description |
| --- | --- | --- |
| `AddProfanityFilter(name)` | `builder.addProfanityFilter(name)` | Adds the profanity-filter API container to the application model. |
| `WithCustomDataBindMount(source)` | `.withCustomDataBindMount(source)` | Bind-mounts newline-delimited `*.txt` word lists into `/app/CustomData`. |
| `ProfanityFilterResource.HttpsEndpoint` | `httpsEndpoint()` | Exposes the HTTPS endpoint used by the generated connection string. |

This library pairs with [ProfanityFilter.Client](https://www.nuget.org/packages/ProfanityFilter.Client), which registers typed REST and SignalR clients that consume the resource connection string.
