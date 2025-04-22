using Godot;
using System;
using System.Collections.Generic;

namespace KG2025.Components.Motifs
{
    public class DiamondMotif : MotifBase
    {
        public DiamondMotif(Node2D parent, KartesiusSystem kartesiusSystem) : base(parent, kartesiusSystem) { }

        public override void Draw(float x, float y, float size)
        {
            DrawDiamondPatternAt(x, y, size);
        }

        private void DrawDiamondPatternAt(float x, float y, float size)
        {
            // Draw three concentric diamonds (rotated squares)

            // Outer diamond
            Vector2[] outerDiamond = new Vector2[5];
            outerDiamond[0] = new Vector2(x, y - size);        // Top
            outerDiamond[1] = new Vector2(x + size, y);        // Right
            outerDiamond[2] = new Vector2(x, y + size);        // Bottom
            outerDiamond[3] = new Vector2(x - size, y);        // Left
            outerDiamond[4] = new Vector2(x, y - size);        // Back to top to close the shape

            // Middle diamond (about 66% of the outer size)
            float middleSize = size * 0.66f;
            Vector2[] middleDiamond = new Vector2[5];
            middleDiamond[0] = new Vector2(x, y - middleSize); // Top
            middleDiamond[1] = new Vector2(x + middleSize, y); // Right
            middleDiamond[2] = new Vector2(x, y + middleSize); // Bottom
            middleDiamond[3] = new Vector2(x - middleSize, y); // Left
            middleDiamond[4] = new Vector2(x, y - middleSize); // Back to top to close the shape

            // Inner diamond (about 33% of the outer size)
            float innerSize = size * 0.33f;
            Vector2[] innerDiamond = new Vector2[5];
            innerDiamond[0] = new Vector2(x, y - innerSize);   // Top
            innerDiamond[1] = new Vector2(x + innerSize, y);   // Right
            innerDiamond[2] = new Vector2(x, y + innerSize);   // Bottom
            innerDiamond[3] = new Vector2(x - innerSize, y);   // Left
            innerDiamond[4] = new Vector2(x, y - innerSize);   // Back to top to close the shape

            // Draw all three diamond outlines
            DrawPolygon(outerDiamond);
            DrawPolygon(middleDiamond);
            DrawPolygon(innerDiamond);
        }
    }
}
