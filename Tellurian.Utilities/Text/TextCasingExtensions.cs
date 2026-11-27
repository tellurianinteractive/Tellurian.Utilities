namespace Tellurian.Utilities.Text;

/// <summary>
/// One declarative correction of a text before saving it: <c>cargo.Name = cargo.Name.Corrected(TextCase.Sentence)</c>. The cleanup
/// is done first, then the casing. Formats with a fixed shape have their own corrections: <see cref="TextCorrectionExtensions"/>.
/// </summary>
public static class TextCasingExtensions
{
    extension(string? text)
    {
        /// <summary>The text cleaned up (by default <see cref="TextCleanup.OrdinaryText"/>) and cased; null stays null.</summary>
        public string? Corrected(TextCase textCase, TextCleanup cleanup = TextCleanup.OrdinaryText)
        {
            if (text is null) return null;
            var result = text;
            if (cleanup.HasFlag(TextCleanup.PlainCharacters)) result = result.AsOrdinaryText() ?? string.Empty;
            else if (cleanup.HasFlag(TextCleanup.Trimmed)) result = result.Trim();
            if (cleanup.HasFlag(TextCleanup.NoSpaces)) result = string.Concat(result.Where(c => !char.IsWhiteSpace(c)));
            if (cleanup.HasFlag(TextCleanup.NoHyphens)) result = string.Concat(result.Where(c => char.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.DashPunctuation));
            if (result.Length > 0) result = NameCapitalizationExtensions.Cased(result, textCase);
            return result.Length == 0 && cleanup.HasFlag(TextCleanup.NullIfEmpty) ? null : result;
        }

        /// <summary>True when <see cref="Corrected"/> wouldn't change the text, for a validation that still needs it.</summary>
        public bool IsCorrected(TextCase textCase, TextCleanup cleanup = TextCleanup.OrdinaryText) => text.Corrected(textCase, cleanup) == text;

        /// <summary>
        /// The text with its first letter a capital and the rest as typed (as <see cref="TextCase.InitialCapital"/>), for display,
        /// such as a label first in a sentence; nothing is cleaned up, and null gives an empty string.
        /// </summary>
        public string WithInitialCapital => string.IsNullOrEmpty(text) ? string.Empty : NameCapitalizationExtensions.Cased(text, TextCase.InitialCapital);

        /// <summary>
        /// The text with its first letter in lower case and the rest as typed, for display, such as a label inside a sentence
        /// ("Edit module"); nothing is cleaned up, and null gives an empty string.
        /// </summary>
        public string WithInitialLowerCase
        {
            get
            {
                if (string.IsNullOrEmpty(text)) return string.Empty;
                var i = 0;
                while (i < text.Length && !char.IsLetter(text[i])) i++;
                return i < text.Length && char.IsUpper(text[i]) ? text[..i] + char.ToLowerInvariant(text[i]) + text[(i + 1)..] : text;
            }
        }
    }
}
