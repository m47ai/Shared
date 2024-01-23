namespace M47.Shared.Utils.Extensions;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public static class EnumExtension
{
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());

        var attributes = (DescriptionAttribute[])field!.GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attributes is not null && attributes.Length > 0)
        {
            return attributes[0].Description;
        }

        return value.ToString();
    }

    public static IEnumerable<T> GetEnumerable<T>()
    {
        var enumType = typeof(T);

        // Can't use generic type constraints on value types,
        // so have to do check like this
        if (enumType.BaseType != typeof(Enum))
        {
            throw new ArgumentException("T must be of type System.Enum");
        }

        var enumValArray = Enum.GetValues(enumType);
        var enumValList = new List<T>(enumValArray.Length);

        foreach (var val in enumValArray)
        {
            enumValList.Add((T)Enum.Parse(enumType, val!.ToString()!));
        }

        return enumValList;
    }

    public static T[] GetArray<T>() => GetEnumerable<T>().ToArray();

    public static T GetValue<T>(this string value)
    {
        var enumType = typeof(T);

        // Can't use generic type constraints on value types,
        // so have to do check like this
        if (enumType.BaseType != typeof(Enum))
        {
            throw new ArgumentException("T must be of type System.Enum");
        }

        return (T)Enum.Parse(enumType, value, true);
    }

    public static (bool, T?) TryGetValue<T>(this string value)
    {
        if (typeof(T).BaseType != typeof(Enum))
        {
            return (false, default);
        }

        return (true, (T)Enum.Parse(typeof(T), value, ignoreCase: true));
    }
}