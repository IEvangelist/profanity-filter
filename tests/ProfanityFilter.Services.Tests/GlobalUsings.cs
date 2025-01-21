// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using System.Collections.Frozen;
global using System.Text.RegularExpressions;

global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging.Abstractions;
global using Microsoft.Extensions.Options;
global using Microsoft.VisualStudio.TestTools.UnitTesting;

global using ProfanityFilter.Common;
global using ProfanityFilter.Services.Extensions;
global using ProfanityFilter.Services.Internals;