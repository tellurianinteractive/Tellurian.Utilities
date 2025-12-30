using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Tellurian.Utilities.Web;

public static partial class ColorExtensions
{
    [GeneratedRegex("^#([A-Fa-f0-9]{6})$")]
    private static partial Regex HexColorPattern();

    private static readonly Dictionary<string, string> ColorNameToHex = new(StringComparer.OrdinalIgnoreCase)
    {
        // Basic colors
        ["Black"] = "#000000",
        ["White"] = "#FFFFFF",
        ["Red"] = "#FF0000",
        ["Green"] = "#008000",
        ["Blue"] = "#0000FF",
        ["Yellow"] = "#FFFF00",
        ["Cyan"] = "#00FFFF",
        ["Magenta"] = "#FF00FF",

        // Common colors
        ["Orange"] = "#FFA500",
        ["Pink"] = "#FFC0CB",
        ["Purple"] = "#800080",
        ["Brown"] = "#A52A2A",
        ["Gray"] = "#808080",
        ["Grey"] = "#808080",

        // Extended basic colors
        ["Lime"] = "#00FF00",
        ["Aqua"] = "#00FFFF",
        ["Fuchsia"] = "#FF00FF",
        ["Navy"] = "#000080",
        ["Teal"] = "#008080",
        ["Maroon"] = "#800000",
        ["Olive"] = "#808000",
        ["Silver"] = "#C0C0C0",

        // Popular web colors
        ["Gold"] = "#FFD700",
        ["Coral"] = "#FF7F50",
        ["Crimson"] = "#DC143C",
        ["Indigo"] = "#4B0082",
        ["Violet"] = "#EE82EE",
        ["Salmon"] = "#FA8072",
        ["Tomato"] = "#FF6347",
        ["Turquoise"] = "#40E0D0",
        ["Chocolate"] = "#D2691E",
        ["Beige"] = "#F5F5DC",
        ["Ivory"] = "#FFFFF0",
        ["Khaki"] = "#F0E68C",
        ["Lavender"] = "#E6E6FA",
        ["Plum"] = "#DDA0DD",
        ["Tan"] = "#D2B48C",
        ["Peru"] = "#CD853F",
        ["Wheat"] = "#F5DEB3",
        ["Azure"] = "#F0FFFF",
        ["Snow"] = "#FFFAFA",

        // Dark variants
        ["DarkBlue"] = "#00008B",
        ["DarkCyan"] = "#008B8B",
        ["DarkGray"] = "#A9A9A9",
        ["DarkGrey"] = "#A9A9A9",
        ["DarkGreen"] = "#006400",
        ["DarkMagenta"] = "#8B008B",
        ["DarkOrange"] = "#FF8C00",
        ["DarkRed"] = "#8B0000",
        ["DarkViolet"] = "#9400D3",

        // Light variants
        ["LightBlue"] = "#ADD8E6",
        ["LightCoral"] = "#F08080",
        ["LightCyan"] = "#E0FFFF",
        ["LightGray"] = "#D3D3D3",
        ["LightGrey"] = "#D3D3D3",
        ["LightGreen"] = "#90EE90",
        ["LightPink"] = "#FFB6C1",
        ["LightYellow"] = "#FFFFE0",

        // Blues
        ["SkyBlue"] = "#87CEEB",
        ["SteelBlue"] = "#4682B4",
        ["RoyalBlue"] = "#4169E1",
        ["DodgerBlue"] = "#1E90FF",
        ["CornflowerBlue"] = "#6495ED",

        // Greens
        ["SeaGreen"] = "#2E8B57",
        ["ForestGreen"] = "#228B22",
        ["LimeGreen"] = "#32CD32",
        ["SpringGreen"] = "#00FF7F",

        // Grays
        ["SlateGray"] = "#708090",
        ["SlateGrey"] = "#708090",
        ["DimGray"] = "#696969",
        ["DimGrey"] = "#696969",

        // Others
        ["Sienna"] = "#A0522D",
        ["OrangeRed"] = "#FF4500",
        ["HotPink"] = "#FF69B4",
        ["DeepPink"] = "#FF1493",
    };

    extension([NotNullWhen(true)] string? color)
    {
        /// <summary>
        /// Gets a value indicating whether the current color value is a valid hexadecimal color code.
        /// </summary>
        public bool IsHexColor =>
            color.HasValue && HexColorPattern().IsMatch(color);

        /// <summary>
        /// Gets the recommended text color (black or white) for optimal contrast against the current background color.
        /// </summary>
        /// <remarks>The returned color is determined based on the brightness of the background color,
        /// ensuring sufficient readability.</remarks>
        public string TextColor
        {
            get
            {
                var hexColor = color.HexColor;
                var r = int.Parse(hexColor.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                var g = int.Parse(hexColor.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                var b = int.Parse(hexColor.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                var yiq = (r * 299 + g * 587 + b * 114) / 1000;
                return yiq >= 128 ? "#000000" : "#FFFFFF";
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current color is white or not set.
        /// </summary>
        /// <remarks>Recognizes both hex code (#FFFFFF) and color name (White, white) case-insensitively.</remarks>
        public bool IsWhite =>
            color.IsEmpty || color.HexColor.Equals("#FFFFFF", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the hexadecimal color code that corresponds to the current color value.
        /// </summary>
        /// <remarks>If the color is already specified as a hexadecimal code, that value is returned.
        /// Otherwise, well-known color names such as "Black" and "White" are mapped (case-insensitively) to their
        /// standard hexadecimal representations. For unrecognized color names, a default value #FFFFFF (white) is returned.</remarks>
        public string HexColor =>
            color.IsHexColor ? color :
            color is not null && ColorNameToHex.TryGetValue(color, out var hex) ? hex : "#FFFFFF";
    }
}
