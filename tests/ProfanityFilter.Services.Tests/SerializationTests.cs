// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProfanityFilter.Services.Tests;

public class SerializationTests
{
    [Fact]
    public void Test()
    {
        var json = """
            {
                "title": "Yeah, I expected this to come through - but it doesn't on the .Title prop :(",
                "test": {
                    "what": "about this",
                    "and": false
                }
            }
            """;

        var example = JsonSerializer.Deserialize<Example>(json);
        Assert.NotNull(example);

        var title = example!.Title ?? (example!["title"].ToString());
        Assert.NotNull(title);

        var test = example!["test"];
        Assert.NotNull(test);
        Assert.IsType<JsonElement>(test);

        var element = (JsonElement)test;
        Assert.True(element.TryGetProperty("what", out var what));
        Assert.Equal("about this", what.GetString());
    }
}

public sealed class Example : Dictionary<string, object>
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = "";
}