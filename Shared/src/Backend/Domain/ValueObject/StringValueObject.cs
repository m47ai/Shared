namespace M47.Shared.Domain.ValueObject;

using System;
using System.Collections.Generic;

public class StringValueObject : ValueObject
{
    public string Value { get; }

    public StringValueObject(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return this.Value;
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

        if (obj is not StringValueObject item)
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