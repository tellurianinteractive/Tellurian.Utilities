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
