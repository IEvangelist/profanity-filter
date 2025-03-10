﻿// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using System.Collections.Concurrent;
global using System.Collections.Frozen;
global using System.Text;
global using System.Text.RegularExpressions;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Logging;
global using Pathological.Globbing.Extensions;
global using Pathological.Globbing.Options;
global using ProfanityFilter.Common;
global using ProfanityFilter.Services;
global using ProfanityFilter.Services.Data;
global using ProfanityFilter.Services.Extensions;
global using ProfanityFilter.Services.Filters;
global using ProfanityFilter.Services.Internals;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(
    assemblyName: "ProfanityFilter.Services.Tests")]

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(
    assemblyName: "ProfanityFilter.Action.Tests")]
