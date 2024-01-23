namespace M47.Shared.Domain.Ports;

public interface IBearerToken
{
    Task<string> UpdateTokenAsync();
}