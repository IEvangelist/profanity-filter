// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using System.Diagnostics.CodeAnalysis;

global using System.Text.Json;
global using System.Text.Json.Serialization;

global using Microsoft.AspNetCore.HttpLogging;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.ModelBinding;

global using ProfanityFilter.Services;
global using ProfanityFilter.Services.Extensions;
global using ProfanityFilter.Services.Filters;
global using ProfanityFilter.Services.Results;

global using ProfanityFilter.WebApi.Endpoints;
global using ProfanityFilter.WebApi.Models;
global using ProfanityFilter.WebApi.Serialization;
