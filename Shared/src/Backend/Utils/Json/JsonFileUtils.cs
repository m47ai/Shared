namespace M47.Shared.Utils.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

public static class JsonFileUtils
{
    private static JsonSerializer _snakeCaseJsonSerializer
        => JsonSerializer.CreateDefault(new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
            Converters =
            {
                new StringEnumConverter(typeof(CamelCaseNamingStrategy))
            },
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        });

    private static JsonSerializer _camelCaseJsonSerializer
        => JsonSerializer.CreateDefault(new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            Converters =
            {
                new StringEnumConverter(typeof(CamelCaseNamingStrategy))
            }
        });

    public static void StreamWriteCamelCase(object obj, string fileName)
    {
        using var streamWriter = File.CreateText(fileName);
        using var jsonWriter = new JsonTextWriter(streamWriter);
        _camelCaseJsonSerializer.Serialize(jsonWriter, obj);
    }

    public static T StreamReadCamelCase<T>(string fileName)
    {
        using var streamReader = File.OpenText(fileName);
        using var jsonReader = new JsonTextReader(streamReader);

        return _camelCaseJsonSerializer.Deserialize<T>(jsonReader)!;
    }

    public static void StreamWriteSnakeCase(object obj, string fileName)
    {
        using var streamWriter = File.CreateText(fileName);
        using var jsonWriter = new JsonTextWriter(streamWriter);

        _snakeCaseJsonSerializer.Serialize(jsonWriter, obj);
    }

    public static T StreamReadSnakeCase<T>(string fileName)
    {
        using var streamReader = File.OpenText(fileName);
        using var jsonReader = new JsonTextReader(streamReader);

        return _snakeCaseJsonSerializer.Deserialize<T>(jsonReader)!;
    }
}