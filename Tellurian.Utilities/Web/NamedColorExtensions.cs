namespace Tellurian.Utilities.Web;

/// <summary>
/// Extensions that give <see cref="NamedColor"/> values.
/// </summary>
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
