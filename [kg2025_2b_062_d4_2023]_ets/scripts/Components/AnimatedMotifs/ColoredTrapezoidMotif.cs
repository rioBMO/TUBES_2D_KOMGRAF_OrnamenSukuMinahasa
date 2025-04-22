using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;
using KG2025.Components;

// Use GodotVector2 alias to avoid ambiguity
using GodotVector2 = Godot.Vector2;

namespace KG2025.Components.AnimatedMotifs
{
    public class ColoredTrapezoidMotif : ColoredMotifBase
    {
        private List<GodotVector2> leftTrapezoidPositions = new List<GodotVector2>();
        private List<GodotVector2> rightTrapezoidPositions = new List<GodotVector2>();
        private float trapezoidSize;
        private float trapezoidSpacing;
        private float trapezoidYOffset = 0f;
        private float trapezoidMovementSpeed;
        private int numTrapezoids;
        private float rotationOffset;
        private bool reverseMovement = false;

        public ColoredTrapezoidMotif(Node2D parent, KartesiusSystem kartesiusSystem,
                                   float trapezoidSize, float trapezoidSpacing,
                                   float movementSpeed, int numTrapezoids = 24, float rotationOffset = -Mathf.Pi / 6)
            : base(parent, kartesiusSystem)
        {
            this.trapezoidSize = trapezoidSize;
            this.trapezoidSpacing = trapezoidSpacing;
            this.trapezoidMovementSpeed = movementSpeed;
            this.numTrapezoids = numTrapezoids;
            this.rotationOffset = rotationOffset;

            SetupTrapezoidPositions();
        }

        private void SetupTrapezoidPositions()
        {
            leftTrapezoidPositions.Clear();
            rightTrapezoidPositions.Clear();

            float centerX = kartesiusSystem.KartesiusCenterX;
            float centerY = kartesiusSystem.KartesiusCenterY;
            float totalHeight = numTrapezoids * trapezoidSpacing;
            float startY = -totalHeight / 2;

            for (int i = 0; i < numTrapezoids; i++)
            {
                float y = startY + i * trapezoidSpacing;

                // Alternate positions between -800 and -900 as in Karya3
                if (i % 2 == 0)
                {
                    leftTrapezoidPositions.Add(new GodotVector2(centerX - 800, y));
                }
                else
                {
                    leftTrapezoidPositions.Add(new GodotVector2(centerX - 900, y));
                }

                // Alternate positions between 800 and 900
                if (i % 2 == 0)
                {
                    rightTrapezoidPositions.Add(new GodotVector2(centerX + 800, y));
                }
                else
                {
                    rightTrapezoidPositions.Add(new GodotVector2(centerX + 900, y));
                }
            }
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            // Update trapezoid animation offset for continuous movement
            trapezoidYOffset += delta * trapezoidMovementSpeed;

            // Reset the offset when it exceeds the total height to create a seamless loop
            float totalHeight = numTrapezoids * trapezoidSpacing;
            if (trapezoidYOffset > totalHeight)
            {
                trapezoidYOffset -= totalHeight;
            }
        }

        public void SetYOffset(float offset)
        {
            this.trapezoidYOffset = offset;
        }

        public void SetReverseMovement(bool reverse)
        {
            this.reverseMovement = reverse;
        }

        public override void Draw()
        {
            float centerX = kartesiusSystem.KartesiusCenterX;
            float centerY = kartesiusSystem.KartesiusCenterY;
            float topY = centerY - 700;
            float bottomY = centerY + 700;

            // Draw background rectangles
            DrawBackgroundRect(centerX - 900, topY, centerX - 760, bottomY, colorPalette.MediumGreen);
            DrawBackgroundRect(centerX + 760, topY, centerX + 900, bottomY, colorPalette.MediumGreen);

            // Draw boundary rectangles
            DrawBorderRect(centerX - 800, topY, centerX - 760, bottomY, colorPalette.DarkGreen);
            DrawBorderRect(centerX - 942, topY, centerX - 900, bottomY, colorPalette.DarkGreen);
            DrawBorderRect(centerX + 760, topY, centerX + 800, bottomY, colorPalette.DarkGreen);
            DrawBorderRect(centerX + 900, topY, centerX + 942, bottomY, colorPalette.DarkGreen);

            // Draw horizontal dividers
            DrawHorizontalBorderRect(centerX - 760, centerY - 30, centerX - 150, centerY + 30, colorPalette.MediumGreen);
            DrawHorizontalBorderRect(centerX + 150, centerY - 30, centerX + 760, centerY + 30, colorPalette.LightOrange);

            // Draw left-moving trapezoids
            DrawLeftTrapezoids(centerY);

            // Draw right-moving trapezoids
            DrawRightTrapezoids(centerY);
        }

        private void DrawLeftTrapezoids(float centerY)
        {
            float totalHeight = numTrapezoids * trapezoidSpacing;

            for (int i = 0; i < leftTrapezoidPositions.Count; i++)
            {
                GodotVector2 basePos = leftTrapezoidPositions[i];

                // Apply movement offset based on direction
                float yPos;
                if (reverseMovement)
                {
                    // Moving downward for left side when reversed
                    yPos = basePos.Y + trapezoidYOffset;
                }
                else
                {
                    // Moving upward for left side normally
                    yPos = basePos.Y - trapezoidYOffset;
                }

                // Wrap around when moving off the screen
                if (yPos < centerY - totalHeight / 2)
                    yPos += totalHeight;
                else if (yPos > centerY + totalHeight / 2)
                    yPos -= totalHeight;

                // Draw the trapezoid with alternating orientation
                bool mirror = i % 2 != 0;
                DrawColoredTrapezoidAt(basePos.X, yPos, trapezoidSize, rotationOffset, mirror);
            }
        }

        private void DrawRightTrapezoids(float centerY)
        {
            float totalHeight = numTrapezoids * trapezoidSpacing;

            for (int i = 0; i < rightTrapezoidPositions.Count; i++)
            {
                GodotVector2 basePos = rightTrapezoidPositions[i];

                // Apply movement offset based on direction (opposite of left side)
                float yPos;
                if (reverseMovement)
                {
                    // Moving upward for right side when reversed
                    yPos = basePos.Y - trapezoidYOffset;
                }
                else
                {
                    // Moving downward for right side normally
                    yPos = basePos.Y + trapezoidYOffset;
                }

                // Wrap around when moving off the screen
                if (yPos < centerY - totalHeight / 2)
                    yPos += totalHeight;
                else if (yPos > centerY + totalHeight / 2)
                    yPos -= totalHeight;

                // Draw the trapezoid with alternating orientation
                bool mirror = i % 2 == 0;
                DrawColoredTrapezoidAt(basePos.X, yPos, trapezoidSize, rotationOffset, mirror);
            }
        }

        private void DrawColoredTrapezoidAt(float x, float y, float size, float rotationOffset = 0f, bool mirrorShape = false)
        {
            // Create hexagon points for the trapezoid base
            GodotVector2[] hexPoints = new GodotVector2[6];
            for (int i = 0; i < 6; i++)
            {
                float angle = i * Mathf.Pi / 3 + rotationOffset;
                hexPoints[i] = new GodotVector2(
                    x + size * Mathf.Cos(angle),
                    y + size * Mathf.Sin(angle)
                );
            }

            // Extract 4 points for trapezoid
            GodotVector2[] trapezoid = new GodotVector2[4];
            if (!mirrorShape)
            {
                // Original orientation
                trapezoid[0] = hexPoints[2];
                trapezoid[1] = hexPoints[3];
                trapezoid[2] = hexPoints[4];
                trapezoid[3] = hexPoints[5];
            }
            else
            {
                // Mirrored orientation
                trapezoid[0] = hexPoints[5];
                trapezoid[1] = hexPoints[0];
                trapezoid[2] = hexPoints[1];
                trapezoid[3] = hexPoints[2];
            }

            // Create layers with different scales and colors
            Color[] colors = { colorPalette.DarkGreen, colorPalette.Cream, colorPalette.DarkGreen, colorPalette.Cream };
            float[] scales = { 1.0f, 0.8f, 0.6f, 0.3f };

            // Draw from OUTSIDE-IN
            for (int j = 0; j < scales.Length; j++)
            {
                float scale = scales[j];
                GodotVector2[] layerTrapezoid = new GodotVector2[4];

                for (int i = 0; i < 4; i++)
                {
                    if (j == 0) // outermost layer uses original points
                    {
                        layerTrapezoid[i] = trapezoid[i];
                    }
                    else // inner layers are scaled
                    {
                        GodotVector2 dir = (trapezoid[i] - new GodotVector2(x, y)) * scale;
                        layerTrapezoid[i] = new GodotVector2(x, y) + dir;
                    }
                }

                // Draw filled trapezoid for this layer
                DrawFilledPolygon(layerTrapezoid, colors[j]);

                // Draw outline
                GodotVector2[] outline = new GodotVector2[5];
                outline[0] = layerTrapezoid[0];
                outline[1] = layerTrapezoid[1];
                outline[2] = layerTrapezoid[2];
                outline[3] = layerTrapezoid[3];
                outline[4] = layerTrapezoid[0]; // Close the shape
                parent.DrawPolyline(outline, lineColor);
            }
        }
    }
}
