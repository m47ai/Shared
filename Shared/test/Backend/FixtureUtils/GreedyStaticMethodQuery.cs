namespace M47.Shared.Tests.FixtureUtils;

using AutoFixture.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class GreedyStaticMethodQuery : IMethodQuery
{
    public IEnumerable<IMethod> SelectMethods(Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        return from methodInfo in type.GetTypeInfo().GetMethods()
               let parameters = methodInfo.GetParameters()
               where parameters.All((ParameterInfo p) => p.ParameterType != type)
               orderby parameters.Length descending
               select (IMethod)new StaticMethod(methodInfo);
    }
}