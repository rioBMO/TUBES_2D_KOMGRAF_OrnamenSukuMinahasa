using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;
using KG2025.Components;

// Use GodotVector2 alias to avoid ambiguity with System.Numerics.Vector2
using GodotVector2 = Godot.Vector2;

namespace KG2025.Components.AnimatedMotifs
{
    public class ColoredCrossMotif : ColoredMotifBase
    {
        private GodotVector2 centerPosition;
        private float patternSize;
        private float orbitRadius;
        private int numPatterns;
        private List<GodotVector2> orbitPositions = new List<GodotVector2>();

        public ColoredCrossMotif(Node2D parent, KartesiusSystem kartesiusSystem, GodotVector2 centerPos,
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
            DrawCrossPattern(centerPosition.X, centerPosition.Y, patternSize * 1.1f);

            // Draw orbiting patterns
            DrawOrbitingCrossPatterns();
        }

        private void DrawOrbitingCrossPatterns()
        {
            // For each position, calculate its new orbit position
            for (int i = 0; i < numPatterns; i++)
            {
                float baseAngle = (i * (2 * Mathf.Pi / numPatterns));
                float currentAngle = baseAngle + orbitAngle;

                // Calculate new position on the orbit
                float x = centerPosition.X + orbitRadius * Mathf.Cos(currentAngle);
                float y = centerPosition.Y + orbitRadius * Mathf.Sin(currentAngle);

                // Draw cross pattern at its orbit position using the same scale as Karya3
                DrawCrossPattern(x, y, patternSize * 0.8f);
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

        private void DrawCrossPattern(float x, float y, float size)
        {
            // Draw filled hexagon with current theme colors
            DrawFilledHexagon(x, y, size, colorPalette.CrossHexFillColor, colorPalette.CrossHexOutlineColor);

            // Define the cross dimensions
            float crossWidthSize = size * 0.5f;
            float crossHeightSize = size;

            // Draw filled cross with current theme colors
            DrawFilledCrossShapeAt(x, y, crossWidthSize, crossHeightSize, colorPalette.CrossFillColor);

            // Draw inner circle with current theme colors
            DrawFilledCircle(x, y, size * 0.15f, colorPalette.CrossInnerCircleColor);
            DrawCircle(new GodotVector2(x, y), size * 0.15f, colorPalette.CrossInnerCircleColor, false);
        }

        private void DrawFilledCrossShapeAt(float x, float y, float width, float height, Color fillColor)
        {
            // Define the cross/plus shape dimensions
            float halfWidth = width / 2;
            float halfHeight = height / 2;

            // Horizontal rectangle of the cross
            GodotVector2[] horizontalRect = new GodotVector2[4];
            horizontalRect[0] = new GodotVector2(x - halfHeight, y - halfWidth);
            horizontalRect[1] = new GodotVector2(x + halfHeight, y - halfWidth);
            horizontalRect[2] = new GodotVector2(x + halfHeight, y + halfWidth);
            horizontalRect[3] = new GodotVector2(x - halfHeight, y + halfWidth);

            // Vertical rectangle of the cross
            GodotVector2[] verticalRect = new GodotVector2[4];
            verticalRect[0] = new GodotVector2(x - halfWidth, y - halfHeight);
            verticalRect[1] = new GodotVector2(x + halfWidth, y - halfHeight);
            verticalRect[2] = new GodotVector2(x + halfWidth, y + halfHeight);
            verticalRect[3] = new GodotVector2(x - halfWidth, y + halfHeight);

            // Draw both filled rectangles
            DrawFilledPolygon(horizontalRect, fillColor);
            DrawFilledPolygon(verticalRect, fillColor);

            // Draw outlines
            Rect2 hRect = new Rect2(x - halfHeight, y - halfWidth, height, width);
            Rect2 vRect = new Rect2(x - halfWidth, y - halfHeight, width, height);
            parent.DrawRect(hRect, lineColor, false);
            parent.DrawRect(vRect, lineColor, false);
        }
    }
}
