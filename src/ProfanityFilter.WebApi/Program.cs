// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR(
        static options => options.EnableDetailedErrors = true)
    .AddMessagePackProtocol();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/var/tmp"));
builder.Services.AddAntiforgery();

builder.Services.AddProfanityFilterServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.ConfigureHttpJsonOptions(
    static options =>
    options.SerializerOptions.TypeInfoResolverChain.Insert(
        0,
        SourceGenerationContext.Default));

var app = builder.Build();

app.UseAntiforgery(); 

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapProfanityFilterEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
