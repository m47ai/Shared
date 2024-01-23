namespace M47.Shared.Utils.Text;

using System;
using System.Security.Cryptography;
using System.Text;

public static class HashCreator
{
    public static string CreateMd5(string input)
    {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);
        var stringBuilder = new StringBuilder();

        for (var i = 0; i < hashBytes.Length; i++)
        {
            stringBuilder.Append(hashBytes[i].ToString("X2"));
        }

        return stringBuilder.ToString().ToLower();
    }

    public static string CreateSha256(string input)
    {
        using var sha = SHA256.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = sha.ComputeHash(inputBytes);

        return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
    }
}