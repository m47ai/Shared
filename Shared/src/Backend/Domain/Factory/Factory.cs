namespace M47.Shared.Domain.Factory;

using System;
using System.Collections.Generic;

public static class Factory<T>
{
    private static readonly Dictionary<Type, Func<T>> _dict = new();

    public static T Create()
    {
        if (_dict.TryGetValue(typeof(T), out Func<T>? constructor))
        {
            return constructor();
        }

        throw new ArgumentException("No type registered for this id");
    }

    public static void Register(Func<T> ctor)
    {
        _dict[typeof(T)] = ctor;
    }
}