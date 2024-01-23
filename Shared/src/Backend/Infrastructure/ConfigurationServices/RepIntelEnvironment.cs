namespace M47.Shared.Infrastructure.ConfigurationServices;

public class RepIntelEnvironment
{
    public string? Name { get; private set; }

    public RepIntelEnvironment(string? name)
    {
        Name = name;
    }
}