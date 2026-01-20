// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

var builder = WebApplication.CreateBuilder(args);

builder.AddProfanityFilterClient("profanity-filter");

builder.Services.AddRedaction(static redaction =>
    redaction.SetRedactor<CharacterRedactor>(
        classifications: [DataClassifications.SensitiveData]));

builder.Services.AddOpenApi();

builder.Services.AddMemoryCache();

builder.Services.AddSignalR(static options => options.EnableDetailedErrors = true)
    .AddJsonProtocol(static options => AssignJsonSerializerContext(options.PayloadSerializerOptions));

builder.Services.AddProfanityFilterServices();

builder.Services.ConfigureHttpJsonOptions(
    static options => AssignJsonSerializerContext(options.SerializerOptions));

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.MapProfanityFilterEndpoints();
app.MapFallbackToFile("index.html");

app.Run();

static void AssignJsonSerializerContext(JsonSerializerOptions options)
{
    options.TypeInfoResolverChain.Insert(0, JsonSerializationContext.Default);
}
