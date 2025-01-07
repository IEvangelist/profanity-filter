// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddProfanityFilterClient("profanity-filter");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.MapPost(
    pattern: "/filter",
    handler: static async (
        [FromBody] ProfanityFilterRequest request,
        [FromServices] IRestClient client) =>
        await client.ApplyFilterAsync(request))
    .WithName("Filter");

app.Run();
