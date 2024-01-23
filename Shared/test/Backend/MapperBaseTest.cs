namespace M47.Shared.Tests;

using AutoMapper;

public abstract class MapperBaseTest
{
    protected MapperBaseTest()
    {
    }

    public static void AssertConfigurationIsValid<T>() where T : Profile, new()
    {
        var mapping = new MapperConfiguration(x => x.AddProfile<T>());
        mapping.AssertConfigurationIsValid();
    }
}