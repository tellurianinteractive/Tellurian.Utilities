namespace Tellurian.Utilities.Text;

/// <summary>
/// How the letters of a text are cased by <see cref="TextCasingExtensions.Corrected"/>: one per kind of text. The name styles
/// (<see cref="Place"/>, <see cref="GivenName"/>, <see cref="FamilyName"/>, <see cref="Organisation"/>) are made for names in
/// Norway, Sweden, Finland, Denmark, Germany, Poland, the Czech Republic, Slovakia, Austria, Hungary, Italy, Switzerland, France,
/// the Netherlands, Great Britain, Luxembourg and Belgium; the language isn't needed.
/// </summary>
public enum TextCase
{
    /// <summary>The letters as typed.</summary>
    AsTyped,
    /// <summary>The first letter a capital, all others lower case, without exceptions: "CST" and "cst" become "Cst".</summary>
    Capitalized,
    /// <summary>
    /// As <see cref="Capitalized"/>, but abbreviations are kept: a word in capitals only in a text that isn't ("Gas (LPG)"), and a
    /// text in capitals only that is one word of up to three letters ("LPG", "CO2"); "STYCKEGODS" becomes "Styckegods". For
    /// descriptions of things, such as cargo names.
    /// </summary>
    Sentence,
    /// <summary>
    /// The first letter a capital (and "IJ" for a Dutch "ij"), all others as typed: for names whose later words or capitals carry
    /// meaning, such as "Stockholm Södra", "Göteborg C" and "KdMJK".
    /// </summary>
    InitialCapital,
    /// <summary>All letters capitals: "ff" becomes "FF".</summary>
    UpperCase,
    /// <summary>All letters lower case.</summary>
    LowerCase,
    /// <summary>Every word, except connecting words such as "och", "am" and "nad" after the first word: places and regions.</summary>
    Place,
    /// <summary>Every word, also after a hyphen, without exceptions: a person's given name.</summary>
    GivenName,
    /// <summary>
    /// Every word, also after a hyphen, except name particles, also first: "von", "af" and the like are always lower case;
    /// particles written differently by country ("van", "de", ...: "van Persie" but Belgian "De Bruyne", Swedish "De Geer") are
    /// kept as typed, as is an elision such as "d'" ("d'Artagnan", "D'Alessandro"). A person's middle and last name.
    /// </summary>
    FamilyName,
    /// <summary>
    /// As <see cref="Place"/>, but legal forms are written as they should be ("GmbH", "e.V.", "Sp. z o.o.", "S.p.A."), and in a
    /// name in capitals only, short words (up to five letters, such as "MEC" or "FREMO") are kept as abbreviations: groups, clubs,
    /// companies and operators.
    /// </summary>
    Organisation,
}

/// <summary>What else <see cref="TextCasingExtensions.Corrected"/> does with a text; the flags combine.</summary>
[Flags]
public enum TextCleanup
{
    /// <summary>Nothing.</summary>
    None = 0,
    /// <summary>Spaces at both ends removed.</summary>
    Trimmed = 1,
    /// <summary>
    /// <see cref="TextCorrectionExtensions.AsOrdinaryText"/> (single line): typographic quotes and dashes plain, invisible
    /// characters removed, repeated spaces one space; includes <see cref="Trimmed"/>.
    /// </summary>
    OrdinaryText = 2 | Trimmed,
    /// <summary>All spaces removed.</summary>
    NoSpaces = 4,
    /// <summary>All hyphens and dashes removed.</summary>
    NoHyphens = 8,
    /// <summary>An empty text becomes null.</summary>
    NullIfEmpty = 16,
}

/// <summary>
/// The <see cref="TextCase"/> for each kind of text, for declaring the casing rules of a language or a country in one place:
/// <c>Casings = { ["StationSignature"] = TextCase.UpperCase }</c>.
/// </summary>
public sealed class TextCasings : Dictionary<string, TextCase>
{
    public TextCasings() : base(StringComparer.Ordinal) { }

    /// <summary>The casing of the kind of text, or <paramref name="otherwise"/> when none is declared.</summary>
    public TextCase Of(string kind, TextCase otherwise = TextCase.AsTyped) => TryGetValue(kind, out var textCase) ? textCase : otherwise;
}

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
            if (cleanup.HasFlag(TextCleanup.OrdinaryText)) result = result.AsOrdinaryText()!;
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
