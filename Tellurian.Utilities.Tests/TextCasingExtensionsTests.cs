using Tellurian.Utilities.Text;

namespace Tellurian.Utilities.Tests;

/// <summary>The declarative correction of texts: a casing per kind of text, and cleanups that combine.</summary>
[TestClass]
public sealed class TextCasingExtensionsTests
{
    [TestMethod]
    [DataRow("CST", "Cst")]
    [DataRow("cst", "Cst")]
    [DataRow("cSt", "Cst")]
    [DataRow("STYCKEGODS", "Styckegods")]
    [DataRow("Göteborg C", "Göteborg c")]
    [DataRow("ijmuiden", "IJmuiden")]
    [DataRow("ärtor", "Ärtor")]
    public void Capitalized(string text, string expected) => Assert.AreEqual(expected, text.Corrected(TextCase.Capitalized));

    [TestMethod]
    [DataRow("STYCKEGODS", "Styckegods")]
    [DataRow("styckegods", "Styckegods")]
    [DataRow("StyckeGods", "Styckegods")]
    [DataRow("Hard Coal", "Hard coal")]
    [DataRow("KOL OCH KOKS", "Kol och koks")]
    [DataRow("LPG", "LPG")]
    [DataRow("CO2", "CO2")]
    [DataRow("KOKS", "Koks")]
    [DataRow("gas (LPG)", "Gas (LPG)")]
    [DataRow("stückgut", "Stückgut")]
    [DataRow("\u201Cwood\u201D \u2013 logs", "\"Wood\" - logs")]
    public void Sentence(string text, string expected) => Assert.AreEqual(expected, text.Corrected(TextCase.Sentence));

    [TestMethod]
    [DataRow("ff", "FF")]
    [DataRow("Hbf", "HBF")]
    [DataRow("aa:bb:cc:dd:ee:ff", "AA:BB:CC:DD:EE:FF")]
    public void UpperCase(string text, string expected) => Assert.AreEqual(expected, text.Corrected(TextCase.UpperCase));

    [TestMethod]
    public void LowerCase() => Assert.AreEqual("abc", "AbC".Corrected(TextCase.LowerCase));

    [TestMethod]
    [DataRow("stockholm Södra", "Stockholm Södra")]
    [DataRow("KdMJK", "KdMJK")]
    [DataRow("SJ", "SJ")]
    public void InitialCapital(string text, string expected) => Assert.AreEqual(expected, text.Corrected(TextCase.InitialCapital));

    [TestMethod]
    public void TheNameStylesAreThere()
    {
        Assert.AreEqual("Frankfurt am Main", "FRANKFURT AM MAIN".Corrected(TextCase.Place));
        Assert.AreEqual("Anna-Karin", "anna-karin".Corrected(TextCase.GivenName));
        Assert.AreEqual("von Sydow", "Von Sydow".Corrected(TextCase.FamilyName));
        Assert.AreEqual("Tåg AB", "tåg ab".Corrected(TextCase.Organisation));
    }

    [TestMethod]
    public void AsTypedOnlyCleansUp() => Assert.AreEqual("ab Cd", "  ab   Cd ".Corrected(TextCase.AsTyped));

    [TestMethod]
    public void TheCleanupsCombine()
    {
        Assert.AreEqual("  a  b ", "  a  b ".Corrected(TextCase.AsTyped, TextCleanup.None));
        Assert.AreEqual("a  b", "  a  b ".Corrected(TextCase.AsTyped, TextCleanup.Trimmed));
        Assert.AreEqual("AB", " a b ".Corrected(TextCase.UpperCase, TextCleanup.NoSpaces));
        Assert.AreEqual("Hpbg", "h-pbg".Corrected(TextCase.Capitalized, TextCleanup.OrdinaryText | TextCleanup.NoHyphens));
        Assert.AreEqual("Hpbg", "h \u2013 pbg".Corrected(TextCase.Capitalized, TextCleanup.NoSpaces | TextCleanup.NoHyphens));
    }

    [TestMethod]
    public void EmptyIsNullOnlyWhenAsked()
    {
        Assert.AreEqual("", "   ".Corrected(TextCase.Sentence));
        Assert.IsNull("   ".Corrected(TextCase.Sentence, TextCleanup.OrdinaryText | TextCleanup.NullIfEmpty));
        Assert.IsNull(((string?)null).Corrected(TextCase.Sentence));
    }

    [TestMethod]
    public void IsCorrectedWhenNothingWouldChange()
    {
        Assert.IsTrue("Styckegods".IsCorrected(TextCase.Sentence));
        Assert.IsFalse("STYCKEGODS".IsCorrected(TextCase.Sentence));
    }

    [TestMethod]
    public void CasingsAreDeclaredPerKindOfText()
    {
        var casings = new TextCasings { ["StationSignature"] = TextCase.UpperCase };
        Assert.AreEqual(TextCase.UpperCase, casings.Of("StationSignature"));
        Assert.AreEqual(TextCase.AsTyped, casings.Of("Other"));
        Assert.AreEqual(TextCase.InitialCapital, casings.Of("Other", TextCase.InitialCapital));
    }

    [TestMethod]
    [DataRow(null, "")]
    [DataRow("", "")]
    [DataRow("redigera modul", "Redigera modul")]
    [DataRow("\"citat\"", "\"Citat\"")]
    [DataRow("åäö", "Åäö")]
    [DataRow("Redan KdMJK", "Redan KdMJK")]
    public void WithInitialCapital(string? text, string expected) => Assert.AreEqual(expected, text.WithInitialCapital);

    [TestMethod]
    [DataRow(null, "")]
    [DataRow("", "")]
    [DataRow("Modul", "modul")]
    [DataRow("Ändra", "ändra")]
    [DataRow("(Station)", "(station)")]
    [DataRow("FREMO", "fREMO", DisplayName = "Only the first letter")]
    [DataRow("123", "123")]
    public void WithInitialLowerCase(string? text, string expected) => Assert.AreEqual(expected, text.WithInitialLowerCase);
}