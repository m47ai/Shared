namespace M47.Shared.Domain.ValueObject;

using System;
using System.Collections.Generic;
using System.Globalization;

public class IntValueObject : ValueObject
{
    public int Value { get; }

    public IntValueObject(int value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return this.Value.ToString(NumberFormatInfo.InvariantInfo);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override bool Equals(object? obj)
    {
        if (this == obj)
        {
            return true;
        }

        if (obj is not IntValueObject item)
        {
            return false;
        }

        return Value == item.Value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Value);
    }
}