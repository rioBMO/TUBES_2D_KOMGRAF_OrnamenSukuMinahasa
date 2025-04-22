using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;
using KG2025.Components;

// Use GodotVector2 alias to avoid ambiguity with System.Numerics.Vector2
using GodotVector2 = Godot.Vector2;

namespace KG2025.Components.AnimatedMotifs
{
    public class ColoredEyeMotif : ColoredMotifBase
    {
        private GodotVector2 centerPosition;
        private float patternSize;
        private float orbitRadius;
        private int numPatterns;
        private List<GodotVector2> orbitPositions = new List<GodotVector2>();

        public ColoredEyeMotif(Node2D parent, KartesiusSystem kartesiusSystem, GodotVector2 centerPos,
                            float patternSize, float orbitRadius, int numPatterns = 6)
            : base(parent, kartesiusSystem)
        {
            this.centerPosition = centerPos;
            this.patternSize = patternSize;
            this.orbitRadius = orbitRadius;
            this.numPatterns = numPatterns;

            // Initialize orbit positions
            SetupOrbitPositions();
        }

        private void SetupOrbitPositions()
        {
            orbitPositions.Clear();
            for (int i = 0; i < numPatterns; i++)
            {
                float angle = i * (2 * Mathf.Pi / numPatterns);
                orbitPositions.Add(new GodotVector2(
                    centerPosition.X + orbitRadius * Mathf.Cos(angle),
                    centerPosition.Y + orbitRadius * Mathf.Sin(angle)
                ));
            }
        }

        public override void Draw()
        {
            // Draw center pattern with exactly the same scale as in Karya3
            DrawEyePattern(centerPosition.X, centerPosition.Y, patternSize * 1.1f);

            // Draw orbiting patterns
            DrawOrbitingEyePatterns();
        }

        private void DrawOrbitingEyePatterns()
        {
            // For each position, calculate its new orbit position
            for (int i = 0; i < numPatterns; i++)
            {
                float baseAngle = (i * (2 * Mathf.Pi / numPatterns));
                float currentAngle = baseAngle + orbitAngle;

                // Calculate new position on the orbit
                float x = centerPosition.X + orbitRadius * Mathf.Cos(currentAngle);
                float y = centerPosition.Y + orbitRadius * Mathf.Sin(currentAngle);

                // Draw eye pattern at its orbit position using the same scale as Karya3
                DrawEyePattern(x, y, patternSize * 0.8f);
            }
        }

        // Add these methods to support external control of animation parameters
        public void SetOrbitAngle(float angle)
        {
            this.orbitAngle = angle;
        }

        public void SetBreathingFactor(float factor)
        {
            this.breathingFactor = factor;
        }

        private void DrawEyePattern(float x, float y, float size)
        {
            // Draw filled hexagon with current theme colors
            DrawFilledHexagon(x, y, size, colorPalette.EyeHexFillColor, colorPalette.EyeHexOutlineColor);

            // Draw outer circle with current theme colors
            DrawFilledCircle(x, y, size * 0.6f, colorPalette.EyeOuterCircleColor);
            DrawCircle(new GodotVector2(x, y), size * 0.6f, colorPalette.EyeOuterCircleColor, false);

            // Draw inner circle with current theme colors
            DrawFilledCircle(x, y, size * 0.15f, colorPalette.EyeInnerCircleColor);
            DrawCircle(new GodotVector2(x, y), size * 0.15f, colorPalette.EyeInnerCircleColor, false);
        }
    }
}
