namespace Tellurian.Utilities.Text;

/// <summary>How a name is capitalised (<see cref="NameCapitalizationExtensions"/>): the kind of name decides.</summary>
[Obsolete("Use TextCase with Corrected(): FirstLetter is TextCase.InitialCapital and AllWords is TextCase.Place.")]
public enum NameStyle
{
    /// <summary>
    /// Only the first letter of the name (and "IJ" for a Dutch "ij"); for names that may contain abbreviations and numbers, such as
    /// stations, signatures, modules and freight customers.
    /// </summary>
    FirstLetter,
    /// <summary>Every word, except connecting words such as "och", "am" and "nad" after the first word; for places and regions.</summary>
    AllWords,
    /// <summary>Every word, also after a hyphen, without exceptions; for a person's first name.</summary>
    GivenName,
    /// <summary>
    /// Every word, also after a hyphen, except name particles, also first: "von", "af" and the like are always lower case;
    /// particles written differently by country ("van", "de", ...: "van Persie" but Belgian "De Bruyne", Swedish "De Geer") are
    /// kept as typed, as is an elision such as "d'" ("d'Artagnan", "D'Alessandro"). For a person's middle and last name.
    /// </summary>
    FamilyName,
    /// <summary>
    /// As <see cref="AllWords"/>, but legal forms are written as they should be ("GmbH", "e.V.", "Sp. z o.o.", "S.p.A."), and in a
    /// name in capitals only, short words (up to five letters, such as "MEC" or "FREMO") are kept as abbreviations; for groups,
    /// clubs, companies and operators.
    /// </summary>
    Organisation,
}
