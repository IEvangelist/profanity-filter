// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using System.Runtime.CompilerServices;
global using System.Text.Json;
global using Microsoft.AspNetCore.Http.Connections;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.AspNetCore.HttpLogging;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.ModelBinding;
global using Microsoft.AspNetCore.SignalR;
global using Scalar.AspNetCore;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Compliance.Classification;
global using Microsoft.Extensions.Compliance.Redaction;
global using ProfanityFilter.Client;
global using ProfanityFilter.Common;
global using ProfanityFilter.Common.Api;
global using ProfanityFilter.Services;
global using ProfanityFilter.WebApi.Compliance;
global using ProfanityFilter.WebApi.Endpoints;
global using ProfanityFilter.WebApi.Hubs;
global using SystemTimer = System.Timers.Timer;