using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;

namespace KG2025.Components.Motifs
{
    public class CrossMotif : MotifBase
    {
        public CrossMotif(Node2D parent, KartesiusSystem kartesiusSystem) : base(parent, kartesiusSystem) { }

        public override void Draw(float x, float y, float size)
        {
            // Create a transformasi instance for matrix operations
            Transformasi transformasi = new Transformasi();

            // Initialize transformation matrix
            float[,] transformMatrix = new float[3, 3];
            Transformasi.Matrix3x3Identity(transformMatrix);

            // Calculate sizes based on the given overall size
            float hexSize = size * 0.9f; // Adjust to match the eye pattern size
            float crossWidthSize = size * 0.5f;
            float crossHeightSize = size * 1.0f;
            float innerCircleSize = size * 0.15f;

            // Center hexagon position
            Vector2 centerPos = new Vector2(x, y);

            // Draw the center cross pattern
            DrawSingleCrossPattern(centerPos.X, centerPos.Y, hexSize, crossWidthSize, crossHeightSize, innerCircleSize, transformMatrix);

            // Calculate positions for surrounding hexagons in first ring
            // We'll create 6 hexagons around the center one, just like in the eye pattern
            float distanceFromCenter = hexSize * 1.8f; // Distance from center to surrounding hexagons

            for (int i = 0; i < 6; i++)
            {
                // Reset transformation matrix for each hexagon
                Transformasi.Matrix3x3Identity(transformMatrix);

                float angle = i * Mathf.Pi / 3; // 60 degrees in radians (6 directions for hexagonal pattern)
                Vector2 hexPos = new Vector2(
                    x + distanceFromCenter * Mathf.Cos(angle),
                    y + distanceFromCenter * Mathf.Sin(angle)
                );

                // Draw each surrounding cross pattern
                DrawSingleCrossPattern(hexPos.X, hexPos.Y, hexSize, crossWidthSize, crossHeightSize, innerCircleSize, transformMatrix);
            }
        }

        // Helper method to draw a single cross pattern (hexagon with cross and circle)
        private void DrawSingleCrossPattern(float x, float y, float hexSize, float crossWidth, float crossHeight, float innerCircleSize, float[,] transformMatrix = null)
        {
            // Create a Transformasi instance if needed
            Transformasi transformasi = new Transformasi();

            // If no transform matrix provided, create an identity matrix
            if (transformMatrix == null)
            {
                transformMatrix = new float[3, 3];
                Transformasi.Matrix3x3Identity(transformMatrix);
            }

            // Generate hexagon points
            Vector2[] hexPoints = new Vector2[6];
            List<Vector2> hexPointsList = new List<Vector2>();

            float angleOffset = Mathf.Pi / 2;
            for (int i = 0; i < 6; i++)
            {
                float angle = i * Mathf.Pi / 3 + angleOffset; // 60 degrees in radians
                Vector2 point = new Vector2(
                    x + hexSize * Mathf.Cos(angle),
                    y + hexSize * Mathf.Sin(angle)
                );

                hexPointsList.Add(point);
            }

            // Apply transformation to the hexagon points if needed
            if (transformMatrix != null)
            {
                // Create a copy of the transform matrix to avoid modifying the original
                float[,] workingMatrix = new float[3, 3];
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        workingMatrix[i, j] = transformMatrix[i, j];
                    }
                }

                // Transform the points
                List<Vector2> transformedPoints = Transformasi.GetTransformPoint(workingMatrix, hexPointsList);

                // Update the hexPoints array with transformed points
                for (int i = 0; i < 6; i++)
                {
                    hexPoints[i] = transformedPoints[i];
                }
            }
            else
            {
                // If no transformation, just copy the points
                for (int i = 0; i < 6; i++)
                {
                    hexPoints[i] = hexPointsList[i];
                }
            }

            // Draw the hexagon outline
            for (int i = 0; i < hexPoints.Length; i++)
            {
                int nextIndex = (i + 1) % hexPoints.Length;
                List<Vector2> linePoints = DrawingUtils.LineDDA(
                    hexPoints[i].X, hexPoints[i].Y,
                    hexPoints[nextIndex].X, hexPoints[nextIndex].Y
                );

                // Draw each point in the line
                DrawingUtils.PutPixelAll(parent, linePoints, lineColor);
            }

            // Calculate transformed center for cross and circle
            Vector2 center = new Vector2(x, y);
            if (transformMatrix != null)
            {
                // Apply the same transformation to the center
                List<Vector2> centerPoint = new List<Vector2> { center };
                List<Vector2> transformedCenter = Transformasi.GetTransformPoint(transformMatrix, centerPoint);
                center = transformedCenter[0];
            }

            // Draw cross/plus shape
            DrawCrossShapeAt(center.X, center.Y, crossWidth, crossHeight, transformMatrix);

            // Draw inner circle outline
            DrawCircle(center.X, center.Y, innerCircleSize);
        }

        // Draw a cross shape at the specified position with optional transformation
        private void DrawCrossShapeAt(float x, float y, float width, float height, float[,] transformMatrix = null)
        {
            // Create a Transformasi instance if needed
            Transformasi transformasi = new Transformasi();

            // Define the cross/plus shape dimensions
            float halfWidth = width / 2;
            float halfHeight = height / 2;

            // Horizontal rectangle of the cross
            Vector2[] horizontalRect = new Vector2[4]
            {
                new Vector2(x - halfHeight, y - halfWidth),
                new Vector2(x + halfHeight, y - halfWidth),
                new Vector2(x + halfHeight, y + halfWidth),
                new Vector2(x - halfHeight, y + halfWidth)
            };

            // Vertical rectangle of the cross
            Vector2[] verticalRect = new Vector2[4]
            {
                new Vector2(x - halfWidth, y - halfHeight),
                new Vector2(x + halfWidth, y - halfHeight),
                new Vector2(x + halfWidth, y + halfHeight),
                new Vector2(x - halfWidth, y + halfHeight)
            };

            // Apply transformations if provided
            if (transformMatrix != null)
            {
                // Create lists for transformation
                List<Vector2> horizontalPoints = new List<Vector2>(horizontalRect);
                List<Vector2> verticalPoints = new List<Vector2>(verticalRect);

                // Create a copy of the transform matrix to avoid modifying the original
                float[,] workingMatrix = new float[3, 3];
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        workingMatrix[i, j] = transformMatrix[i, j];
                    }
                }

                // Transform the points
                List<Vector2> transformedHorizontal = Transformasi.GetTransformPoint(workingMatrix, horizontalPoints);
                List<Vector2> transformedVertical = Transformasi.GetTransformPoint(workingMatrix, verticalPoints);

                // Update the rectangle arrays with transformed points
                for (int i = 0; i < 4; i++)
                {
                    horizontalRect[i] = transformedHorizontal[i];
                    verticalRect[i] = transformedVertical[i];
                }
            }

            // Add closing point to make it a loop
            Vector2[] horizontalRectClosed = new Vector2[5];
            Vector2[] verticalRectClosed = new Vector2[5];

            for (int i = 0; i < 4; i++)
            {
                horizontalRectClosed[i] = horizontalRect[i];
                verticalRectClosed[i] = verticalRect[i];
            }
            horizontalRectClosed[4] = horizontalRect[0];
            verticalRectClosed[4] = verticalRect[0];

            // Draw the horizontal part of the cross
            DrawPolygon(horizontalRectClosed);

            // Draw the vertical part of the cross
            DrawPolygon(verticalRectClosed);
        }
    }
}
