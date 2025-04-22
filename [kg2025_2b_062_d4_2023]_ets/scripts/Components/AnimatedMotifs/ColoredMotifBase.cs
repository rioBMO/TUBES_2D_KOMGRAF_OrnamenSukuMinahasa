using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;
using KG2025.Components;

// Use GodotVector2 alias to avoid ambiguity with System.Numerics.Vector2
using GodotVector2 = Godot.Vector2;

namespace KG2025.Components.AnimatedMotifs
{
    public abstract class ColoredMotifBase : AnimatedMotifBase
    {
        // Change to accept both types of color management
        protected ColorThemeManager colorPalette;

        public ColoredMotifBase(Node2D parent, KartesiusSystem kartesiusSystem)
            : base(parent, kartesiusSystem)
        {
            this.colorPalette = new ColorThemeManager();
        }

        // Add method to accept ColorPalette
        public void SetColorPalette(ColorPalette palette)
        {
            this.colorPalette = ColorPaletteAdapter.ConvertToThemeManager(palette);
        }

        // Keep original method for ColorThemeManager
        public void SetColorPalette(ColorThemeManager palette)
        {
            this.colorPalette = palette;
        }

        // Additional drawing methods for filled shapes
        protected void DrawFilledPolygon(GodotVector2[] points, Color fillColor)
        {
            parent.DrawPolygon(points, new Color[] { fillColor });
        }

        protected void DrawFilledCircle(float x, float y, float radius, Color fillColor)
        {
            parent.DrawCircle(new GodotVector2(x, y), radius, fillColor);
        }

        protected void DrawRect(float x1, float y1, float width, float height, Color fillColor, bool filled = true)
        {
            parent.DrawRect(new Rect2(x1, y1, width, height), fillColor, filled);
        }

        protected void DrawBackgroundRect(float x1, float y1, float x2, float y2, Color fillColor)
        {
            // Ensure coordinates are properly ordered
            if (x1 > x2)
            {
                float temp = x1;
                x1 = x2;
                x2 = temp;
            }

            if (y1 > y2)
            {
                float temp = y1;
                y1 = y2;
                y2 = temp;
            }

            // Calculate rectangle dimensions
            float width = x2 - x1;
            float height = y2 - y1;
            parent.DrawRect(new Rect2(x1, y1, width, height), fillColor, true);
        }

        protected void DrawBorderRect(float x1, float topY, float x2, float bottomY, Color fillColor)
        {
            // Ensure x1 is smaller than x2
            if (x1 > x2)
            {
                float temp = x1;
                x1 = x2;
                x2 = temp;
            }

            // Calculate rectangle dimensions
            float width = x2 - x1;
            Rect2 rect = new Rect2(x1, topY, width, bottomY - topY);

            // Draw filled rectangle
            parent.DrawRect(rect, fillColor, true);

            // Draw outline for clarity
            parent.DrawRect(rect, lineColor, false);
        }

        protected void DrawHorizontalBorderRect(float leftX, float y1, float rightX, float y2, Color fillColor)
        {
            // Ensure y1 is smaller than y2
            if (y1 > y2)
            {
                float temp = y1;
                y1 = y2;
                y2 = temp;
            }

            // Calculate rectangle dimensions
            float height = y2 - y1;
            Rect2 rect = new Rect2(leftX, y1, rightX - leftX, height);

            // Draw filled rectangle
            parent.DrawRect(rect, fillColor, true);

            // Draw outline for clarity
            parent.DrawRect(rect, lineColor, false);
        }

        protected void DrawFilledHexagon(float x, float y, float size, Color fillColor, Color outlineColor)
        {
            // Create hexagon points
            GodotVector2[] hexPoints = new GodotVector2[6];
            for (int i = 0; i < 6; i++)
            {
                float angle = i * Mathf.Pi / 3; // 60 degrees in radians
                hexPoints[i] = new GodotVector2(
                    x + size * Mathf.Cos(angle),
                    y + size * Mathf.Sin(angle)
                );
            }

            // Draw filled hexagon
            parent.DrawPolygon(hexPoints, new Color[] { fillColor });

            // Draw outline
            GodotVector2[] outline = new GodotVector2[7];
            for (int i = 0; i < 6; i++)
            {
                outline[i] = hexPoints[i];
            }
            outline[6] = outline[0]; // Close the shape

            parent.DrawPolyline(outline, outlineColor, 4.0f);
        }

        // Add this new method overload for DrawCircle that supports the filled parameter
        protected void DrawCircle(GodotVector2 center, float radius, Color color, bool filled)
        {
            if (filled)
            {
                // For filled circle, use Godot's built-in DrawCircle method
                parent.DrawCircle(center, radius, color);
            }
            else
            {
                // For outline only, draw a circle using an arc
                parent.DrawArc(center, radius, 0, Mathf.Pi * 2, 32, color);
            }
        }

        // Add these methods for external control of animation parameters
        public virtual void SetOrbitAngle(float angle)
        {
            this.orbitAngle = angle;
        }

        public virtual void SetBreathingFactor(float factor)
        {
            this.breathingFactor = factor;
        }
    }
}
