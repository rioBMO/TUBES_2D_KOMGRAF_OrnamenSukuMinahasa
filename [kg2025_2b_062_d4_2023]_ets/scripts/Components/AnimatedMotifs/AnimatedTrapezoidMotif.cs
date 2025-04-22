using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;
using KG2025.Components;

namespace KG2025.Components.AnimatedMotifs
{
    public class AnimatedTrapezoidMotif : AnimatedMotifBase
    {
        private List<Vector2> leftTrapezoidPositions = new List<Vector2>();
        private List<Vector2> rightTrapezoidPositions = new List<Vector2>();
        private float trapezoidSize;
        private float trapezoidSpacing;
        private float trapezoidYOffset = 0f;
        private float trapezoidMovementSpeed;
        private int numTrapezoids;
        private float rotationOffset;
        
        public AnimatedTrapezoidMotif(Node2D parent, KartesiusSystem kartesiusSystem, 
            float trapezoidSize, float trapezoidSpacing, float movementSpeed, int numTrapezoids = 24, float rotationOffset = -Mathf.Pi/6) 
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

            // Create positions for left column of trapezoids
            float totalHeight = numTrapezoids * trapezoidSpacing;
            float startY = -totalHeight / 2;

            for (int i = 0; i < numTrapezoids; i++)
            {
                float y = startY + i * trapezoidSpacing;

                // Alternate positions between -800 and -900 as in Karya1
                if (i % 2 == 0)
                {
                    leftTrapezoidPositions.Add(new Vector2(centerX - 800, y));
                }
                else
                {
                    leftTrapezoidPositions.Add(new Vector2(centerX - 900, y));
                }
            }

            // Create positions for right column of trapezoids
            for (int i = 0; i < numTrapezoids; i++)
            {
                float y = startY + i * trapezoidSpacing;

                // Alternate positions between 800 and 900 as in Karya1
                if (i % 2 == 0)
                {
                    rightTrapezoidPositions.Add(new Vector2(centerX + 800, y));
                }
                else
                {
                    rightTrapezoidPositions.Add(new Vector2(centerX + 900, y));
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
        
        public override void Draw()
        {
            float centerY = kartesiusSystem.KartesiusCenterY;
            
            // Define boundary dimensions exactly as in Karya1
            float topY = centerY - 700;
            float bottomY = centerY + 700;

            // Draw boundary lines
            DrawBoundaryLines(topY, bottomY);
            
            // Draw horizontal divider lines
            DrawHorizontalDividers();
            
            // Draw left-side trapezoids (moving upward)
            DrawLeftSideTrapezoids();
            
            // Draw right-side trapezoids (moving downward)
            DrawRightSideTrapezoids();
        }
        
        private void DrawBoundaryLines(float topY, float bottomY)
        {
            float centerX = kartesiusSystem.KartesiusCenterX;
            
            // Left side boundary lines
            DrawVerticalLine(centerX - 800, topY, bottomY);
            DrawVerticalLine(centerX - 760, topY, bottomY);
            DrawVerticalLine(centerX - 942, topY, bottomY);
            DrawVerticalLine(centerX - 900, topY, bottomY);

            // Right side boundary lines
            DrawVerticalLine(centerX + 760, topY, bottomY);
            DrawVerticalLine(centerX + 800, topY, bottomY);
            DrawVerticalLine(centerX + 900, topY, bottomY);
            DrawVerticalLine(centerX + 942, topY, bottomY);
        }
        
        private void DrawHorizontalDividers()
        {
            float centerX = kartesiusSystem.KartesiusCenterX;
            float centerY = kartesiusSystem.KartesiusCenterY;
            
            // Left side horizontal divider lines
            DrawHorizontalLine(centerX - 456, centerY + 20, 609);
            DrawHorizontalLine(centerX - 456, centerY - 20, 609);

            // Right side horizontal divider lines
            DrawHorizontalLine(centerX + 456, centerY + 20, 609);
            DrawHorizontalLine(centerX + 456, centerY - 20, 609);
        }
        
        private void DrawLeftSideTrapezoids()
        {
            float centerY = kartesiusSystem.KartesiusCenterY;
            float totalHeight = numTrapezoids * trapezoidSpacing;
            
            // Draw all left trapezoids with continuous upward movement
            for (int i = 0; i < leftTrapezoidPositions.Count; i++)
            {
                Vector2 basePos = leftTrapezoidPositions[i];

                // Apply the continuous upward movement offset
                float yPos = basePos.Y - trapezoidYOffset;

                // Wrap around when moving off the screen
                if (yPos < centerY - totalHeight / 2)
                    yPos += totalHeight;

                // Draw the trapezoid with alternating orientation
                bool mirror = i % 2 != 0;
                DrawTrapezoidLayersAt(basePos.X, yPos, trapezoidSize, rotationOffset, mirror);
            }
        }
        
        private void DrawRightSideTrapezoids()
        {
            float centerY = kartesiusSystem.KartesiusCenterY;
            float totalHeight = numTrapezoids * trapezoidSpacing;
            
            // Draw all right trapezoids with continuous downward movement
            for (int i = 0; i < rightTrapezoidPositions.Count; i++)
            {
                Vector2 basePos = rightTrapezoidPositions[i];

                // Apply continuous downward movement offset
                float yPos = basePos.Y + trapezoidYOffset;

                // Wrap around when moving off the screen
                if (yPos > centerY + totalHeight / 2)
                    yPos -= totalHeight;

                // Draw the trapezoid with alternating orientation
                bool mirror = i % 2 == 0;
                DrawTrapezoidLayersAt(basePos.X, yPos, trapezoidSize, rotationOffset, mirror);
            }
        }
        
        private void DrawTrapezoidLayersAt(float x, float y, float size, float rotationOffset = 0f, bool mirrorShape = false)
        {
            // Create hexagon points as the base for the trapezoid
            Vector2[] hexPoints = new Vector2[6];
            for (int i = 0; i < 6; i++)
            {
                float angle = i * Mathf.Pi / 3 + rotationOffset;
                hexPoints[i] = new Vector2(
                    x + size * Mathf.Cos(angle),
                    y + size * Mathf.Sin(angle)
                );
            }

            // Extract 4 points for trapezoid
            Vector2[] trapezoid = new Vector2[5];

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

            trapezoid[4] = trapezoid[0]; // Close shape

            // Draw the main trapezoid outline
            DrawPolygon(trapezoid);

            // Draw inner trapezoid layers
            float[] scales = { 0.8f, 0.6f, 0.3f };

            // Draw each inner layer
            for (int j = 0; j < scales.Length; j++)
            {
                float scale = scales[j];
                Vector2[] innerTrapezoid = new Vector2[5];

                for (int i = 0; i < 4; i++)
                {
                    Vector2 dir = (trapezoid[i] - new Vector2(x, y)) * scale;
                    innerTrapezoid[i] = new Vector2(x, y) + dir;
                }
                innerTrapezoid[4] = innerTrapezoid[0];

                DrawPolygon(innerTrapezoid);
            }
        }
        
        private void DrawVerticalLine(float x, float topY, float bottomY)
        {
            // Draw the vertical line
            List<Vector2> linePoints = DrawingUtils.LineDDA(x, topY, x, bottomY);
            DrawingUtils.PutPixelAll(parent, linePoints, lineColor);

            // Add a second line right next to the first for thicker appearance
            List<Vector2> thickerLinePoints = DrawingUtils.LineDDA(x + 1, topY, x + 1, bottomY);
            DrawingUtils.PutPixelAll(parent, thickerLinePoints, lineColor);
        }
        
        private void DrawHorizontalLine(float x, float y, float length)
        {
            // Draw the main horizontal line
            List<Vector2> linePoints = DrawingUtils.LineDDA(
                x - length / 2, y,
                x + length / 2, y
            );
            DrawingUtils.PutPixelAll(parent, linePoints, lineColor);

            // Draw a second line for thickness
            List<Vector2> thickerLinePoints = DrawingUtils.LineDDA(
                x - length / 2, y + 1,
                x + length / 2, y + 1
            );
            DrawingUtils.PutPixelAll(parent, thickerLinePoints, lineColor);
        }
    }
}
