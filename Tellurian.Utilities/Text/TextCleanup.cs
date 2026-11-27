namespace Tellurian.Utilities.Text;

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
    /// characters removed, repeated spaces one space; the text is trimmed too.
    /// </summary>
    PlainCharacters = 2,
    /// <summary><see cref="PlainCharacters"/> and <see cref="Trimmed"/>: the default cleanup.</summary>
    OrdinaryText = PlainCharacters | Trimmed,
    /// <summary>All spaces removed.</summary>
    NoSpaces = 4,
    /// <summary>All hyphens and dashes removed.</summary>
    NoHyphens = 8,
    /// <summary>An empty text becomes null.</summary>
    NullIfEmpty = 16,
}
