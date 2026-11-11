# Profanity Filter: Client

This is a .NET library intended to be used as a client for the Profanity Filter API. It provides a pair of clients for both REST and realtime communication with the API. The library is built on top of the [📦 Microsoft.Extensions.Http](https://www.nuget.org/packages/Microsoft.Extensions.Http) and [📦 Microsoft.AspNetCore.SignalR.Client](https://www.nuget.org/packages/Microsoft.AspNetCore.SignalR.Client) packages.

The Profanity Filter API is a service that filters profane language from text. This client maps to the:

- `ghcr.io/ievangelist/profanity-filter-api:13.4.0` container image.

## Get started

To install the [📦 ProfanityFilter.Client](https://www.nuget.org/packages/ProfanityFilter.Client) NuGet package:

```bash
dotnet add package ProfanityFilter.Client
```

To register the client in your application, call `AddProfanityFilterClient` on an `IHostApplicationBuilder` instance:

```csharp
using Microsoft.AspNetCore.Mvc;
using ProfanityFilter.Client;
using ProfanityFilter.Common.Api;
using ProfanityFilter.Common.Monads;

var builder = WebApplication.CreateBuilder(args);

builder.AddProfanityFilterClient("profanity-filter");

var app = builder.Build();

app.MapPost(
    pattern: "/filter",
    handler: static async (
        [FromBody] ProfanityFilterRequest request,
        [FromServices] IRestClient client) =>
        (await client.ApplyFilterAsync(request)) switch
        {
            Available<ProfanityFilterResponse>(var response) => Results.Ok(response),
            Absent<ProfanityFilterResponse> => Results.NoContent()
        })
    .WithName("Filter");

app.Run();
```

The connection name parameter registers the client in the application's services collection. The client can be injected into any service that requires it. The following types are available via DI:

- `IRestClient`: Interface for the REST client.
- `IRealtimeClient`: Interface for the realtime client.
- `ProfanityFilterClient`: Concrete type exposing the pair of REST and realtime clients.

## Optional REST results

REST methods return `Task<Maybe<T>>`. `Maybe<T>` has two cases:
`Available<T>` contains a value, and `Absent<T>` represents no value.
A JSON `null` response becomes `Absent<T>`; an empty array is still available.
HTTP and deserialization errors throw exceptions rather than returning an absent
result. Use pattern matching to handle both cases:

```csharp
Maybe<string[]> result = await client.GetDataNamesAsync();

string[] names = result switch
{
    Available<string[]>(var values) => values,
    Absent<string[]> => []
};
```

Use `AsMaybe()` to wrap a value, or `Chain()` to transform an available value.
These extension methods are in the `ProfanityFilter.Common.Extensions` namespace:

```csharp
using ProfanityFilter.Common.Extensions;

string? dataName = null;
Maybe<string> selectedDataName = dataName.AsMaybe(); // Absent<string>
Maybe<int> count = result.Chain(static values => values.Length);
```

`AsMaybe()` maps null to `Absent<T>`. `Chain()` runs its callback only for an
available value and propagates absence otherwise. `default(Maybe<T>)` is absent;
an explicitly constructed `Available<T>` can contain null, as can the result of
`Chain()` when its callback returns null. Unwrap a result before returning it
from an HTTP endpoint, as shown in the example above.

## Configuration

The client can be configured using the `ProfanityFilterOptions` class (in the `ProfanityFilter` configuration key). The following options are available:

- `ApiBaseAddress`: The base address of the Profanity Filter API.

HTTPS connections validate the API's server certificate, including when the client runs in a container. For development or private certificate authorities, install the issuing CA certificate in the client's OS or container trust store rather than disabling certificate validation.

```json
{
  "ProfanityFilter": {
    "ApiBaseAddress": "https://profanity-filter-api.azurewebsites.net"
  }
}
```
