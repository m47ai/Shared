namespace M47.Shared.Infrastructure.Compress;

using System.IO;
using System.Text;
using System.Threading.Tasks;

public interface ICompressor
{
    Task<MemoryStream> CompressAsync(byte[] data, CancellationToken cancellationToken = default);

    Task<MemoryStream> CompressAsync(string data, Encoding encode, CancellationToken cancellationToken = default);

    Task<MemoryStream> CompressUtf8Async(string data, CancellationToken cancellationToken = default);

    Task<string> DecompressAsync(byte[] zippedData, Encoding encode, CancellationToken cancellationToken = default);

    Task<string> DecompressAsync(Stream stream, Encoding encode, CancellationToken cancellationToken = default);

    Task<string> DecompressUtf8Async(byte[] zippedData, CancellationToken cancellationToken = default);

    Task<string> DecompressUtf8Async(Stream stream, CancellationToken cancellationToken = default);
}