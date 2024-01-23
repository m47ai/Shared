namespace M47.Shared.Tests.Factory;

using M47.Shared.Tests.TestContainers;

public class SharedFactory : LocalStackWebBaseServicesFactory<BaseTest>
{
    public SharedFactory() : base("Shared")
    {
    }
}