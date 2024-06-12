// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.WebApi.Endpoints;

internal static class ProfanityFilterEndpointExtensions
{
    internal static WebApplication MapProfanityFilterEndpoints(this WebApplication app)
    {
        var profanity = app.MapGroup("profanity");

        profanity.MapPost("filter", OnApplyFilterAsync)
            .WithOpenApi()
            .WithRequestTimeout(TimeSpan.FromSeconds(10))
            .WithSummary("""
                Use this endpoint to attempt applying a profanity-filter. The response is returned as Markdown.
                """)
            .WithHttpLogging(HttpLoggingFields.All);

        profanity.MapGet("strategies", OnGetStrategies)
            .WithOpenApi()
            .WithRequestTimeout(TimeSpan.FromSeconds(10))
            .CacheOutput()
            .WithSummary("""
                Returns an array of the possible replacement strategies available. See https://github.com/IEvangelist/profanity-filter?tab=readme-ov-file#-replacement-strategies
                """)
            .WithHttpLogging(HttpLoggingFields.All);

        var data = profanity.MapGroup("data");

        data.MapGet("names", OnGetDataNamesAsync)
            .WithOpenApi()
            .WithRequestTimeout(TimeSpan.FromSeconds(10))
            .CacheOutput()
            .WithSummary("""
                Returns an array of the data names.
                """)
            .WithHttpLogging(HttpLoggingFields.All);

        data.MapGet("{name}", OnGetDataByNameAsync)
            .WithOpenApi()
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
            InputText: filterResult.Input,
            FilteredText: filterResult.FinalOutput,
            ReplacementStrategy: request.Strategy,
            FiltrationSteps: [.. filterResult.Steps?.Where(static s => s.IsFiltered)],
            Matches: [.. filterResult.Matches]
        );

        return TypedResults.Json(
            response,
            SourceGenerationContext.Default.ProfanityFilterResponse);
    }

    private static IResult OnGetStrategies() =>
        TypedResults.Json([
                .. Enum.GetValues<ReplacementStrategy>()
            ],
            SourceGenerationContext.Default.StrategyResponseArray
        );

    private static async Task<IResult> OnGetDataNamesAsync(
        [FromServices] IProfaneContentFilterService filterService)
    {
        var map = await filterService.ReadAllProfaneWordsAsync();

        return TypedResults.Json([
                .. map.Keys.Select(static key => Path.GetFileNameWithoutExtension(key))
            ],
            SourceGenerationContext.Default.StringArray);
    }

    private static async Task<IResult> OnGetDataByNameAsync(
        [FromRoute] string name,
        [FromServices] IProfaneContentFilterService filterService)
    {
        var map = await filterService.ReadAllProfaneWordsAsync();

        foreach (var (key, value) in map)
        {
            if (Path.GetFileNameWithoutExtension(key) == name)
            {
                return TypedResults.Json([
                        .. value.ProfaneWords
                    ],
                    SourceGenerationContext.Default.StringArray);
            }
        }

        return TypedResults.NotFound();
    }
}
