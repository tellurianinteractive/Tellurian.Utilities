using Tellurian.Utilities.Text;

namespace Tellurian.Utilities.Tests;

/// <summary>Corrections of input: what can be corrected is, and what can't is kept for a validation to refuse.</summary>
[TestClass]
public sealed class TextCorrectionExtensionsTests
{
    [TestMethod]
    [DataRow(null, null)]
    [DataRow("", "")]
    [DataRow("  Hällsjö  ", "Hällsjö")]
    [DataRow("Kalle’s “bästa”", "Kalle's \"bästa\"")]
    [DataRow("1950–1965 — slut", "1950-1965 - slut")]
    [DataRow("Vänta…", "Vänta...")]
    [DataRow("Nedre Ullerud", "Nedre Ullerud")]
    [DataRow("Nedre Ullerud", "Nedre Ullerud")]
    [DataRow("Gröndal", "Gröndal")]
    [DataRow("Gröndal", "Gröndal")]
    [DataRow("Kors​berga﻿", "Korsberga")]
    [DataRow("A\u0007B", "AB")]
    [DataRow("Övre   Bäck\tgården", "Övre Bäck gården")]
    [DataRow("Rad ett\r\nrad två", "Rad ett rad två")]
    [DataRow("Москва", "Москва")]
    public void OrdinaryText(string? text, string? expected) => Assert.AreEqual(expected, text.AsOrdinaryText());

    [TestMethod]
    public void MultilineTextKeepsLineBreaksAndIndentation() =>
        Assert.AreEqual("## Rubrik\r\n\r\n  - punkt - ett\r\n\tkod", "  ## Rubrik\r\n\r\n  - punkt – ett\r\n\tkod  ".AsOrdinaryText(multiline: true));

    [TestMethod]
    [DataRow(null, null)]
    [DataRow("", "")]
    [DataRow("Anna@Example.se", "anna@example.se")]
    [DataRow("a@b.se; c@d.se", "a@b.se;c@d.se")]
    [DataRow("a@b.se, c@d.se\r\ne@f.se", "a@b.se;c@d.se;e@f.se")]
    [DataRow("a@b.se;;A@B.se;", "a@b.se")]
    [DataRow("inte en adress", "inte;en;adress")]
    public void EmailAddresses(string? text, string? expected) => Assert.AreEqual(expected, text.AsEmailAddresses());

    [TestMethod]
    [DataRow(null, null)]
    [DataRow("", "")]
    [DataRow(" 3 ", "3")]
    [DataRow("10-15", "10-15")]
    [DataRow("15-10", "10-15")]
    [DataRow("10 - 15; 3 3", "3,10-15")]
    [DataRow("5,3,4,9", "3-5,9")]
    [DataRow("7-7", "7")]
    [DataRow("1, 9999", "1,9999")]
    [DataRow("0", "0")]
    [DataRow("10000", "10000")]
    [DataRow("3, x", "3, x")]
    [DataRow("1-2-3", "1-2-3")]
    [DataRow("-5", "-5")]
    public void NumberRanges(string? text, string? expected) => Assert.AreEqual(expected, text.AsNumberRanges(1, 9999));

    [TestMethod]
    public void CorrectedNumberRangesAreValid() => Assert.IsTrue("15-10; 3 3".AsNumberRanges(1, 9999).TryParseNumberRanges(1, 9999, out var addresses) && addresses.SequenceEqual([3, 10, 11, 12, 13, 14, 15]));

    [TestMethod]
    [DataRow(null, null)]
    [DataRow("aabbccddeeff", "AA:BB:CC:DD:EE:FF")]
    [DataRow("aa-bb-cc-dd-ee-ff", "AA:BB:CC:DD:EE:FF")]
    [DataRow("aabb.ccdd.eeff", "AA:BB:CC:DD:EE:FF")]
    [DataRow(" AA:BB:CC:DD:EE:FF ", "AA:BB:CC:DD:EE:FF")]
    [DataRow("aa:bb:cc:dd:ee", "aa:bb:cc:dd:ee")]
    [DataRow("gg:bb:cc:dd:ee:ff", "gg:bb:cc:dd:ee:ff")]
    public void MacAddress(string? text, string? expected) => Assert.AreEqual(expected, text.AsMacAddress());

    [TestMethod]
    [DataRow(new[] { 5, 3, 4, 9, 9 }, "3-5,9")]
    [DataRow(new[] { 1, 2 }, "1-2")]
    [DataRow(new[] { 7 }, "7")]
    [DataRow(new int[0], "")]
    public void ToCollapsedRanges(int[] numbers, string expected) => Assert.AreEqual(expected, numbers.ToCollapsedRanges());

    [TestMethod]
    [DataRow("3,10-15", true)]
    [DataRow("", true)]
    [DataRow("15-10", false)]
    [DataRow("7-7", false)]
    [DataRow("0", false)]
    [DataRow("3, x", false)]
    public void TryParseNumberRanges(string text, bool expected) => Assert.AreEqual(expected, text.TryParseNumberRanges(1, 9999, out _));

    [TestMethod]
    [DataRow("AA:BB:CC:DD:EE:FF", true)]
    [DataRow("aa:bb:cc:dd:ee:ff", false)]
    [DataRow("::::::::::::::::0", false)]
    public void IsMacAddress(string text, bool expected) => Assert.AreEqual(expected, text.IsMacAddress);
}
