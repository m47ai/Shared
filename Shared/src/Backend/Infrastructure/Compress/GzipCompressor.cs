namespace M47.Shared.Infrastructure.Compress;

using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class GzipCompressor : ICompressor
{
    public static async Task<MemoryStream> CreateStreamFromUtf8Async(string @string, CancellationToken cancellationToken = default)
        => await new GzipCompressor().CompressAsync(@string, Encoding.UTF8, cancellationToken);

    public async Task<MemoryStream> CompressAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        var ms = new MemoryStream();

        using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
        {
            var buffer = data;
            await zip.WriteAsync(buffer, cancellationToken);
            await zip.FlushAsync(cancellationToken);
        }

        ms.Position = 0;

        return ms;
    }

    public async Task<MemoryStream> CompressAsync(string data, Encoding encode, CancellationToken cancellationToken = default)
        => await CompressAsync(encode.GetBytes(data), cancellationToken);

    public async Task<MemoryStream> CompressUtf8Async(string data, CancellationToken cancellationToken = default)
        => await CompressAsync(Encoding.UTF8.GetBytes(data), cancellationToken);

    public async Task<string> DecompressAsync(byte[] zippedData, Encoding encode, CancellationToken cancellationToken = default)
    {
        ValidateZippedDataLength(zippedData);

        MemoryStream? inputStream = null;
        var outputStream = new MemoryStream();

        try
        {
            inputStream = new MemoryStream(zippedData);
            using var zip = new GZipStream(inputStream, CompressionMode.Decompress);
            inputStream = null;
            await zip.CopyToAsync(outputStream, cancellationToken);
        }
        finally
        {
            inputStream?.Dispose();
        }

        var byteContent = outputStream.ToArray();

        return encode.GetString(byteContent!)!;
    }

    public async Task<string> DecompressAsync(Stream stream, Encoding encode, CancellationToken cancellationToken = default)
        => await DecompressAsync(await ReadAllBytesAsync(stream, cancellationToken), encode, cancellationToken);

    public async Task<string> DecompressUtf8Async(byte[] zippedData, CancellationToken cancellationToken = default)
        => await DecompressAsync(zippedData, Encoding.UTF8, cancellationToken);

    public async Task<string> DecompressUtf8Async(Stream stream, CancellationToken cancellationToken = default)
        => await DecompressUtf8Async(await ReadAllBytesAsync(stream, cancellationToken), cancellationToken);

    private static async Task<byte[]> ReadAllBytesAsync(Stream input, CancellationToken cancellationToken)
    {
        using var ms = new MemoryStream();
        await input.CopyToAsync(ms, cancellationToken);

        return ms.ToArray();
    }

    private static void ValidateZippedDataLength(byte[] zippedData)
    {
        if (zippedData.Length == 0)
        {
            throw new ArgumentException($"{nameof(zippedData)} should be larger than 0");
        }
    }
}