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
