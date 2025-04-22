using Godot;
using System;
using KG2025.Utils;

namespace KG2025.Components.UI
{
    public class HelpOverlay
    {
        private Node2D parent;

        // Help text content
        private string controlsHeader = "Controls:";
        private string controlR = "R - Reverse Orbit Rotation";
        private string controlC = "C - Swap Eye / Cross Colors";
        private string controlPlus = "+ - Increase Animation Speed";
        private string controlMinus = "- - Decrease Animation Speed";
        private string statusHeader = "Status:";
        private string hideHint = "Press H to hide instructions";

        // UI visual properties
        private float padding = 15;
        private float lineHeight = 20;
        private float textWidth = 300;
        private float textHeight = 200;

        public HelpOverlay(Node2D parent)
        {
            this.parent = parent;
        }

        public void Draw(InputManager inputManager)
        {
            if (!inputManager.ShowHelp) return;

            // Calculate position from bottom of screen
            float screenHeight = parent.GetViewportRect().Size.Y;
            float bottomMargin = 220;
            float startY = screenHeight - bottomMargin;
            float xPos = 10;

            // Draw background rectangle for better readability
            DrawBackgroundPanel(xPos, startY);

            // Add reminder about hiding instructions with H key
            parent.DrawString(ThemeDB.FallbackFont, new Vector2(xPos, startY), hideHint,
                            HorizontalAlignment.Left, -1, 16, Colors.Black);
            startY += lineHeight;

            // Draw controls section
            parent.DrawString(ThemeDB.FallbackFont, new Vector2(xPos, startY), controlsHeader,
                            HorizontalAlignment.Left, -1, 16, Colors.Black);
            startY += lineHeight;

            parent.DrawString(ThemeDB.FallbackFont, new Vector2(xPos, startY), controlR,
                            HorizontalAlignment.Left, -1, 16, Colors.Black);
            startY += lineHeight;

            parent.DrawString(ThemeDB.FallbackFont, new Vector2(xPos, startY), controlC,
                            HorizontalAlignment.Left, -1, 16, Colors.Black);
            startY += lineHeight;

            parent.DrawString(ThemeDB.FallbackFont, new Vector2(xPos, startY), controlPlus,
                            HorizontalAlignment.Left, -1, 16, Colors.Black);
            startY += lineHeight;

            parent.DrawString(ThemeDB.FallbackFont, new Vector2(xPos, startY), controlMinus,
                            HorizontalAlignment.Left, -1, 16, Colors.Black);
            startY += lineHeight * 1.5f; // Add extra space between sections

            // Draw status section
            parent.DrawString(ThemeDB.FallbackFont, new Vector2(xPos, startY), statusHeader,
                            HorizontalAlignment.Left, -1, 16, Colors.Black);
            startY += lineHeight;

            // Draw current speed
            string speedText = $"Speed: {inputManager.OrbitSpeedModifier:F1} x";
            parent.DrawString(ThemeDB.FallbackFont, new Vector2(xPos, startY), speedText,
                            HorizontalAlignment.Left, -1, 16, Colors.Black);
            startY += lineHeight;

            // Draw orbit direction
            string directionText = $"Orbit Direction: {(inputManager.ReverseOrbitRotation ? "Reverse" : "Forward")}";
            parent.DrawString(ThemeDB.FallbackFont, new Vector2(xPos, startY), directionText,
                            HorizontalAlignment.Left, -1, 16, Colors.Black);
            startY += lineHeight;

            // Draw color status
            string colorText = $"Colors: {(inputManager.SwapEyeCrossColors ? "Swapped" : "Normal")}";
            parent.DrawString(ThemeDB.FallbackFont, new Vector2(xPos, startY), colorText,
                            HorizontalAlignment.Left, -1, 16, Colors.Black);
        }

        private void DrawBackgroundPanel(float x, float y)
        {
            // Draw background rectangle with semi-transparent white
            parent.DrawRect(
                new Rect2(
                    x - padding,
                    y - padding - 10, // Extra top padding
                    textWidth + (padding * 2),
                    textHeight + (padding * 2) + 10 // Extra bottom padding
                ),
                new Color(1, 1, 1, 0.7f),
                true // Filled
            );

            // Draw border around the background
            parent.DrawRect(
                new Rect2(
                    x - padding,
                    y - padding - 10,
                    textWidth + (padding * 2),
                    textHeight + (padding * 2) + 10
                ),
                Colors.Black,
                false // Outline only
            );
        }
    }
}
