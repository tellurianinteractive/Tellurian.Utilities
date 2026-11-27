namespace Tellurian.Utilities.Text;

/// <summary>
/// The <see cref="TextCase"/> for each kind of text, for declaring the casing rules of a language or a country in one place:
/// <c>Casings = { ["StationSignature"] = TextCase.UpperCase }</c>.
/// </summary>
public sealed class TextCasings : Dictionary<string, TextCase>
{
    /// <summary>Creates casings whose kinds of text are compared ordinally.</summary>
    public TextCasings() : base(StringComparer.Ordinal) { }

    /// <summary>The casing of the kind of text, or <paramref name="otherwise"/> when none is declared.</summary>
    public TextCase Of(string kind, TextCase otherwise = TextCase.AsTyped) => TryGetValue(kind, out var textCase) ? textCase : otherwise;
}
