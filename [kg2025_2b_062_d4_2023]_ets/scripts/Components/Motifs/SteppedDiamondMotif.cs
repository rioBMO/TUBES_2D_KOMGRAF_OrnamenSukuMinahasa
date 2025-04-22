using Godot;
using System;
using System.Collections.Generic;

namespace KG2025.Components.Motifs
{
    public class SteppedDiamondMotif : MotifBase
    {
        public SteppedDiamondMotif(Node2D parent, KartesiusSystem kartesiusSystem) : base(parent, kartesiusSystem) { }

        public override void Draw(float x, float y, float size)
        {
            DrawSteppedDiamondPatternAt(x, y, size);
        }

        private void DrawSteppedDiamondPatternAt(float x, float y, float squareSize)
        {
            // Draw a diamond-shaped outline made of squares

            // Top square
            DrawSquare(x, y - 4 * squareSize, squareSize);

            // Top-right squares
            DrawSquare(x + squareSize, y - 3 * squareSize, squareSize);
            DrawSquare(x + 2 * squareSize, y - 2 * squareSize, squareSize);
            DrawSquare(x + 3 * squareSize, y - squareSize, squareSize);

            // Right square
            DrawSquare(x + 4 * squareSize, y, squareSize);

            // Bottom-right squares
            DrawSquare(x + 3 * squareSize, y + squareSize, squareSize);
            DrawSquare(x + 2 * squareSize, y + 2 * squareSize, squareSize);
            DrawSquare(x + squareSize, y + 3 * squareSize, squareSize);

            // Bottom square
            DrawSquare(x, y + 4 * squareSize, squareSize);

            // Bottom-left squares
            DrawSquare(x - squareSize, y + 3 * squareSize, squareSize);
            DrawSquare(x - 2 * squareSize, y + 2 * squareSize, squareSize);
            DrawSquare(x - 3 * squareSize, y + squareSize, squareSize);

            // Left square
            DrawSquare(x - 4 * squareSize, y, squareSize);

            // Top-left squares
            DrawSquare(x - 3 * squareSize, y - squareSize, squareSize);
            DrawSquare(x - 2 * squareSize, y - 2 * squareSize, squareSize);
            DrawSquare(x - squareSize, y - 3 * squareSize, squareSize);
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
