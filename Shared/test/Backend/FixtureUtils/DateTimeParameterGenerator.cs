namespace M47.Shared.Tests.FixtureUtils;

using AutoFixture.Kernel;
using System;
using System.Reflection;

public class DateTimeParameterGenerator : ISpecimenBuilder
{
    private readonly string _name;
    private readonly DateTime _value;

    public DateTimeParameterGenerator(string name, DateTime value)
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
        var isDateTime = parameterInfo.Name!.Contains(_name);

        if (isDateTime)
        {
            return _value;
        }

        return new NoSpecimen();
    }
}