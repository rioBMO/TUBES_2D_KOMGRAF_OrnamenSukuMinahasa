using Godot;
using System;

namespace KG2025.Utils
{
    public class ColorPalette
    {
        // Standard color palette for Karya3
        public Color DarkGreen { get; private set; } = new Color("283618");     // Dark green
        public Color MediumGreen { get; private set; } = new Color("606C38");   // Medium green
        public Color Cream { get; private set; } = new Color("FEFAE0");         // Off-white/cream
        public Color LightOrange { get; private set; } = new Color("DDA15E");   // Light orange
        public Color DarkOrange { get; private set; } = new Color("BC6C25");    // Dark orange

        // Default line color
        public Color LineColor { get; private set; } = Colors.White;

        public ColorPalette()
        {
        }

        // Initialize with custom colors if needed
        public ColorPalette(Color darkGreen, Color mediumGreen, Color cream, Color lightOrange, Color darkOrange)
        {
            DarkGreen = darkGreen;
            MediumGreen = mediumGreen;
            Cream = cream;
            LightOrange = lightOrange;
            DarkOrange = darkOrange;
        }

        public void SetLineColor(Color color)
        {
            LineColor = color;
        }
    }
}
