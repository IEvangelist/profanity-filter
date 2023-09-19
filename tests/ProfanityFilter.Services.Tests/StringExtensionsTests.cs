// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Tests;

public sealed class StringExtensionsTests
{
    [Fact]
    public void FromJson_ValidJson_ReturnsDeserializedObject()
    {
        // Arrange
        var json = """
            {
              "name": "Ada",
              "age": 36,
              "location": "London, England"
            }
            """;

        // Act
        var ada = json.FromJson<Person>(PersonContext.Default.Person!);

        // Assert
        Assert.Equal("Ada", ada.Name);
        Assert.Equal(36, ada.Age);
        Assert.Equal("London, England", ada.Location);
    }

    [Fact]
    public void FromJson_InvalidJson_ReturnsDefault()
    {
        // Arrange
        var json = "null";

        // Act
        var person = json.FromJson<Person>(PersonContext.Default.Person!);

        // Assert
        Assert.Null(person);
    }

    [Fact]
    public void ToJson_ValidObject_ReturnsJsonString()
    {
        // Arrange
        var ada = new Person
        {
            Name = "Ada",
            Age = 36,
            Location = "London, England"
        };

        static string NormalizeString(string value)
        {
            return value.Replace("\n", "")
                .Replace("\r", "")
                .Replace(Environment.NewLine, "");
        }

        // Act
        var json = ada.ToJson(PersonContext.Default.Person!);

        // Assert
        Assert.Equal(
            expected: NormalizeString("""
            {
              "name": "Ada",
              "age": 36,
              "location": "London, England"
            }
            """),
            actual: NormalizeString(json));
    }

    [Fact]
    public void ToJson_NullObject_ReturnsStringNull()
    {
        // Arrange
        Person? person = null;

        // Act
        var json = person.ToJson(PersonContext.Default.Person!);

        // Assert
        Assert.Equal("null", json);
    }
}

internal sealed class Person
{
    public string Name { get; set; } = default!;
    public int Age { get; set; }
    public string Location { get; set; } = default!;
}


[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(Person))]
internal partial class PersonContext : JsonSerializerContext { }