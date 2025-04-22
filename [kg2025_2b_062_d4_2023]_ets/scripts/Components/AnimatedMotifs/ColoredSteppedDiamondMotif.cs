using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;
using KG2025.Components;

// Use GodotVector2 alias to avoid ambiguity
using GodotVector2 = Godot.Vector2;

namespace KG2025.Components.AnimatedMotifs
{
    public class ColoredSteppedDiamondMotif : ColoredMotifBase
    {
        private GodotVector2 position;
        private float squareSize;
        private bool inverseBreathe;
        private float steppedDiamondBreathingFactor = 1.0f;

        public ColoredSteppedDiamondMotif(Node2D parent, KartesiusSystem kartesiusSystem,
                                        GodotVector2 position, float squareSize, bool inverseBreathe = false)
            : base(parent, kartesiusSystem)
        {
            this.position = position;
            this.squareSize = squareSize;
            this.inverseBreathe = inverseBreathe;
        }

        // Override the SetBreathingFactor to update the stepped-diamond-specific factor
        public override void SetBreathingFactor(float factor)
        {
            base.SetBreathingFactor(factor);
            this.steppedDiamondBreathingFactor = factor;
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            float phaseOffset = inverseBreathe ? Mathf.Pi : 0;
            float rawBreathing = (Mathf.Sin(breathingTime + phaseOffset) + 1) / 2;

            // Calculate maximum allowed scale factor for stepped diamond
            float verticalLineOffset = 110;
            float boundaryWidth = verticalLineOffset - 10;
            float maxSteppedDiamondWidth = 4 * squareSize;
            float maxAllowedScale = boundaryWidth / maxSteppedDiamondWidth;
            float constrainedMaxScale = Mathf.Min(maxScale, maxAllowedScale);

            // Calculate final breathing factor
            steppedDiamondBreathingFactor = minScale + rawBreathing * (constrainedMaxScale - minScale);
        }

        public override void Draw()
        {
            float scaledSize = squareSize * steppedDiamondBreathingFactor;

            // Fill color alternates between two colors for visual interest
            Color fillColor1 = colorPalette.DarkGreen;
            Color fillColor2 = colorPalette.DarkOrange;

            // Draw a diamond-shaped outline made of filled squares
            // Top square
            DrawFilledSquare(position.X, position.Y - 4 * scaledSize, scaledSize, fillColor1);

            // Top-right squares
            DrawFilledSquare(position.X + scaledSize, position.Y - 3 * scaledSize, scaledSize, fillColor2);
            DrawFilledSquare(position.X + 2 * scaledSize, position.Y - 2 * scaledSize, scaledSize, fillColor1);
            DrawFilledSquare(position.X + 3 * scaledSize, position.Y - scaledSize, scaledSize, fillColor2);

            // Right square
            DrawFilledSquare(position.X + 4 * scaledSize, position.Y, scaledSize, fillColor1);

            // Bottom-right squares
            DrawFilledSquare(position.X + 3 * scaledSize, position.Y + scaledSize, scaledSize, fillColor2);
            DrawFilledSquare(position.X + 2 * scaledSize, position.Y + 2 * scaledSize, scaledSize, fillColor1);
            DrawFilledSquare(position.X + scaledSize, position.Y + 3 * scaledSize, scaledSize, fillColor2);

            // Bottom square
            DrawFilledSquare(position.X, position.Y + 4 * scaledSize, scaledSize, fillColor1);

            // Bottom-left squares
            DrawFilledSquare(position.X - scaledSize, position.Y + 3 * scaledSize, scaledSize, fillColor2);
            DrawFilledSquare(position.X - 2 * scaledSize, position.Y + 2 * scaledSize, scaledSize, fillColor1);
            DrawFilledSquare(position.X - 3 * scaledSize, position.Y + scaledSize, scaledSize, fillColor2);

            // Left square
            DrawFilledSquare(position.X - 4 * scaledSize, position.Y, scaledSize, fillColor1);

            // Top-left squares
            DrawFilledSquare(position.X - 3 * scaledSize, position.Y - scaledSize, scaledSize, fillColor2);
            DrawFilledSquare(position.X - 2 * scaledSize, position.Y - 2 * scaledSize, scaledSize, fillColor1);
            DrawFilledSquare(position.X - scaledSize, position.Y - 3 * scaledSize, scaledSize, fillColor2);
        }

        private void DrawFilledSquare(float x, float y, float size, Color fillColor)
        {
            // Create square points
            GodotVector2[] square = new GodotVector2[4];
            square[0] = new GodotVector2(x, y);
            square[1] = new GodotVector2(x + size, y);
            square[2] = new GodotVector2(x + size, y + size);
            square[3] = new GodotVector2(x, y + size);

            // Draw filled square
            DrawFilledPolygon(square, fillColor);

            // Draw outline
            GodotVector2[] outline = new GodotVector2[5];
            outline[0] = square[0];
            outline[1] = square[1];
            outline[2] = square[2];
            outline[3] = square[3];
            outline[4] = square[0]; // Close the shape
            parent.DrawPolyline(outline, lineColor);
        }
    }
}
