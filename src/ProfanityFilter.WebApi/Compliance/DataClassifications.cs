// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace ProfanityFilter.WebApi.Compliance;

internal static class DataClassifications
{
    public static readonly string Taxonomy = typeof(DataClassifications).FullName!;

    public static DataClassification SensitiveData = new(
        taxonomyName: Taxonomy,
        value: nameof(SensitiveData));
}
