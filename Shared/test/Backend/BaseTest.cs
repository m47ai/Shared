namespace M47.Shared.Tests;

using AutoMapper;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using System;

public abstract class BaseTest
{
    protected static IMapper InitMapper(params Type[] types)
        => new MapperConfiguration(cfg => cfg.AddMaps(types)).CreateMapper();

    protected static void ShouldJsonBeEquivalent(string actualJson, string expectedJson)
    {
        var expected = JToken.Parse(expectedJson);
        var actual = JToken.Parse(actualJson);

        actual.Should().BeEquivalentTo(expected);
    }

    protected static void ShouldJsonMatch(string actualJson, string expectedJson)
    {
        var expected = JToken.Parse(expectedJson);
        var actual = JToken.Parse(actualJson);

        actual.ToString().Should().Match(expected.ToString());
    }
}