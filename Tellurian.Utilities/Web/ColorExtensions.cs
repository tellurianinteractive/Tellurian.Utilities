using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Tellurian.Utilities.Web;

public static partial class ColorExtensions
{
    [GeneratedRegex("^#([A-Fa-f0-9]{6})$")]
    private static partial Regex HexColor();

    extension([NotNullWhen(true)] string? color)
    {
        /// <summary>
        /// Gets a value indicating whether the current color value is a valid hexadecimal color code.
        /// </summary>
        public bool IsHexColor =>
            color.HasValue && HexColor().IsMatch(color);

        /// <summary>
        /// Gets the recommended text color (black or white) for optimal contrast against the current background color.
        /// </summary>
        /// <remarks>The returned color is determined based on the brightness of the background color,
        /// ensuring sufficient readability. If the background color is not specified in hexadecimal format, the
        /// property returns null.</remarks>
        public string? TextColor
        {
            get
            {
                if (color.IsHexColor)
                {
                    var r = int.Parse(color.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                    var g = int.Parse(color.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                    var b = int.Parse(color.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                    var yiq = (r * 299 + g * 587 + b * 114) / 1000;
                    return yiq >= 128 ? "#000000" : "#FFFFFF";
                }
                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current color is white or not set.
        /// </summary>
        public bool IsWhiteColor =>
            color.IsEmpty || color.Equals("#FFFFFF", StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// Gets the hexadecimal color code that corresponds to the current color value.
        /// </summary>
        /// <remarks>If the color is already specified as a hexadecimal code, that value is returned.
        /// Otherwise, well-known color names such as "Black" and "White" are mapped to their standard hexadecimal
        /// representations. For unrecognized color names, a default value is returned.</remarks>
        public string ToHexColor =>
            color.IsHexColor ? color :
            color switch
            {
                "Black" => "#000000",
                "White" => "#FFFFFF",
                // TODO: Fill in other colors.
                _ => "000000"
            };
    }
}
