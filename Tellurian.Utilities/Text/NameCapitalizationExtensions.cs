namespace Tellurian.Utilities.Text;

/// <summary>
/// The casing of texts (<see cref="TextCase"/>), applied through <see cref="TextCasingExtensions.Corrected"/>. For names, a name
/// written in capitals only is first made lower case (except with <see cref="TextCase.InitialCapital"/>, where it may be an
/// abbreviation, and short words with <see cref="TextCase.Organisation"/>); otherwise first letters change, and capitals inside a word are kept only in a name form,
/// such as "McDonald" or "O'Brien": a jumbled word such as "SÖdeR" becomes "Söder". A word in capitals only in a name that
/// isn't is kept, as an abbreviation. Also: elisions ("Cortina d'Ampezzo", "Pont-l'Évêque", "L'Aquila"), "IJ" in Dutch
/// ("IJmuiden"), "Mc" and "O'". Made for names in Norway, Sweden, Finland, Denmark, Germany, Poland, the Czech Republic, Slovakia,
/// Austria, Hungary, Italy, Switzerland, France, the Netherlands, Great Britain, Luxembourg and Belgium. The language isn't needed:
/// the lists are shared by all languages, with words that conflict between languages left out (such as "by": English
/// "Stoke-by-Nayland" but Danish "Den Gamle By").
/// </summary>
public static partial class NameCapitalizationExtensions
{
    extension(string? name)
    {
        /// <summary>The name capitalised in the style; null and empty stay as they are.</summary>
        /// <param name="style">The kind of name.</param>
        [Obsolete("Use Corrected(TextCase, TextCleanup.None): FirstLetter is TextCase.InitialCapital and AllWords is TextCase.Place.")]
        public string? AsCapitalizedName(NameStyle style = NameStyle.AllWords) => string.IsNullOrEmpty(name) ? name : Cased(name, style switch
        {
            NameStyle.FirstLetter => TextCase.InitialCapital,
            NameStyle.GivenName => TextCase.GivenName,
            NameStyle.FamilyName => TextCase.FamilyName,
            NameStyle.Organisation => TextCase.Organisation,
            _ => TextCase.Place,
        });

        /// <summary>True when <see cref="AsCapitalizedName"/> wouldn't change the name; an empty name is valid.</summary>
        [Obsolete("Use IsCorrected(TextCase, TextCleanup.None).")]
        public bool IsCapitalizedName(NameStyle style = NameStyle.AllWords) => name.AsCapitalizedName(style) == name;
    }

    /// <summary>The text, not empty, in the casing.</summary>
    internal static string Cased(string text, TextCase textCase) => textCase switch
    {
        TextCase.AsTyped => text,
        TextCase.UpperCase => text.ToUpperInvariant(),
        TextCase.LowerCase => text.ToLowerInvariant(),
        TextCase.Capitalized => WithFirstCapital(text.ToLowerInvariant()),
        TextCase.Sentence => AsSentence(text),
        TextCase.InitialCapital => WithFirstCapital(text),
        _ => AsName(text, textCase),
    };

    /// <summary>
    /// The first letter a capital and the others lower case, except abbreviations: words in capitals only in a text that isn't, and a
    /// text in capitals only that is a single word of up to three letters.
    /// </summary>
    private static string AsSentence(string text)
    {
        if (IsAllUpperCase(text)) return !text.Any(char.IsWhiteSpace) && text.Count(char.IsLetter) <= 3 ? text : WithFirstCapital(text.ToLowerInvariant());
        return WithFirstCapital(LetterRun().Replace(text, run => run.Value.Count(char.IsLetter) > 1 && !run.Value.Any(char.IsLower) ? run.Value : run.Value.ToLowerInvariant()));
    }

    private static string AsName(string name, TextCase style)
    {
        var text = !IsAllUpperCase(name) ? name :
            style == TextCase.Organisation ? LongCapitalWord().Replace(name, word => word.Value.ToLowerInvariant()) :
            name.ToLowerInvariant();
        var result = new System.Text.StringBuilder(text.Length);
        var isFirstWord = true;
        var start = 0;
        for (var i = 0; i <= text.Length; i++)
        {
            if (i < text.Length && text[i] is not (' ' or '-')) continue;
            if (i > start)
            {
                result.Append(Capitalized(text[start..i], style, isFirstWord));
                isFirstWord = false;
            }
            if (i < text.Length) result.Append(text[i]);
            start = i + 1;
        }
        return result.ToString();
    }

    private static string Capitalized(string word, TextCase style, bool isFirstWord)
    {
        if (style == TextCase.Organisation && LegalForms.TryGetValue(word, out var spellings)) return spellings.Contains(word, StringComparer.Ordinal) ? word : spellings[0];
        if (style == TextCase.FamilyName && VariableNameParticles.Contains(word)) return word;
        var isException = style switch
        {
            TextCase.Place or TextCase.Organisation => !isFirstWord && ConnectingWords.Contains(word),
            TextCase.FamilyName => NameParticles.Contains(word),
            _ => false,
        };
        if (isException) return word.ToLowerInvariant();
        // Dutch "'s" and "'t" are lower case: "'s-Hertogenbosch", "'t Harde".
        if (word.Length == 2 && word[0] is '\'' or '’' && word[1] is 's' or 'S' or 't' or 'T') return word.ToLowerInvariant();
        // An abbreviation in an organisation's name, such as "KdMJK" or "MjBV", is kept as typed.
        if (style == TextCase.Organisation && word.Count(char.IsUpper) >= 2) return word;

        var apostrophe = word.IndexOfAny(['\'', '’']);
        if (apostrophe > 0 && apostrophe < word.Length - 1)
        {
            var prefix = word[..apostrophe].ToLowerInvariant();
            var rest = WordCapitalized(word[(apostrophe + 1)..]);
            if (prefix == "o") return "O" + word[apostrophe] + rest;
            if (Elisions.Contains(prefix))
            {
                // "d'Artagnan" or "D'Alessandro" as typed; "Cortina d'Ampezzo", but "L'Aquila" as the first word or in a given name.
                var elision = style == TextCase.FamilyName ? word[..apostrophe] :
                    isFirstWord || style == TextCase.GivenName ? WithFirstCapital(prefix) : prefix;
                return elision + word[apostrophe] + rest;
            }
        }
        if (word.Length > 2 && word.StartsWith("mc", StringComparison.OrdinalIgnoreCase) && char.IsLetter(word[2])) return "Mc" + WordCapitalized(word[2..]);
        return WordCapitalized(word);
    }

    /// <summary>
    /// Each run of letters in the word gets its capital ("Horten/Jæren", a name in quotation marks); capitals inside a run are kept
    /// only in a name form ("McDonald", "DiCaprio"), jumbled ones ("SÖdeR") are not. A colon belongs to the run, as in "S:t".
    /// </summary>
    private static string WordCapitalized(string word) =>
        LetterRun().Replace(word, run => WithFirstCapital(run.Value.Any(char.IsLower) && !NameForm().IsMatch(run.Value) ? run.Value.ToLowerInvariant() : run.Value));

    [System.Text.RegularExpressions.GeneratedRegex(@"\p{L}[\p{L}'’:]*")]
    private static partial System.Text.RegularExpressions.Regex LetterRun();

    /// <summary>
    /// The first letter as a capital, also after a leading quote or digit ("\"Wood\""), and a Dutch "ij" at the start as "IJ"
    /// ("IJmuiden", "IJsselstein").
    /// </summary>
    private static string WithFirstCapital(string text)
    {
        var i = 0;
        while (i < text.Length && !char.IsLetter(text[i])) i++;
        if (i == text.Length) return text;
        if (text.Length - i >= 2 && text[i] is 'i' or 'I' && text[i + 1] is 'j' or 'J' && (text.Length - i == 2 || !char.IsUpper(text[i + 2]))) return text[..i] + "IJ" + text[(i + 2)..];
        return char.IsLower(text[i]) ? text[..i] + char.ToUpperInvariant(text[i]) + text[(i + 1)..] : text;
    }

    /// <summary>
    /// A word in lower case after its first letter, or, beginning with a capital, in parts that each begin with one capital:
    /// "McDonald", "O'Brien".
    /// </summary>
    [System.Text.RegularExpressions.GeneratedRegex(@"^(\p{L}[\p{Ll}'’]*|\p{Lu}[\p{Ll}'’]*(\p{Lu}\p{Ll}[\p{Ll}'’]*)+)$")]
    private static partial System.Text.RegularExpressions.Regex NameForm();

    /// <summary>A word of more than five capitals: in an organisation's name in capitals only, not an abbreviation.</summary>
    [System.Text.RegularExpressions.GeneratedRegex(@"\p{Lu}{6,}")]
    private static partial System.Text.RegularExpressions.Regex LongCapitalWord();

    /// <summary>French and Italian elisions before a vowel: "d'Ampezzo", "l'Évêque", "nell'Emilia".</summary>
    private static readonly HashSet<string> Elisions = new(["d", "l", "dell", "nell", "dall", "sull", "all"], StringComparer.OrdinalIgnoreCase);

    /// <summary>Legal forms of organisations in the app's countries, with their accepted spellings, the first being the correction.</summary>
    private static readonly Dictionary<string, string[]> LegalForms = new(StringComparer.OrdinalIgnoreCase)
    {
        // German, Austrian, Swiss
        ["GmbH"] = ["GmbH"], ["AG"] = ["AG"], ["KG"] = ["KG"], ["OHG"] = ["OHG"], ["e.V."] = ["e.V."], ["Co."] = ["Co."],
        // Nordic
        ["AB"] = ["AB", "Ab"], ["A/S"] = ["A/S"], ["ApS"] = ["ApS"], ["AS"] = ["AS"], ["ASA"] = ["ASA"], ["Oy"] = ["Oy"], ["Oyj"] = ["Oyj"],
        // Czech, Slovak, Polish, Hungarian
        ["s.r.o."] = ["s.r.o."], ["a.s."] = ["a.s."], ["Sp."] = ["Sp."], ["o.o."] = ["o.o."], ["Kft."] = ["Kft."], ["Zrt."] = ["Zrt."],
        // Italian, French, Belgian, Luxembourgish, Swiss
        ["S.p.A."] = ["S.p.A."], ["S.r.l."] = ["S.r.l."], ["SA"] = ["SA"], ["SARL"] = ["SARL"], ["Sàrl"] = ["Sàrl"],
        // Dutch, Belgian
        ["BV"] = ["BV"], ["B.V."] = ["B.V."], ["NV"] = ["NV"], ["N.V."] = ["N.V."],
        // British
        ["Ltd"] = ["Ltd"], ["Ltd."] = ["Ltd."], ["plc"] = ["plc", "PLC"],
    };

    private static bool IsAllUpperCase(string text) => text.Count(char.IsLetter) > 1 && !text.Any(char.IsLower);

    /// <summary>
    /// Connecting words in place and group names, lower case except as the first word: the earlier validator's list and words used
    /// so in European place names, such as "Frankfurt an der Oder", "Bergen op Zoom", "Boulogne-sur-Mer", "Castiglione del Lago",
    /// "Stratford-upon-Avon" and "Ústí nad Labem".
    /// </summary>
    private static readonly HashSet<string> ConnectingWords = new(
    [
        // Earlier list
        "i", "af", "am", "an", "in", "im", "auf", "och", "und", "van", "von", // not "by": Danish "Den Gamle By"
        // Scandinavian
        "og", "på", "vid", "ved", "av", "över",
        // German
        "der", "die", "das", "dem", "den", "des", "bei", "ob", "vor", "zum", "zur", "unter",
        // Dutch
        "aan", "op", "het",
        // French
        "de", "du", "la", "le", "les", "sur", "sous", "en", "et", "aux", "lès",
        // Italian, Spanish, Portuguese
        "di", "del", "della", "delle", "dei", "al", "sul", "sulla", "e", "y", "da", "do", "dos", "las", "los",
        // English
        "and", "of", "the", "on", "upon", "under", "next", "super",
        // Czech, Slovak, Polish, Slovene
        "nad", "pod", "na", "u", "pri", "při", "przy", "w", "z", "v",
    ], StringComparer.OrdinalIgnoreCase);

    /// <summary>Name particles that are always lower case in middle and last names.</summary>
    private static readonly HashSet<string> NameParticles = new(["af", "av", "von", "zu", "zum", "zur", "vom"], StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Name particles kept as typed, since the custom differs by country and family; a name in capitals only gets them in lower case.
    /// </summary>
    private static readonly HashSet<string> VariableNameParticles = new(
        ["van", "de", "der", "den", "des", "ten", "ter", "op", "in", "het", "da", "di", "du", "do", "dos", "das", "del", "della", "dei", "degli", "dal", "dalla", "la", "le", "y", "e"],
        StringComparer.OrdinalIgnoreCase);
}