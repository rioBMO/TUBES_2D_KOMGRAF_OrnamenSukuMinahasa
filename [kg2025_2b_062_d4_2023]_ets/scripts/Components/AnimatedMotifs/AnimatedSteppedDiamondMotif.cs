using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;
using KG2025.Components;

namespace KG2025.Components.AnimatedMotifs
{
    public class AnimatedSteppedDiamondMotif : AnimatedMotifBase
    {
        private Vector2 position;
        private float squareSize;
        
        // Separate breathing factor
        private float steppedDiamondBreathingFactor = 1.0f;
        private bool inverseBreathe = false;
        
        public AnimatedSteppedDiamondMotif(Node2D parent, KartesiusSystem kartesiusSystem, Vector2 position, float squareSize, bool inverseBreathe = false) 
            : base(parent, kartesiusSystem)
        {
            this.position = position;
            this.squareSize = squareSize;
            this.inverseBreathe = inverseBreathe;
        }
        
        public override void Update(float delta)
        {
            base.Update(delta);
            
            // Calculate stepped diamond-specific breathing factor
            if (inverseBreathe)
            {
                // 180 degrees out of phase
                steppedDiamondBreathingFactor = minScale + ((Mathf.Sin(breathingTime + Mathf.Pi) + 1) / 2) * (maxScale - minScale);
            }
            else
            {
                steppedDiamondBreathingFactor = breathingFactor;
            }
        }
        
        public override void Draw()
        {
            float scaledSize = squareSize * steppedDiamondBreathingFactor;
            
            // Top square
            DrawSquare(position.X, position.Y - 4 * scaledSize, scaledSize);

            // Top-right squares
            DrawSquare(position.X + scaledSize, position.Y - 3 * scaledSize, scaledSize);
            DrawSquare(position.X + 2 * scaledSize, position.Y - 2 * scaledSize, scaledSize);
            DrawSquare(position.X + 3 * scaledSize, position.Y - scaledSize, scaledSize);

            // Right square
            DrawSquare(position.X + 4 * scaledSize, position.Y, scaledSize);

            // Bottom-right squares
            DrawSquare(position.X + 3 * scaledSize, position.Y + scaledSize, scaledSize);
            DrawSquare(position.X + 2 * scaledSize, position.Y + 2 * scaledSize, scaledSize);
            DrawSquare(position.X + scaledSize, position.Y + 3 * scaledSize, scaledSize);

            // Bottom square
            DrawSquare(position.X, position.Y + 4 * scaledSize, scaledSize);

            // Bottom-left squares
            DrawSquare(position.X - scaledSize, position.Y + 3 * scaledSize, scaledSize);
            DrawSquare(position.X - 2 * scaledSize, position.Y + 2 * scaledSize, scaledSize);
            DrawSquare(position.X - 3 * scaledSize, position.Y + scaledSize, scaledSize);

            // Left square
            DrawSquare(position.X - 4 * scaledSize, position.Y, scaledSize);

            // Top-left squares
            DrawSquare(position.X - 3 * scaledSize, position.Y - scaledSize, scaledSize);
            DrawSquare(position.X - 2 * scaledSize, position.Y - 2 * scaledSize, scaledSize);
            DrawSquare(position.X - scaledSize, position.Y - 3 * scaledSize, scaledSize);
        }
        
        private void DrawSquare(float x, float y, float size)
        {
            // Draw a square with top-left corner at (x,y) and given size
            Vector2[] square = new Vector2[5];
            square[0] = new Vector2(x, y);
            square[1] = new Vector2(x + size, y);
            square[2] = new Vector2(x + size, y + size);
            square[3] = new Vector2(x, y + size);
            square[4] = new Vector2(x, y);

            // Draw the square outline
            DrawPolygon(square);
        }
    }
}
