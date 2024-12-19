// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.WebApi.Endpoints;

internal static partial class ProfanityFilterEndpoints
{
    internal static WebApplication MapProfanityFilterEndpoints(this WebApplication app)
    {
        var profanity = app.MapGroup("profanity")
            .DisableAntiforgery();

        profanity.MapHub<ProfanityHub>("hub", options =>
            {
                options.AllowStatefulReconnects = true;
                options.Transports =
                    HttpTransportType.WebSockets |
                    HttpTransportType.ServerSentEvents |
                    HttpTransportType.LongPolling;
            })
            // Doesn't actually work, consider AsyncAPI per Safia!
            .WithOpenApi()
            .WithSummary("""
                The profanity filter hub endpoint, used for live bi-directional updates.
                """);

        profanity.MapPost("filter", OnApplyFilterAsync)
            .WithOpenApi()
            .Produces(200, typeof(ProfanityFilterResponse))
            .ProducesValidationProblem()
            .WithRequestTimeout(TimeSpan.FromSeconds(10))
            .WithSummary("""
                Use this endpoint to attempt applying a profanity-filter. The response is returned as Markdown.
                """)
            .WithHttpLogging(HttpLoggingFields.All);

        profanity.MapGet("strategies", OnGetStrategies)
            .WithOpenApi()
            .Produces(200, typeof(StrategyResponse[]))
            .WithRequestTimeout(TimeSpan.FromSeconds(10))
            .CacheOutput()
            .WithSummary("""
                Returns an array of the possible replacement strategies available. See https://github.com/IEvangelist/profanity-filter?tab=readme-ov-file#-replacement-strategies
                """)
            .WithHttpLogging(HttpLoggingFields.All);

        profanity.MapGet("targets", OnGetTargets)
            .WithOpenApi()
            .Produces(200, typeof(FilterTargetResponse[]))
            .WithRequestTimeout(TimeSpan.FromSeconds(10))
            .CacheOutput()
            .WithSummary("""
                        Returns an array of the possible filter targets available.
                        """)
            .WithHttpLogging(HttpLoggingFields.All);

        var data = profanity.MapGroup("data")
            .DisableAntiforgery();

        data.MapGet("", OnGetDataNamesAsync)
            .WithOpenApi()
            .Produces(200, typeof(string[]))
            .WithRequestTimeout(TimeSpan.FromSeconds(10))
            .CacheOutput()
            .WithSummary("""
                Returns an array of the data names.
                """)
            .WithHttpLogging(HttpLoggingFields.All);

        data.MapGet("{name}", OnGetDataByNameAsync)
            .WithOpenApi()
            .Produces(200, typeof(string[]))
            .WithRequestTimeout(TimeSpan.FromSeconds(10))
            .CacheOutput()
            .WithSummary("""
                Returns an array of the profane words for a given data name.
                """)
            .WithHttpLogging(HttpLoggingFields.All);

        return app;
    }

    private static async Task<IResult> OnApplyFilterAsync(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] ProfanityFilterRequest request,
        [FromServices] IProfaneContentFilterService filterService)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Text))
        {
            return Results.BadRequest("""
                You need to provide a valid request, as example HTTP POST body:
                {
                    "text": "Some content to evaluate.",
                    "strategy": "RedactedRectangle"
                }
                """);
        }

        var parameters = new FilterParameters(
            request.Strategy, FilterTarget.Body);

        var filterResult =
            await filterService.FilterProfanityAsync(request.Text, parameters);

        var response = new ProfanityFilterResponse(
            ContainsProfanity: filterResult.IsFiltered,
            InputText: filterResult.Input ?? "",
            FilteredText: filterResult.FinalOutput,
            ReplacementStrategy: request.Strategy,
            FiltrationSteps: [.. filterResult.Steps?.Where(static s => s.IsFiltered) ?? []],
            Matches: [.. filterResult.Matches ?? []]
        );

        return TypedResults.Json(
            response,
            JsonSerializationContext.Default.ProfanityFilterResponse);
    }

    private static JsonHttpResult<StrategyResponse[]> OnGetStrategies() =>
        TypedResults.Json([
                .. Enum.GetValues<ReplacementStrategy>()
            ],
            JsonSerializationContext.Default.StrategyResponseArray
        );

    private static JsonHttpResult<FilterTargetResponse[]> OnGetTargets() =>
        TypedResults.Json([
                .. Enum.GetValues<FilterTarget>()
            ],
            JsonSerializationContext.Default.FilterTargetResponseArray
        );

    private static async Task<IResult> OnGetDataNamesAsync(
        [FromServices] IMemoryCache cache,
        [FromServices] IProfaneContentFilterService filterService)
    {
        var map = await filterService.ReadAllProfaneWordsAsync();

        var fileNames = await GetProfaneContentNamesAsync(cache, map.Keys);

        return TypedResults.Json([
                .. fileNames.Keys
            ],
            JsonSerializationContext.Default.StringArray);
    }

    private static async Task<IResult> OnGetDataByNameAsync(
        [FromRoute] string name,
        [FromServices] IMemoryCache cache,
        [FromServices] IProfaneContentFilterService filterService)
    {
        var map = await filterService.ReadAllProfaneWordsAsync();

        var fileNames = await GetProfaneContentNamesAsync(cache, map.Keys);

        foreach (var (key, value) in map)
        {
            if (fileNames.TryGetValue(key, out var fileName) && fileName == name)
            {
                return TypedResults.Json([
                        .. value.ProfaneWords
                    ],
                    JsonSerializationContext.Default.StringArray);
            }
        }

        return TypedResults.NotFound();
    }
}
