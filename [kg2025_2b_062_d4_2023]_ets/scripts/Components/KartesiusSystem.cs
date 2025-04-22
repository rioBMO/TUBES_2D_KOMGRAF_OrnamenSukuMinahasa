using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;

namespace KG2025.Components
{
    public class KartesiusSystem
    {
        private Node2D parent;

        // Kartesius system properties
        public bool ShowKartesius { get; set; } = true;
        public int MarginLeft { get; set; } = 50;
        public int MarginTop { get; set; } = 50;
        public int MarginRight { get; set; } = 50;
        public int MarginBottom { get; set; } = 50;
        public bool ShowGrid { get; set; } = false;
        public int GridSpacing { get; set; } = 50;
        public Color GridColor { get; set; } = new Color(0.8f, 0.8f, 0.8f, 0.5f);
        public Color AxisColor { get; set; } = new Color(0.5f, 0.5f, 0.5f);

        public float KartesiusCenterX { get; private set; }
        public float KartesiusCenterY { get; private set; }
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }

        public KartesiusSystem(Node2D parent)
        {
            this.parent = parent;
            UpdateScreenDimensions();
        }

        public void UpdateScreenDimensions()
        {
            ScreenWidth = (int)parent.GetViewportRect().Size.X;
            ScreenHeight = (int)parent.GetViewportRect().Size.Y;

            // Calculate Kartesius center
            KartesiusCenterX = ScreenWidth / 2;
            KartesiusCenterY = ScreenHeight / 2;
        }

        // Convert Cartesian coordinates to pixel coordinates
        public Vector2 ConvertToPixel(float x, float y)
        {
            // In Cartesian system, positive Y is upward but in pixel coordinates, positive Y is downward
            return new Vector2(
                KartesiusCenterX + x,
                KartesiusCenterY - y
            );
        }

        // Convert pixel coordinates to Cartesian coordinates
        public Vector2 ConvertToCartesian(float pixelX, float pixelY)
        {
            return new Vector2(
                pixelX - KartesiusCenterX,
                KartesiusCenterY - pixelY
            );
        }

        // Draw the Cartesian coordinate system
        public void DrawKartesius()
        {
            var kartesiusPoints = GambarKartesius();

            // Draw all points in the Cartesian system
            foreach (var point in kartesiusPoints)
            {
                DrawingUtils.PutPixel(parent, point.X, point.Y, AxisColor);
            }

            // Optional: Draw quadrant labels
            // DrawQuadrantLabels();
        }

        // Generate points for the Cartesian coordinate system
        private List<Vector2> GambarKartesius()
        {
            List<Vector2> res = new List<Vector2>();

            // Draw X axis (horizontal)
            res.AddRange(DrawingUtils.LineDDA(MarginLeft, KartesiusCenterY, ScreenWidth - MarginRight, KartesiusCenterY));

            // Draw Y axis (vertical)
            res.AddRange(DrawingUtils.LineDDA(KartesiusCenterX, MarginTop, KartesiusCenterX, ScreenHeight - MarginBottom));

            // Draw grid if enabled
            if (ShowGrid)
            {
                // Vertical grid lines (parallel to Y-axis)
                for (float x = KartesiusCenterX + GridSpacing; x < ScreenWidth - MarginRight; x += GridSpacing)
                {
                    res.AddRange(DrawingUtils.LineDDA(x, MarginTop, x, ScreenHeight - MarginBottom));
                }

                for (float x = KartesiusCenterX - GridSpacing; x > MarginLeft; x -= GridSpacing)
                {
                    res.AddRange(DrawingUtils.LineDDA(x, MarginTop, x, ScreenHeight - MarginBottom));
                }

                // Horizontal grid lines (parallel to X-axis)
                for (float y = KartesiusCenterY + GridSpacing; y < ScreenHeight - MarginBottom; y += GridSpacing)
                {
                    res.AddRange(DrawingUtils.LineDDA(MarginLeft, y, ScreenWidth - MarginRight, y));
                }

                for (float y = KartesiusCenterY - GridSpacing; y > MarginTop; y -= GridSpacing)
                {
                    res.AddRange(DrawingUtils.LineDDA(MarginLeft, y, ScreenWidth - MarginRight, y));
                }
            }

            return res;
        }

        // Draw quadrant labels
        private void DrawQuadrantLabels()
        {
            // Custom color for labels
            Color labelColor = new Color("#32CD30");

            // Draw in Quadrant I (top-right)
            parent.DrawString(ThemeDB.FallbackFont, new Vector2(KartesiusCenterX + 15, KartesiusCenterY - 15),
                "I", HorizontalAlignment.Left, -1, 16, labelColor);

            // Draw in Quadrant II (top-left)
            parent.DrawString(ThemeDB.FallbackFont, new Vector2(KartesiusCenterX - 30, KartesiusCenterY - 15),
                "II", HorizontalAlignment.Left, -1, 16, labelColor);

            // Draw in Quadrant III (bottom-left)
            parent.DrawString(ThemeDB.FallbackFont, new Vector2(KartesiusCenterX - 30, KartesiusCenterY + 30),
                "III", HorizontalAlignment.Left, -1, 16, labelColor);

            // Draw in Quadrant IV (bottom-right)
            parent.DrawString(ThemeDB.FallbackFont, new Vector2(KartesiusCenterX + 15, KartesiusCenterY + 30),
                "IV", HorizontalAlignment.Left, -1, 16, labelColor);

            // Draw axis labels
            parent.DrawString(ThemeDB.FallbackFont, new Vector2(ScreenWidth - MarginRight - 20, KartesiusCenterY + 20),
                "X", HorizontalAlignment.Left, -1, 16, labelColor);
            parent.DrawString(ThemeDB.FallbackFont, new Vector2(KartesiusCenterX + 10, MarginTop),
                "Y", HorizontalAlignment.Left, -1, 16, labelColor);

            // Draw origin label
            parent.DrawString(ThemeDB.FallbackFont, new Vector2(KartesiusCenterX - 20, KartesiusCenterY + 20),
                "O", HorizontalAlignment.Left, -1, 16, labelColor);

            // Draw coordinate numbers along the axes (if grid is enabled)
            if (ShowGrid)
            {
                for (int i = 1; i * GridSpacing < ScreenWidth - MarginRight - KartesiusCenterX; i++)
                {
                    parent.DrawString(ThemeDB.FallbackFont, new Vector2(KartesiusCenterX + i * GridSpacing, KartesiusCenterY + 15),
                        i.ToString(), HorizontalAlignment.Left, -1, 12, labelColor);
                }

                for (int i = 1; i * GridSpacing < KartesiusCenterX - MarginLeft; i++)
                {
                    parent.DrawString(ThemeDB.FallbackFont, new Vector2(KartesiusCenterX - i * GridSpacing, KartesiusCenterY + 15),
                        (-i).ToString(), HorizontalAlignment.Left, -1, 12, labelColor);
                }

                for (int i = 1; i * GridSpacing < KartesiusCenterY - MarginTop; i++)
                {
                    parent.DrawString(ThemeDB.FallbackFont, new Vector2(KartesiusCenterX + 5, KartesiusCenterY - i * GridSpacing),
                        i.ToString(), HorizontalAlignment.Left, -1, 12, labelColor);
                }

                for (int i = 1; i * GridSpacing < ScreenHeight - MarginBottom - KartesiusCenterY; i++)
                {
                    parent.DrawString(ThemeDB.FallbackFont, new Vector2(KartesiusCenterX + 5, KartesiusCenterY + i * GridSpacing),
                        (-i).ToString(), HorizontalAlignment.Left, -1, 12, labelColor);
                }
            }
        }

        // Calculate the center of each quadrant
        public (Vector2, Vector2, Vector2, Vector2) CalculateQuadrantCenters()
        {
            // Calculate the usable area for each quadrant (respecting margins)
            float quadrantWidth = (ScreenWidth - MarginLeft - MarginRight) / 2;
            float quadrantHeight = (ScreenHeight - MarginTop - MarginBottom) / 2;

            // Calculate the center point of each quadrant in pixel coordinates
            Vector2 q1Center = new Vector2(
                KartesiusCenterX + quadrantWidth / 2,
                KartesiusCenterY - quadrantHeight / 2
            );

            Vector2 q2Center = new Vector2(
                KartesiusCenterX - quadrantWidth / 2,
                KartesiusCenterY - quadrantHeight / 2
            );

            Vector2 q3Center = new Vector2(
                KartesiusCenterX - quadrantWidth / 2,
                KartesiusCenterY + quadrantHeight / 2
            );

            Vector2 q4Center = new Vector2(
                KartesiusCenterX + quadrantWidth / 2,
                KartesiusCenterY + quadrantHeight / 2
            );

            return (q1Center, q2Center, q3Center, q4Center);
        }

        // Draw vertical line in Cartesian coordinates
        public void DrawVerticalLine(float cartX, float yTop, float yBottom)
        {
            // Convert Cartesian coordinates to pixel coordinates
            Vector2 topPoint = ConvertToPixel(cartX, yTop);
            Vector2 bottomPoint = ConvertToPixel(cartX, yBottom);

            // Draw the vertical line
            List<Vector2> linePoints = DrawingUtils.LineDDA(
                topPoint.X, topPoint.Y,
                bottomPoint.X, bottomPoint.Y
            );

            // Draw the line with slightly thicker appearance
            DrawingUtils.PutPixelAll(parent, linePoints, AxisColor);

            // Add a second line right next to the first for thicker appearance
            List<Vector2> thickerLinePoints = DrawingUtils.LineDDA(
                topPoint.X + 1, topPoint.Y,
                bottomPoint.X + 1, bottomPoint.Y
            );
            DrawingUtils.PutPixelAll(parent, thickerLinePoints, AxisColor);
        }

        // Draw horizontal line in Cartesian coordinates
        public void DrawHorizontalLine(float cartY, float xLeft, float xRight, float? length = null)
        {
            // If length is specified, adjust the xRight coordinate
            if (length.HasValue)
            {
                // Calculate the new xRight based on specified length
                float centerX = (xLeft + xRight) / 2;
                float halfLength = length.Value / 2;

                // Adjust left and right points based on the center and half length
                xLeft = centerX - halfLength;
                xRight = centerX + halfLength;
            }

            // Convert Cartesian coordinates to pixel coordinates
            Vector2 leftPoint = ConvertToPixel(xLeft, cartY);
            Vector2 rightPoint = ConvertToPixel(xRight, cartY);

            // Draw the horizontal line
            List<Vector2> linePoints = DrawingUtils.LineDDA(
                leftPoint.X, leftPoint.Y,
                rightPoint.X, rightPoint.Y
            );

            // Draw the line with slightly thicker appearance
            DrawingUtils.PutPixelAll(parent, linePoints, AxisColor);

            // Add a second line right below the first for thicker appearance
            List<Vector2> thickerLinePoints = DrawingUtils.LineDDA(
                leftPoint.X, leftPoint.Y + 1,
                rightPoint.X, rightPoint.Y + 1
            );
            DrawingUtils.PutPixelAll(parent, thickerLinePoints, AxisColor);
        }

        // Draw trapezoid boundary lines
        public void DrawTrapezoidBoundaryLines(float cartX, float yTop, float yBottom, float spacing = 40)
        {
            // Draw the first line right next to trapezoids
            DrawVerticalLine(cartX, yTop, yBottom);

            // Draw the second line with spacing
            DrawVerticalLine(cartX + spacing, yTop, yBottom);
        }
    }
}
