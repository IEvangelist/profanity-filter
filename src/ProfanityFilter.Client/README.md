# Profanity Filter: Client

This is a .NET library intended to be used as a client for the Profanity Filter API. It provides a pair of clients for both REST and realtime communication with the API. The library is built on top of the [📦 Microsoft.Extensions.Http](https://www.nuget.org/packages/Microsoft.Extensions.Http) and [📦 Microsoft.AspNetCore.SignalR.Client](https://www.nuget.org/packages/Microsoft.AspNetCore.SignalR.Client) packages.

The Profanity Filter API is a service that filters profane language from text. This client maps to the:

- `ghcr.io/ievangelist/profanity-filter-api:latest` container image.

## Get started

To install the [📦 ProfanityFilter.Client](https://www.nuget.org/packages/ProfanityFilter.Client) NuGet package:

```bash
dotnet add package ProfanityFilter.Client
```

To register the client in your application, call `AddProfanityFilterClient` on an `IHostApplicationBuilder` instance:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddProfanityFilterClient("profanity-filter");

var app = builder.Build();

app.MapPost(
    pattern: "/filter",
    handler: static async (
        [FromBody] ProfanityFilterRequest request,
        [FromServices] IRestClient client) =>
        await client.ApplyFilterAsync(request))
    .WithName("Filter");

app.Run();
```

The connection name parameter registers the client in the application's services collection. The client can be injected into any service that requires it. The following types are available via DI:

- `IRestClient`: Interface for the REST client.
- `IRealtimeClient`: Interface for the realtime client.
- `ProfanityFilterClient`: Concrete type exposing the pair of REST and realtime clients.

## Configuration

The client can be configured using the `ProfanityFilterOptions` class (in the `ProfanityFilter` configuration key). The following options are available:

- `ApiBaseAddress`: The base address of the Profanity Filter API.

```json
{
  "ProfanityFilter": {
    "ApiBaseAddress": "https://profanity-filter-api.azurewebsites.net"
  }
}
```
