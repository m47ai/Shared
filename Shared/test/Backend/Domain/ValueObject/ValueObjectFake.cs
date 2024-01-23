namespace M47.Shared.Tests.Domain.ValueObject;

using M47.Shared.Domain.ValueObject;
using System.Collections.Generic;

public class ValueObjectFake : ValueObject
{
    public int? Property1 { get; set; }
    public int? Property2 { get; set; }

    public ValueObjectFake(int? property1, int? property2)
    {
        Property1 = property1;
        Property2 = property2;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Property1;
        yield return Property2;
    }
}