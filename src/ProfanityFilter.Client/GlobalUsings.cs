﻿// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using System.Diagnostics.CodeAnalysis;
global using System.Net.Http.Json;
global using System.Text.Json.Serialization.Metadata;

global using Microsoft.AspNetCore.SignalR.Client;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

global using ProfanityFilter.Client;
global using ProfanityFilter.Client.Options;
global using ProfanityFilter.Common;
global using ProfanityFilter.Common.Api;
global using ProfanityFilter.Common.Extensions;
global using ProfanityFilter.Common.Monads;
