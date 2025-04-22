using Godot;
using System;

namespace KG2025.Utils
{
    public class ColorThemeManager
    {
        // Base color palette - make properties publicly settable for adapter
        public Color DarkGreen { get; set; } = new Color("283618");
        public Color MediumGreen { get; set; } = new Color("606C38");
        public Color Cream { get; set; } = new Color("FEFAE0");
        public Color LightOrange { get; set; } = new Color("DDA15E");
        public Color DarkOrange { get; set; } = new Color("BC6C25");

        // Line color
        public Color LineColor { get; private set; } = Colors.White;

        // Eye pattern original colors
        private Color originalEyeHexOutlineColor;
        private Color originalEyeHexFillColor;
        private Color originalEyeOuterCircleColor;
        private Color originalEyeInnerCircleColor;

        // Cross pattern original colors
        private Color originalCrossHexOutlineColor;
        private Color originalCrossHexFillColor;
        private Color originalCrossFillColor;
        private Color originalCrossInnerCircleColor;

        // Current eye pattern colors
        public Color EyeHexOutlineColor { get; private set; }
        public Color EyeHexFillColor { get; private set; }
        public Color EyeOuterCircleColor { get; private set; }
        public Color EyeInnerCircleColor { get; private set; }

        // Current cross pattern colors
        public Color CrossHexOutlineColor { get; private set; }
        public Color CrossHexFillColor { get; private set; }
        public Color CrossFillColor { get; private set; }
        public Color CrossInnerCircleColor { get; private set; }

        public ColorThemeManager()
        {
            // Initialize original colors to match Karya4
            originalEyeHexOutlineColor = DarkGreen;
            originalEyeHexFillColor = MediumGreen;
            originalEyeOuterCircleColor = Cream;
            originalEyeInnerCircleColor = DarkOrange;

            originalCrossHexOutlineColor = DarkOrange;
            originalCrossHexFillColor = LightOrange;
            originalCrossFillColor = Cream;
            originalCrossInnerCircleColor = DarkGreen;

            // Set current colors to originals
            ResetColors();
        }

        // Reset colors to original values
        public void ResetColors()
        {
            EyeHexOutlineColor = originalEyeHexOutlineColor;
            EyeHexFillColor = originalEyeHexFillColor;
            EyeOuterCircleColor = originalEyeOuterCircleColor;
            EyeInnerCircleColor = originalEyeInnerCircleColor;

            CrossHexOutlineColor = originalCrossHexOutlineColor;
            CrossHexFillColor = originalCrossHexFillColor;
            CrossFillColor = originalCrossFillColor;
            CrossInnerCircleColor = originalCrossInnerCircleColor;
        }

        // Swap eye and cross colors
        public void SwapEyeCrossColors()
        {
            EyeHexOutlineColor = originalCrossHexOutlineColor;
            EyeHexFillColor = originalCrossHexFillColor;
            EyeOuterCircleColor = originalCrossFillColor;
            EyeInnerCircleColor = originalCrossInnerCircleColor;

            CrossHexOutlineColor = originalEyeHexOutlineColor;
            CrossHexFillColor = originalEyeHexFillColor;
            CrossFillColor = originalEyeOuterCircleColor;
            CrossInnerCircleColor = originalEyeInnerCircleColor;
        }

        // Update colors based on input manager state
        public void UpdateColors(InputManager inputManager)
        {
            if (inputManager.SwapEyeCrossColors)
            {
                SwapEyeCrossColors();
            }
            else
            {
                ResetColors();
            }
        }

        public void SetLineColor(Color color)
        {
            LineColor = color;
        }
    }
}
