﻿// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Reactive.Linq;
global using System.Runtime.CompilerServices;
global using System.Timers;

global using Markdig;

global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.DataProtection;
global using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
global using Microsoft.AspNetCore.Hosting.Server;
global using Microsoft.AspNetCore.Hosting.Server.Features;
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
global using ProfanityFilter.Shared;
global using ProfanityFilter.Shared.Api;
global using ProfanityFilter.WebApi.Compliance;
global using ProfanityFilter.WebApi.Components;
global using ProfanityFilter.WebApi.Endpoints;
global using ProfanityFilter.WebApi.Hubs;
global using ProfanityFilter.WebApi.Services;

global using SystemTimer = System.Timers.Timer;