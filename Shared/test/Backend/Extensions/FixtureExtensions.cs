namespace M47.Shared.Tests.Extensions;

using AutoFixture.Kernel;
using M47.Shared.Tests.FixtureUtils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

public static class FixtureExtensions
{
    public static FixtureCustomization<T> For<T>(this Fixture fixture) => new(fixture);
}

internal partial class OverridePropertyBuilder<T, TProp> : ISpecimenBuilder
{
    private readonly PropertyInfo _propertyInfo;
    private readonly TProp _value;

    public OverridePropertyBuilder(Expression<Func<T, TProp>> expr, TProp value)
    {
        _propertyInfo = (expr.Body as MemberExpression)?.Member as PropertyInfo ??
                        throw new InvalidOperationException("invalid property expression");
        _value = value;
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not ParameterInfo pi)
        {
            return new NoSpecimen();
        }

        var camelCase = MyRegex().Replace(_propertyInfo.Name, m => m.Groups[1].Value.ToLower() + m.Groups[2]);

        if (pi.ParameterType != typeof(TProp) || pi.Name != camelCase)
        {
            return new NoSpecimen();
        }

        return _value!;
    }

    [GeneratedRegex("(\\w)(.*)")]
    private static partial Regex MyRegex();
}

public class FixtureCustomization<T>
{
    public Fixture Fixture { get; }

    public FixtureCustomization(Fixture fixture)
    {
        Fixture = fixture;
    }

    public FixtureCustomization<T> With<TProp>(Expression<Func<T, TProp>> expr, TProp value)
    {
        Fixture.Customizations.Add(new OverridePropertyBuilder<T, TProp>(expr, value));

        return this;
    }

    /// <summary>
    /// Customized to invoke greedy static method of the given class
    /// </summary>
    /// <typeparam name="TProp"></typeparam>
    /// <param name="expr"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public FixtureCustomization<T> WithStatic<TProp>(Expression<Func<T, TProp>> expr, TProp value)
    {
        Fixture.Customize<T>(c => c.FromFactory(new AutoFixture.Kernel.MethodInvoker(new GreedyStaticMethodQuery())));
        Fixture.Customizations.Add(new OverridePropertyBuilder<T, TProp>(expr, value));

        return this;
    }

    public FixtureCustomization<T> WithStatic(ISpecimenBuilder specimen)
    {
        Fixture.Customize<T>(c => c.FromFactory(new AutoFixture.Kernel.MethodInvoker(new GreedyStaticMethodQuery())));
        Fixture.Customizations.Add(specimen);

        return this;
    }

    public T Create() => Fixture.Create<T>();

    public IEnumerable<T> CreateMany(int count) => Fixture.CreateMany<T>(count);

    public IEnumerable<T> CreateMany() => Fixture.CreateMany<T>();
}