// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Reactive.Linq;
global using System.Runtime.CompilerServices;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Timers;

global using Markdig;

global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.DataProtection;
global using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
global using Microsoft.AspNetCore.Http.Connections;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.AspNetCore.HttpLogging;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.ModelBinding;
global using Microsoft.AspNetCore.SignalR;
global using Microsoft.AspNetCore.SignalR.Client;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Compliance.Classification;
global using Microsoft.Extensions.Compliance.Redaction;
global using Microsoft.JSInterop;

global using ProfanityFilter.Services;
global using ProfanityFilter.Services.Filters;
global using ProfanityFilter.Services.Results;
global using ProfanityFilter.WebApi.Compliance;
global using ProfanityFilter.WebApi.Components;
global using ProfanityFilter.WebApi.Endpoints;
global using ProfanityFilter.WebApi.Hubs;
global using ProfanityFilter.WebApi.Models;
global using ProfanityFilter.WebApi.Serialization;

global using SystemTimer = System.Timers.Timer;