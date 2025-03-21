// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

var builder = WebApplication.CreateBuilder(args);

builder.AddProfanityFilterClient("profanity-filter");

builder.Services.AddRedaction(static redaction =>
    redaction.SetRedactor<CharacterRedactor>(
        classifications: [DataClassifications.SensitiveData]));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLocalStorageServices();
builder.Services.AddMemoryCache();

builder.Services.AddSignalR(static options => options.EnableDetailedErrors = true);

builder.Services.AddAntiforgery();
builder.Services.AddDataProtection()
    .UseCryptographicAlgorithms(new()
    {
        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
    })
    .PersistKeysToFileSystem(new DirectoryInfo(@"/var/tmp"));

builder.Services.AddProfanityFilterServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.ConfigureHttpJsonOptions(
    static options => AssignJsonSerializerContext(options.SerializerOptions));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapProfanityFilterEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static void AssignJsonSerializerContext(JsonSerializerOptions options)
{
    options.TypeInfoResolverChain.Insert(0, JsonSerializationContext.Default);
}
