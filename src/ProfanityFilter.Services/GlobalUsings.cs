// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using System.Collections.Concurrent;
global using System.Collections.Frozen;
global using System.Text;
global using System.Text.RegularExpressions;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.FileSystemGlobbing;

global using Nito.AsyncEx;

global using ProfanityFilter.Services.Extensions;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(
    assemblyName: "ProfanityFilter.Services.Tests")]

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(
    assemblyName: "ProfanityFilter.Action.Tests")]
