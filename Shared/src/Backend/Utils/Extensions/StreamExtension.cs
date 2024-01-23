namespace M47.Shared.Utils.Extensions;

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

public static class StreamExtension
{
    private const int BufferLength = 16 * 1024;

    public static MemoryStream Copy(this MemoryStream ms, int offset, int count)
    {
        var data = ms.ToArray().Skip(offset).Take(count).ToArray();

        return new MemoryStream(data);
    }

    public static byte[] ToByteArray(this Stream stream)
    {
        long originalPosition = 0;

        if (stream.CanSeek)
        {
            originalPosition = stream.Position;
            stream.Position = 0;
        }

        try
        {
            byte[] readBuffer = new byte[4096];

            int totalBytesRead = 0;
            int bytesRead;

            while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
            {
                totalBytesRead += bytesRead;

                if (totalBytesRead == readBuffer.Length)
                {
                    int nextByte = stream.ReadByte();
                    if (nextByte != -1)
                    {
                        byte[] temp = new byte[readBuffer.Length * 2];
                        Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                        Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                        readBuffer = temp;
                        totalBytesRead++;
                    }
                }
            }

            byte[] buffer = readBuffer;
            if (readBuffer.Length != totalBytesRead)
            {
                buffer = new byte[totalBytesRead];
                Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
            }

            return buffer;
        }
        finally
        {
            if (stream.CanSeek)
            {
                stream.Position = originalPosition;
            }
        }
    }

    public static string GetString(this Stream input)
    {
        string @string;

        using (var reader = new StreamReader(input))
        {
            if (reader.BaseStream.CanSeek)
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
            }

            @string = reader.ReadToEnd();
        }

        return @string;
    }

    public static void WriteToStream(this Stream source, Stream destination)
    {
        int count;
        var buffer = new byte[BufferLength];
        while ((count = source.Read(buffer, 0, buffer.Length)) > 0)
        {
            destination.Write(buffer, 0, count);
        }
    }

    public static Stream GenerateStreamFromString(this string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;

        return stream;
    }

    public static Stream Clone(this Stream source)
    {
        var responseStream = new MemoryStream();

        source.CopyTo(responseStream);

        if (source.CanSeek)
        {
            source.Position = 0;
        }

        responseStream.Position = 0;

        return responseStream;
    }

    public static string SaveToFilePath(this Stream stream, string? filePath = null)
    {
        var path = filePath ?? Path.GetTempPath() + Path.GetRandomFileName();

        using (var file = new FileStream(path, FileMode.Create))
        {
            stream.CopyTo(file);
        }

        return path;
    }

    public static Stream Decompress(this Stream input)
    {
        using var archive = new ZipArchive(input, ZipArchiveMode.Read);
        var entry = archive.Entries.FirstOrDefault();

        return entry!.Open().Clone();
    }

    public static Stream Decompress(this byte[] bytes)
    {
        return Decompress(new MemoryStream(bytes));
    }

    public static bool IsPkZipCompressedData(this byte[] data)
    {
        Debug.Assert(data?.Length >= 4);

        // if the first 4 bytes of the array are the ZIP signature then it is compressed data
        return BitConverter.ToInt32(data, 0) == ZIP_LEAD_BYTES;
    }

    public static bool IsGZipCompressedData(this byte[] data)
    {
        Debug.Assert(data?.Length >= 2);

        return BitConverter.ToUInt16(data, 0) == GZIP_LEAD_BYTES;
    }

    private const int ZIP_LEAD_BYTES = 0x04034b50;
    private const ushort GZIP_LEAD_BYTES = 0x8b1f;
}