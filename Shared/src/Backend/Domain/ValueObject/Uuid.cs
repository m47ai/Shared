namespace M47.Shared.Domain.ValueObject;

using System;
using System.Collections.Generic;
using System.ComponentModel;

public class Uuid : ValueObject
{
    public string Value { get; }

    public Uuid(string value)
    {
        EnsureIsValidUuid(value);
        Value = value;
    }

    private static void EnsureIsValidUuid(string value)
    {
        if (!Guid.TryParse(value, out _))
        {
            throw new InvalidEnumArgumentException($"{value} is not a valid GUID");
        }
    }

    public override string ToString() => this.Value;

    public static Uuid Random() => new(Guid.NewGuid().ToString());

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

        if (obj is not Uuid item)
        {
            return false;
        }

        return Value == item.Value;
    }

    public override int GetHashCode() => HashCode.Combine(this.Value);
}