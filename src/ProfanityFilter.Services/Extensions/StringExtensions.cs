// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.Services.Extensions;

internal static class StringExtensions
{
    /// <summary>
    /// Deserializes the JSON string to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the JSON string to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="typeInfo">The <see cref="JsonTypeInfo"/>
    /// instance to use when deserializing.</param>
    /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
    internal static T FromJson<T>(this string json, JsonTypeInfo<T> typeInfo)
    {
        try
        {
            if (JsonSerializer.Deserialize<T>(json, typeInfo) is T value)
            {
                Debug.WriteLine($"""
                    Successfully deserialized JSON to {typeof(T).Name}:
                    """);

                return value;
            }
            else
            {

                Debug.WriteLine($"""
                    Unable to deserialize JSON to {typeof(T).Name}:
                      Given: {json}
                    """);

                return default!;
            }
        }
        catch (Exception ex) when (Debugger.IsAttached)
        {
            _ = ex;

            Debug.WriteLine($"""
                Unable to deserialize JSON to {typeof(T).Name}:
                  Given: {json}
                """);

            Debugger.Break();

            return default!;
        }
    }

    /// <summary>
    /// Converts the specified object to its equivalent JSON representation.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <param name="typeInfo">The <see cref="JsonTypeInfo"/>
    /// instance to use when deserializing.</param>
    /// <returns>A JSON string representation of the object.</returns>
    internal static string ToJson<T>(this T value, JsonTypeInfo<T> typeInfo)
    {
        try
        {
            var json = JsonSerializer.Serialize(value, typeInfo);

            Debug.WriteLine($"""
                Successfully serialized {typeof(T).Name} to JSON:
                """);

            return json;
        }
        catch (Exception ex) when (Debugger.IsAttached)
        {
            _ = ex;

            Debug.WriteLine($"""
                Unable to serialize {typeof(T).Name} to JSON:
                  Given: {value}
                """);

            Debugger.Break();

            return "";
        }
    }
}
