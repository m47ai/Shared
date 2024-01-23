namespace M47.Shared.Infrastructure.Database.DynamoDb.Model;

using Amazon.DynamoDBv2.DataModel;
using System;
using System.Linq;

public abstract class DocumentDynamoDb
{
    public object GetKey()
        => GetType().GetProperties()
                    .First(prop => Attribute.IsDefined(prop, typeof(DynamoDBHashKeyAttribute)))
                    .GetValue(this)!;
}