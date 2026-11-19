namespace Tellurian.Utilities.Web;

/// <summary>
/// A color with its name and hex code, for example for a color drop-down or swatches; see <see cref="NamedColorExtensions.ToNamedColors"/>.
/// </summary>
/// <param name="Name">The color name as given, for example to use as a resource key for a translated label.</param>
/// <param name="HexColor">The color as <c>#RRGGBB</c> in capitals.</param>
public sealed record NamedColor(string Name, string HexColor)
{
    /// <summary>A contrasting text color, white or black, for the color as background.</summary>
    public string TextColor => HexColor.TextColor;
}

public static class NamedColorExtensions
{
    extension(IEnumerable<string> colorNames)
    {
        /// <summary>
        /// The named colors with their hex codes, in the given order, from color names such as "Yellow", "Red" or "DarkBlue"
        /// (case-insensitive, the standard web colors, see <see cref="ColorExtensions"/>); hex colors are accepted too.
        /// </summary>
        /// <exception cref="ArgumentException">A name is no known color, so a typo shows up at once.</exception>
        public IReadOnlyList<NamedColor> ToNamedColors()
        {
            var unknown = colorNames.Where(name => !name.IsKnownColor).ToArray();
            if (unknown.Length > 0) throw new ArgumentException($"Unknown color names: {string.Join(", ", unknown)}.", nameof(colorNames));
            return [.. colorNames.Select(name => new NamedColor(name, name.NormalizedHexColor!))];
        }
    }
}