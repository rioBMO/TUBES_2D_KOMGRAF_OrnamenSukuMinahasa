using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;

namespace KG2025.Components.Motifs
{
    public class EyeMotif : MotifBase
    {
        public EyeMotif(Node2D parent, KartesiusSystem kartesiusSystem) : base(parent, kartesiusSystem) { }

        public override void Draw(float x, float y, float size)
        {
            // Create a transformasi instance for matrix operations
            Transformasi transformasi = new Transformasi();

            // Initialize transformation matrix
            float[,] transformMatrix = new float[3, 3];
            Transformasi.Matrix3x3Identity(transformMatrix);

            // Calculate sizes based on the given overall size
            float hexSize = size * 0.9f; // Smaller hexagons to fit more in the pattern
            float outerCircleSize = hexSize * 0.7f;
            float innerCircleSize = hexSize * 0.2f;

            // Center hexagon position
            Vector2 centerPos = new Vector2(x, y);

            // Draw the center hexagon with circles
            DrawSingleEyePattern(centerPos.X, centerPos.Y, hexSize, outerCircleSize, innerCircleSize, transformMatrix);

            // Calculate positions for surrounding hexagons in first ring
            // We'll create 6 hexagons around the center one
            float distanceFromCenter = hexSize * 1.8f; // Distance from center to surrounding hexagons

            for (int i = 0; i < 6; i++)
            {
                // Reset transformation matrix for each hexagon
                Transformasi.Matrix3x3Identity(transformMatrix);

                float angle = i * Mathf.Pi / 3; // 60 degrees in radians
                Vector2 hexPos = new Vector2(
                    x + distanceFromCenter * Mathf.Cos(angle),
                    y + distanceFromCenter * Mathf.Sin(angle)
                );

                // Apply rotation to make the pattern more interesting
                Vector2 rotationCenter = hexPos;

                // Draw each surrounding hexagon with circles
                DrawSingleEyePattern(hexPos.X, hexPos.Y, hexSize, outerCircleSize, innerCircleSize, transformMatrix);

                // Add a second ring of hexagons (optional, based on size)
                if (size > 80) // Only add second ring for larger motifs
                {
                    // Add a second hexagon further out in this direction
                    float outerDistance = distanceFromCenter * 2;
                    Vector2 outerHexPos = new Vector2(
                        x + outerDistance * Mathf.Cos(angle),
                        y + outerDistance * Mathf.Sin(angle)
                    );

                    // Reset and create a new transformation for the outer hexagon
                    Transformasi.Matrix3x3Identity(transformMatrix);
                    rotationCenter = outerHexPos;

                    // Apply a different rotation and scaling to the outer ring
                    transformasi.RotationCounterClockwise(transformMatrix, 15 * i, rotationCenter);
                    Vector2 scaleCenter = outerHexPos;
                    transformasi.Scaling(transformMatrix, 0.8f, 0.8f, scaleCenter); // Slightly smaller

                    // Draw the outer hexagon
                    DrawSingleEyePattern(outerHexPos.X, outerHexPos.Y, hexSize * 0.8f, outerCircleSize * 0.8f, innerCircleSize * 0.8f, transformMatrix);
                }
            }
        }

        // Helper method to draw a single eye pattern (hexagon with circles)
        private void DrawSingleEyePattern(float x, float y, float hexSize, float outerCircleSize, float innerCircleSize, float[,] transformMatrix = null)
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
                float angle = i * Mathf.Pi / 3 + angleOffset;
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

            // Calculate transformed circle center
            Vector2 circleCenter = new Vector2(x, y);
            if (transformMatrix != null)
            {
                // Apply the same transformation to the circle center
                List<Vector2> centerPoint = new List<Vector2> { circleCenter };
                List<Vector2> transformedCenter = Transformasi.GetTransformPoint(transformMatrix, centerPoint);
                circleCenter = transformedCenter[0];
            }

            // Draw outer circle
            DrawCircle(circleCenter.X, circleCenter.Y, outerCircleSize);

            // Draw inner circle outline
            DrawCircle(circleCenter.X, circleCenter.Y, innerCircleSize, Colors.White);
        }
    }
}
