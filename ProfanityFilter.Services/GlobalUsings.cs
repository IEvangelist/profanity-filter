// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using System.Diagnostics;
global using System.Collections.Concurrent;
global using System.Text.RegularExpressions;

global using Microsoft.Extensions.DependencyInjection;

global using Nito.AsyncEx;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(
    assemblyName: "ProfanityFilter.Services.Tests")]