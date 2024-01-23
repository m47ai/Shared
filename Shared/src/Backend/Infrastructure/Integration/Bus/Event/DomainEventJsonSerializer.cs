namespace M47.Shared.Infrastructure.Integration.Bus.Event;

using M47.Shared.Domain.Bus.Event;
using M47.Shared.Domain.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

public class DomainEventJsonSerializer
{
    protected DomainEventJsonSerializer()
    {
    }

    private static readonly JsonSerializerSettings _settings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy(),
        },
        Converters =
        {
            new StringEnumConverter(typeof(CamelCaseNamingStrategy))
        },
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore,
    };

    public static string Serialize<T>(Message<T> message) where T : IBaseEvent
        => JsonConvert.SerializeObject(message, _settings);
}