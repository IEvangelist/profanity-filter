// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();
builder.Services.AddProfanityFilterServices();

builder.Services.ConfigureHttpJsonOptions(
    static options =>
    options.SerializerOptions.TypeInfoResolverChain.Insert(
        0,
        SourceGenerationContext.Default));

builder.Services.Configure<JsonHubProtocolOptions>(
    static options =>
    options.PayloadSerializerOptions.TypeInfoResolverChain.Insert(
        0, SourceGenerationContext.Default));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapProfanityFilterEndpoints();

app.Run();
