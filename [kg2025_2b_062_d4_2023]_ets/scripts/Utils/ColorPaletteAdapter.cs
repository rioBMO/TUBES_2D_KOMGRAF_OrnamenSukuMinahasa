using Godot;
using System;

namespace KG2025.Utils
{
    public class ColorPaletteAdapter
    {
        public static ColorThemeManager ConvertToThemeManager(ColorPalette palette)
        {
            ColorThemeManager themeManager = new ColorThemeManager();

            // Copy colors from the palette to the theme manager
            themeManager.DarkGreen = palette.DarkGreen;
            themeManager.MediumGreen = palette.MediumGreen;
            themeManager.Cream = palette.Cream;
            themeManager.LightOrange = palette.LightOrange;
            themeManager.DarkOrange = palette.DarkOrange;

            // Set line color
            themeManager.SetLineColor(palette.LineColor);

            return themeManager;
        }
    }
}
