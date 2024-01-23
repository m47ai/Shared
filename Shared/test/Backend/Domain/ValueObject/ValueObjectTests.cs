namespace M47.Shared.Tests.Domain.ValueObject;

public sealed class ValueObjectTests
{
    [Theory]
    [InlineData(-2147483648, -2147483648, -2147483648, -2147483648)]
    [InlineData(2147483647, 2147483647, 2147483647, 2147483647)]
    [InlineData(null, 2147483647, null, 2147483647)]
    [InlineData(null, null, null, null)]
    public void Should_Equal_When_TwoValueObjectsPropertiesEqual(int? object1Prop1, int? object1Prop2, int? object2Prop1, int? object2Prop2)
    {
        var valueObject1 = new ValueObjectFake(object1Prop1, object1Prop2);
        var valueObject2 = new ValueObjectFake(object2Prop1, object2Prop2);

        Assert.Equal(valueObject1, valueObject2);
    }

    [Theory]
    [InlineData(1, 2, 2, 1)]
    [InlineData(1, 1, -1, -1)]
    [InlineData(0, 2147483647, null, 2147483647)]
    [InlineData(1, 0, 1, null)]
    [InlineData(0, 0, null, null)]
    public void Should_NotEqual_When_TwoValueObjectsPropertiesNotEqual(int? object1Prop1, int? object1Prop2, int? object2Prop1, int? object2Prop2)
    {
        var valueObject1 = new ValueObjectFake(object1Prop1, object1Prop2);
        var valueObject2 = new ValueObjectFake(object2Prop1, object2Prop2);

        Assert.NotEqual(valueObject1, valueObject2);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(null, null)]
    public void Should_NotEqual_When_OneValueObjectIsNull(int? object1Prop1, int? object1Prop2)
    {
        var valueObject1 = new ValueObjectFake(object1Prop1, object1Prop2);
        ValueObjectFake? valueObject2 = null;

        Assert.NotEqual(valueObject1, valueObject2);
    }

    [Fact]
    public void Should_Equal_When_TwoValueObjectAreNull()
    {
        ValueObjectFake? valueObject1 = null;
        ValueObjectFake? valueObject2 = null;

        Assert.Equal(valueObject1, valueObject2);
    }
}