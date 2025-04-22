using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;
using KG2025.Components;

// Use GodotVector2 alias to avoid ambiguity with System.Numerics.Vector2
using GodotVector2 = Godot.Vector2;

namespace KG2025.Components.AnimatedMotifs
{
    public class AnimatedDiamondMotif : AnimatedMotifBase
    {
        private GodotVector2 position;
        private float size;
        private float diamondBreathingFactor = 1.0f;

        public AnimatedDiamondMotif(Node2D parent, KartesiusSystem kartesiusSystem,
                                   GodotVector2 position, float size)
            : base(parent, kartesiusSystem)
        {
            this.position = position;
            this.size = size;
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            // Calculate diamond-specific breathing factor with boundary constraints
            // Exactly as in the original Karya2 code
            float verticalLineOffset = 110;
            float boundaryWidth = verticalLineOffset - 10; // Buffer from boundary
            float maxAllowedScale = boundaryWidth / size;
            float constrainedMaxScale = Mathf.Min(maxScale, maxAllowedScale);

            // Calculate breathing factor using sine wave
            diamondBreathingFactor = minScale + ((Mathf.Sin(breathingTime) + 1) / 2) * (constrainedMaxScale - minScale);
        }

        public override void Draw()
        {
            float scaledSize = size * diamondBreathingFactor;

            // Outer diamond
            GodotVector2[] outerDiamond = new GodotVector2[5];
            outerDiamond[0] = new GodotVector2(position.X, position.Y - scaledSize);        // Top
            outerDiamond[1] = new GodotVector2(position.X + scaledSize, position.Y);        // Right
            outerDiamond[2] = new GodotVector2(position.X, position.Y + scaledSize);        // Bottom
            outerDiamond[3] = new GodotVector2(position.X - scaledSize, position.Y);        // Left
            outerDiamond[4] = new GodotVector2(position.X, position.Y - scaledSize);        // Back to top

            // Middle diamond (about 66% of the outer size)
            float middleSize = scaledSize * 0.66f;
            GodotVector2[] middleDiamond = new GodotVector2[5];
            middleDiamond[0] = new GodotVector2(position.X, position.Y - middleSize);
            middleDiamond[1] = new GodotVector2(position.X + middleSize, position.Y);
            middleDiamond[2] = new GodotVector2(position.X, position.Y + middleSize);
            middleDiamond[3] = new GodotVector2(position.X - middleSize, position.Y);
            middleDiamond[4] = new GodotVector2(position.X, position.Y - middleSize);

            // Inner diamond (about 33% of the outer size)
            float innerSize = scaledSize * 0.33f;
            GodotVector2[] innerDiamond = new GodotVector2[5];
            innerDiamond[0] = new GodotVector2(position.X, position.Y - innerSize);
            innerDiamond[1] = new GodotVector2(position.X + innerSize, position.Y);
            innerDiamond[2] = new GodotVector2(position.X, position.Y + innerSize);
            innerDiamond[3] = new GodotVector2(position.X - innerSize, position.Y);
            innerDiamond[4] = new GodotVector2(position.X, position.Y - innerSize);

            // Draw all three diamond outlines
            DrawPolygon(outerDiamond);
            DrawPolygon(middleDiamond);
            DrawPolygon(innerDiamond);
        }
    }
}
