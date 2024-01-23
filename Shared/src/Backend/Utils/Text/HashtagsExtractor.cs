namespace M47.Shared.Utils.Text;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Regexes are .net translations of java regexes located at: https://github.com/twitter/twitter-text
/// </summary>
public static class HashtagsExtractor
{
    private static readonly Regex _validHashtag = new("(^|[^&" + _hashtagAlphaNumericChars + "])([#|\uFF03](?!https?)(?<!"
                                                      + _basicUrlDetection + "))(" + _hashtagAlphaNumeric + "*" + _hashtagAlpha +
                                                      _hashtagAlphaNumeric + "*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public const int _validHashtagGroupTag = 5;

    private const string _latinAccentsChars = "\\u00c0-\\u00d6\\u00d8-\\u00f6\\u00f8-\\u00ff" + // Latin-1
                                              "\\u0100-\\u024f" + // Latin Extended A and B
                                              "\\u0253\\u0254\\u0256\\u0257\\u0259\\u025b\\u0263\\u0268\\u026f\\u0272\\u0289\\u028b" +
                                              // IPA Extensions
                                              "\\u02bb" + // Hawaiian
                                              "\\u0300-\\u036f" + // Combining diacritics
                                              "\\u1e00-\\u1eff";

    // Latin Extended Additional (mostly for Vietnamese)

    private const string _hashtagAlphaChars = "a-z" + _latinAccentsChars +
                                              "\\u0400-\\u04ff\\u0500-\\u0527" + // Cyrillic
                                              "\\u2de0-\\u2dff\\ua640-\\ua69f" + // Cyrillic Extended A/B
                                              "\\u0591-\\u05bf\\u05c1-\\u05c2\\u05c4-\\u05c5\\u05c7" +
                                              "\\u05d0-\\u05ea\\u05f0-\\u05f4" + // Hebrew
                                              "\\ufb1d-\\ufb28\\ufb2a-\\ufb36\\ufb38-\\ufb3c\\ufb3e\\ufb40-\\ufb41" +
                                              "\\ufb43-\\ufb44\\ufb46-\\ufb4f" + // Hebrew Pres. Forms
                                              "\\u0610-\\u061a\\u0620-\\u065f\\u066e-\\u06d3\\u06d5-\\u06dc" +
                                              "\\u06de-\\u06e8\\u06ea-\\u06ef\\u06fa-\\u06fc\\u06ff" + // Arabic
                                              "\\u0750-\\u077f\\u08a0\\u08a2-\\u08ac\\u08e4-\\u08fe" +
                                              // Arabic Supplement and Extended A
                                              "\\ufb50-\\ufbb1\\ufbd3-\\ufd3d\\ufd50-\\ufd8f\\ufd92-\\ufdc7\\ufdf0-\\ufdfb" +
                                              // Pres. Forms A
                                              "\\ufe70-\\ufe74\\ufe76-\\ufefc" + // Pres. Forms B
                                              "\\u200c" + // Zero-Width Non-Joiner
                                              "\\u0e01-\\u0e3a\\u0e40-\\u0e4e" + // Thai
                                              "\\u1100-\\u11ff\\u3130-\\u3185\\uA960-\\uA97F\\uAC00-\\uD7AF\\uD7B0-\\uD7FF" +
                                              // Hangul (Korean)
                                              "\\p{IsHiragana}\\p{IsKatakana}" + // Japanese Hiragana and Katakana
                                              "\\p{IsCJKUnifiedIdeographs}" + // Japanese Kanji / Chinese Han
                                              "\\u3003\\u3005\\u303b" + // Kanji/Han iteration marks
                                              "\\uff21-\\uff3a\\uff41-\\uff5a" + // full width Alphabet
                                              "\\uff66-\\uff9f" + // half width Katakana
                                              "\\uffa1-\\uffdc"; // half width Hangul (Korean)

    private const string _hashtagAlphaNumericChars = "0-9\\uff10-\\uff19_" + _hashtagAlphaChars;
    private const string _hashtagAlpha = "[" + _hashtagAlphaChars + "]";
    private const string _hashtagAlphaNumeric = "[" + _hashtagAlphaNumericChars + "]";

    /* URL related hash regex collection */
    private const string _urlValidChars = "\\p{Nl}a-z" + _latinAccentsChars;
    private const string _basicUrlDetection = "(https?\\S*)|(" + _urlValidDomainName + "\\S*)";

    private const string _urlValidDomainName = "(?:(?:[" + _urlValidChars + "][" + _urlValidChars + "\\-]*)?[" +
                                                    _urlValidChars + "]\\.)";

    /// <summary>
    /// Extract #hashtag references from Tweet text.
    /// </summary>
    /// <param name="text">text of the tweet from which to extract hashtags</param>
    /// <returns>List of hashtags referenced (without the leading # sign)</returns>
    public static IEnumerable<string> HashtagsInText(string? text, bool toLower = false, bool returnHashtagChar = false)
    {
        var hashtags = new List<string>();

        if (!string.IsNullOrEmpty(text))
        {
            hashtags.AddRange(ExtractList(_validHashtag, text, _validHashtagGroupTag, toLower));

            if (returnHashtagChar)
            {
                return hashtags.Select(item => "#" + item);
            }
        }

        return hashtags;
    }

    /// <summary>
    /// Helper method for extracting multiple matches from Tweet text.
    /// </summary>
    /// <param name="pattern">pattern to match and use for extraction</param>
    /// <param name="text">text of the Tweet to extract from</param>
    /// <param name="groupNumber">groupNumber the capturing group of the pattern that should be added to the list.</param>
    /// <returns>list of extracted values, or an empty list if there were none.</returns>
    private static IEnumerable<string> ExtractList(Regex pattern, string text, int groupNumber, bool toLower)
    {
        var matcher = pattern.Match(text);

        while (matcher.Success)
        {
            var value = matcher.Groups[groupNumber].Value;

            yield return toLower ? value.ToLower() : value;

            matcher = matcher.NextMatch();
        }
    }
}