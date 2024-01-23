namespace M47.Shared.Domain.Comparer;

using System.Collections.Generic;
using System.Linq;

public abstract class ComparerObject
{
    private const int _hashPrimeNumberInit = 17;
    private const int _hashPrimeNumber = 29;

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ComparerObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (_hashPrimeNumberInit * _hashPrimeNumber)
                   + GetEqualityComponents().Select(x => (x?.GetHashCode()) ?? 0)
                                            .Aggregate((x, y) => (_hashPrimeNumber * x) + y);
        }
    }

    protected static bool EqualOperator(ComparerObject? left, ComparerObject? right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }

        return left is null || left.Equals(right!);
    }

    protected static bool NotEqualOperator(ComparerObject? left, ComparerObject? right)
    {
        return !EqualOperator(left, right);
    }

    protected abstract IEnumerable<object?> GetEqualityComponents();
}