// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

#:sdk Aspire.AppHost.Sdk@13.1.0
#:package Aspire.Hosting.JavaScript
#:project ../src/ProfanityFilter.WebApi/ProfanityFilter.WebApi.csproj

var builder = DistributedApplication.CreateBuilder(args);

var webApi = builder.AddProject<Projects.ProfanityFilter_WebApi>("webapi")
    .WithExternalHttpEndpoints();

// Web app connects to WebApi which handles the profanity filtering
var web = builder.AddViteApp("web", "../src/ProfanityFilter.Web")
    .WithEnvironment("API_HTTPS", webApi.GetEndpoint("https"))
    .WithReference(webApi)
    .WaitFor(webApi);

webApi.PublishWithContainerFiles(web, "wwwroot");

builder.Build().Run();
