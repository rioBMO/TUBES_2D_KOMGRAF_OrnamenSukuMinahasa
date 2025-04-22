using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;
using KG2025.Components;

namespace KG2025.Components.Motifs
{
    public abstract class MotifBase
    {
        protected Node2D parent;
        protected KartesiusSystem kartesiusSystem;
        protected Color lineColor = Colors.White;
        protected Color fillColor = Colors.Transparent;

        public MotifBase(Node2D parent, KartesiusSystem kartesiusSystem)
        {
            this.parent = parent;
            this.kartesiusSystem = kartesiusSystem;
        }

        // Set colors for the motif
        public void SetColors(Color lineColor, Color fillColor = default)
        {
            this.lineColor = lineColor;
            this.fillColor = fillColor != default ? fillColor : Colors.Transparent;
        }

        // Draw the motif at the specified position
        public abstract void Draw(float x, float y, float size);

        // Draw the motif at a position specified in Cartesian coordinates
        public void DrawInCartesian(float cartesianX, float cartesianY, float size)
        {
            Vector2 pixelPos = kartesiusSystem.ConvertToPixel(cartesianX, cartesianY);
            Draw(pixelPos.X, pixelPos.Y, size);
        }

        // Helper methods that might be useful for derived classes
        protected void DrawCircle(float x, float y, float radius, Color? color = null)
        {
            List<Vector2> points = DrawingUtils.Lingkaran(x, y, radius);
            DrawingUtils.PutPixelAll(parent, points, color ?? lineColor);
        }

        protected void DrawLine(float x1, float y1, float x2, float y2, Color? color = null)
        {
            List<Vector2> points = DrawingUtils.LineDDA(x1, y1, x2, y2);
            DrawingUtils.PutPixelAll(parent, points, color ?? lineColor);
        }

        protected void DrawPolygon(Vector2[] points, Color? color = null)
        {
            DrawingUtils.DrawPolygonOutline(parent, points, color ?? lineColor);
        }
    }
}
