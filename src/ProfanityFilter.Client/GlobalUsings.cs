// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using System;
global using System.Collections.Frozen;
global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics.CodeAnalysis;
global using System.Net.Http.Json;
global using System.Runtime.CompilerServices;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.Json.Serialization.Metadata;
global using System.Threading.Channels;

global using Microsoft.AspNetCore.SignalR.Client;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Hosting;

global using ProfanityFilter.Client;
global using ProfanityFilter.Client.Options;
global using ProfanityFilter.Shared;
global using ProfanityFilter.Shared.Api;
global using ProfanityFilter.Shared.Extensions;
global using ProfanityFilter.Shared.Optional;
