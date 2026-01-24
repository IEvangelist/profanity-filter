// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

#:sdk Aspire.AppHost.Sdk@13.1.0
#:package Aspire.Hosting.Docker
#:package Aspire.Hosting.JavaScript
#:package ProfanityFilter.Hosting
#:project ../src/ProfanityFilter.WebApi/ProfanityFilter.WebApi.csproj
#:property NoWarn=$(NoWarn);ASPIREPUBLISHERS001;ASPIRECOMPUTE001;ASPIREPIPELINES003;ASPIRECOMPUTE003;ASPIRECERTIFICATES001;

using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

_ = builder.AddDockerComposeEnvironment("compose");

var releaseVersion = builder.AddParameterFromConfiguration(
    "release-version", "RELEASE_VERSION");

var imageName = builder.AddParameterFromConfiguration(
    "image-name", "IMAGE_NAME");

var ghco = builder.AddContainerRegistry("github-container-registry", "ghcr.io");

var webApi = builder.AddProject<Projects.ProfanityFilter_WebApi>("webapi")
    .WithExternalHttpEndpoints()
    .WithEnvironment("PROFANITY_FILTER_CUSTOM_DATA_PATH", Path.GetFullPath("CustomData"))
    .WithContainerRegistry(ghco)
    .WithImagePushOptions(async context =>
    {
        context.Options.RemoteImageTag = await releaseVersion.Resource.GetValueAsync(context.CancellationToken)
            ?? "latest";
        context.Options.RemoteImageName = await imageName.Resource.GetValueAsync(context.CancellationToken)
            ?? "ievangelist/profanity-filter-api";
    })
    .WithUrlForEndpoint("https", url =>
    {
        url.DisplayText = "Scalar (HTTPS)";
        url.Url = "/scalar/v1";
    });

// Web app connects to WebApi which handles the profanity filtering
var web = builder.AddViteApp("web", "../src/ProfanityFilter.Web")
    .WithEnvironment("API_HTTPS", webApi.GetEndpoint("https"))
    .WithReference(webApi)
    .WaitFor(webApi);

webApi.PublishWithContainerFiles(web, "wwwroot");

// For local consumers, add a profanity filter resource
if (builder.Environment.IsDevelopment())
{
    _ = builder.AddProfanityFilter("profanity-filter")
        .WithHttpsDeveloperCertificate()
        .WithCustomDataBindMount("./CustomData");

    _ = builder.AddProfanityFilter("priview-filter")
        .WithImageTag("10.0.1-preview.011")
        .WithHttpsDeveloperCertificate()
        .WithCustomDataBindMount("./CustomData");
}

builder.Build().Run();
