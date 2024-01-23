namespace M47.Shared.Utils.Text;

using System;
using System.Text;
using System.Text.Json;

public static class Text
{
    public static string TruncateByWord(string input, int length, int offset = 50)
    {
        if (input.Length <= length)
        {
            return input;
        }

        var index = 0;
        var builder = new StringBuilder(input);

        var tail = builder.Length - length;
        tail = (tail > offset) ? offset : tail;

        var contentExceed = builder.ToString(length, tail);

        foreach (char c in contentExceed)
        {
            if (Char.IsPunctuation(c) || Char.IsSeparator(c))
            {
                break;
            }

            index++;
        }

        return builder.ToString(0, index + length);
    }

    public static string PrettyJson(this string unPrettyJson)
    {
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        var jsonElement = JsonSerializer.Deserialize<JsonElement>(unPrettyJson);

        return JsonSerializer.Serialize(jsonElement, options);
    }

    public static string GetSha256(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        using var sha = System.Security.Cryptography.SHA256.Create();
        var textData = Encoding.UTF8.GetBytes(text);
        var hash = sha.ComputeHash(textData);

        return BitConverter.ToString(hash).Replace("-", string.Empty);
    }
}