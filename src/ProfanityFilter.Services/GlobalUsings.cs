// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using System.Collections.Concurrent;
global using System.Collections.Frozen;
global using System.Diagnostics.CodeAnalysis;
global using System.Text;
global using System.Text.RegularExpressions;

global using Microsoft.Extensions.DependencyInjection;

global using Nito.AsyncEx;

global using Pathological.Globbing.Extensions;
global using Pathological.Globbing.Options;

global using ProfanityFilter.Services.Extensions;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(
    assemblyName: "ProfanityFilter.Services.Tests")]

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(
    assemblyName: "ProfanityFilter.Action.Tests")]
