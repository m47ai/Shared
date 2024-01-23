namespace M47.Shared.Infrastructure.Storage;

using M47.Shared.Infrastructure.Storage.Model;

public interface IPublicStorage
{
    string GeneratePreSignedUrl(RequestGeneratePreSignedUrl request);
}