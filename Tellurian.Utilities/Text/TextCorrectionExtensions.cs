using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Tellurian.Utilities.Text;

/// <summary>
/// Corrections of typed or pasted input, for applying before saving it, so users only need to be told about what can't be
/// corrected. Each correction keeps a value it can't make sense of unchanged (apart from trimming), so a validation that follows can
/// refuse it.
/// </summary>
public static partial class TextCorrectionExtensions
{
    extension(string? text)
    {
        /// <summary>
        /// Text as typed or pasted, made ordinary: typographic quotes, dashes and ellipses become their plain equivalents, all kinds
        /// of spaces a plain space, letters with separate accent marks single letters (Unicode normalisation), zero-width and control
        /// characters are removed, and the text is trimmed. A single line also has its line breaks and repeated spaces replaced by
        /// one space; <paramref name="multiline"/> text keeps its line breaks and indentation.
        /// </summary>
        public string? AsOrdinaryText(bool multiline = false)
        {
            if (string.IsNullOrEmpty(text)) return text;
            var normalized = text.Normalize(NormalizationForm.FormC);
            var result = new StringBuilder(normalized.Length);
            foreach (var c in normalized)
            {
                switch (c)
                {
                    case '‘' or '’' or '‚' or '‛' or '′': result.Append('\''); break;
                    case '“' or '”' or '„' or '‟' or '″': result.Append('"'); break;
                    case '‐' or '‑' or '‒' or '–' or '—' or '―' or '−': result.Append('-'); break;
                    case '…': result.Append("..."); break;
                    case '\r': if (multiline) result.Append(c); break;
                    case '\n' or '\t': result.Append(multiline ? c : ' '); break;
                    case '​' or '‌' or '‍' or '⁠' or '﻿': break;
                    default:
                        if (char.IsControl(c)) break;
                        result.Append(char.GetUnicodeCategory(c) is UnicodeCategory.SpaceSeparator ? ' ' : c);
                        break;
                }
            }
            var corrected = result.ToString();
            return multiline ? corrected.Trim() : RepeatedSpaces().Replace(corrected, " ").Trim();
        }

        /// <summary>
        /// Email addresses separated by semicolons: any of <c>; ,</c> space or a line break separates them; each is trimmed and in
        /// lower case, and empty entries and duplicates are removed.
        /// </summary>
        public string? AsEmailAddresses()
        {
            if (text is null) return null;
            return string.Join(';', text.Split([';', ',', ' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                .Select(address => address.ToLowerInvariant())
                .Distinct());
        }

        /// <summary>
        /// Determines whether the text is one email address: a local part of letters, digits and <c>.!#$%&amp;'*+/=?^_`{|}~-</c>, an
        /// <c>@</c>, and a domain of labels with letters, digits and hyphens, separated by dots (the HTML standard's rule for an
        /// email input). Not trimmed: surrounding spaces make it invalid.
        /// </summary>
        public bool IsEmailAddress => text.HasValue && EmailAddress().IsMatch(text);

        /// <summary>
        /// Determines whether the text is one or more email addresses separated by semicolons, as <see cref="AsEmailAddresses"/>
        /// gives them. To accept what the correction understands, check the corrected text: <c>text.AsEmailAddresses().IsEmailAddresses</c>.
        /// </summary>
        public bool IsEmailAddresses => text.HasValue && text.Split(';').All(address => address.IsEmailAddress);

        /// <summary>A MAC address in any common notation (<c>AABBCCDDEEFF</c>, with <c>: - .</c> or spaces) as <c>AA:BB:CC:DD:EE:FF</c>.</summary>
        public string? AsMacAddress()
        {
            if (text.IsEmpty) return text?.Trim();
            var digits = MacSeparators().Replace(text, "");
            if (digits.Length != 12 || !digits.All(char.IsAsciiHexDigit)) return text.Trim();
            return string.Join(':', digits.ToUpperInvariant().Chunk(2).Select(pair => new string(pair)));
        }

        /// <summary>Determines whether the text is a MAC address as <c>AA:BB:CC:DD:EE:FF</c>, which <see cref="AsMacAddress"/> gives.</summary>
        public bool IsMacAddress => text.HasValue && MacAddress().IsMatch(text);

        /// <summary>
        /// Numbers and ranges between <paramref name="minimum"/> and <paramref name="maximum"/>, such as <c>3,10-15</c>: <c>; ,</c>
        /// or spaces separate them, a reversed range is turned, and the numbers are sorted with duplicates removed and consecutive
        /// numbers as ranges. Text that isn't such numbers is only trimmed.
        /// </summary>
        public string? AsNumberRanges(int minimum, int maximum)
        {
            if (text.IsEmpty) return text?.Trim();
            var numbers = new SortedSet<int>();
            foreach (var part in AroundHyphen().Replace(text, "-").Split([',', ';', ' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
            {
                var ends = part.Split('-');
                if (ends.Length is 0 or > 2 || !ends.All(e => int.TryParse(e, NumberStyles.None, CultureInfo.InvariantCulture, out var n) && n >= minimum && n <= maximum)) return text.Trim();
                var values = ends.Select(e => int.Parse(e, CultureInfo.InvariantCulture)).ToArray();
                for (var number = values.Min(); number <= values.Max(); number++) numbers.Add(number);
            }
            return numbers.ToArray().ToCollapsedRanges();
        }

        /// <summary>
        /// Parses numbers and ranges separated by commas, such as <c>3,10-15</c>, each between <paramref name="minimum"/> and
        /// <paramref name="maximum"/>; a range must be ascending. Empty text gives no numbers and is valid.
        /// </summary>
        public bool TryParseNumberRanges(int minimum, int maximum, out int[] numbers)
        {
            numbers = [];
            if (text.IsEmpty) return true;
            var result = new List<int>();
            foreach (var part in text.ToItems(','))
            {
                var ends = part.Split('-');
                if (ends.Length > 2 || !ends.All(e => int.TryParse(e.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var n) && n >= minimum && n <= maximum)) return false;
                var first = int.Parse(ends[0].Trim(), CultureInfo.InvariantCulture);
                var last = int.Parse(ends[^1].Trim(), CultureInfo.InvariantCulture);
                if (first > last || (ends.Length == 2 && first == last)) return false;
                result.AddRange(Enumerable.Range(first, last - first + 1));
            }
            numbers = [.. result];
            return true;
        }
    }

    extension(int[]? numbers)
    {
        /// <summary>The numbers sorted, without duplicates, with consecutive numbers as ranges: <c>3,10-15</c>.</summary>
        public string ToCollapsedRanges()
        {
            if (numbers is null || numbers.Length == 0) return string.Empty;
            var ordered = numbers.Distinct().Order().ToArray();
            var result = new StringBuilder(ordered.Length * 5);
            var start = 0;
            for (var i = 1; i <= ordered.Length; i++)
            {
                if (i < ordered.Length && ordered[i] == ordered[i - 1] + 1) continue;
                if (result.Length > 0) result.Append(',');
                result.Append(ordered[start]);
                if (i - 1 > start) result.Append('-').Append(ordered[i - 1]);
                start = i;
            }
            return result.ToString();
        }
    }

    [GeneratedRegex(" {2,}")]
    private static partial Regex RepeatedSpaces();

    [GeneratedRegex(@"\s*-\s*")]
    private static partial Regex AroundHyphen();

    [GeneratedRegex(@"[\s:.\-]")]
    private static partial Regex MacSeparators();

    [GeneratedRegex("^([0-9A-F]{2}:){5}[0-9A-F]{2}$")]
    private static partial Regex MacAddress();

    [GeneratedRegex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")]
    private static partial Regex EmailAddress();
}