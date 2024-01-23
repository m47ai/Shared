namespace M47.Shared.Tests.FixtureUtils;

using AutoFixture.Kernel;
using System.Reflection;

public class StringParameterGenerator : ISpecimenBuilder
{
    private readonly string _name;
    private readonly string _value;

    public StringParameterGenerator(string name, string value)
    {
        _name = name;
        _value = value;
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not ParameterInfo parameterInfo)
        {
            return new NoSpecimen();
        }

        var isReachParam = parameterInfo.Name!.Contains(_name);

        if (isReachParam)
        {
            return _value;
        }

        return new NoSpecimen();
    }
}