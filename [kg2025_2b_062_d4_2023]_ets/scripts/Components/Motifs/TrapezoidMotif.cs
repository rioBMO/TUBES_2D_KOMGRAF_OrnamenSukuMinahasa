using Godot;
using System;
using System.Collections.Generic;

namespace KG2025.Components.Motifs
{
    public class TrapezoidMotif : MotifBase
    {
        public TrapezoidMotif(Node2D parent, KartesiusSystem kartesiusSystem) : base(parent, kartesiusSystem) { }

        public override void Draw(float x, float y, float size)
        {
            // This method uses pixel coordinates directly
            DrawTrapezoidPatternAt(x, y, size);
        }

        public void DrawInCartesianWithOrientation(float cartX, float cartY, float size, float rotationOffset = 0f, bool mirrorShape = false)
        {
            // Convert Cartesian coordinates to pixel coordinates and draw
            Vector2 pixelPosition = kartesiusSystem.ConvertToPixel(cartX, cartY);
            DrawTrapezoidPatternAt(pixelPosition.X, pixelPosition.Y, size, rotationOffset, mirrorShape);
        }

        private void DrawTrapezoidPatternAt(float x, float y, float size, float rotationOffset = 0f, bool mirrorShape = false)
        {
            // Buat 6 titik hexagon berputar dengan pusat di (x, y)
            Vector2[] hexPoints = new Vector2[6];
            for (int i = 0; i < 6; i++)
            {
                float angle = i * Mathf.Pi / 3 + rotationOffset;
                hexPoints[i] = new Vector2(
                    x + size * Mathf.Cos(angle),
                    y + size * Mathf.Sin(angle)
                );
            }

            // Ambil 4 titik untuk trapezoid
            Vector2[] trapezoid = new Vector2[5];

            if (!mirrorShape)
            {
                // Original orientation (for left side)
                trapezoid[0] = hexPoints[2];
                trapezoid[1] = hexPoints[3];
                trapezoid[2] = hexPoints[4];
                trapezoid[3] = hexPoints[5];
            }
            else
            {
                // Mirrored orientation (for right side)
                trapezoid[0] = hexPoints[5];
                trapezoid[1] = hexPoints[0];
                trapezoid[2] = hexPoints[1];
                trapezoid[3] = hexPoints[2];
            }

            trapezoid[4] = trapezoid[0]; // Tutup bentuk

            // Gambar outline trapezoid utama
            DrawPolygon(trapezoid);

            // Skala untuk lapisan dalam
            float[] scales = { 0.8f, 0.6f, 0.3f };

            // Gambar layer trapezoid yang lebih kecil
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
    }
}
