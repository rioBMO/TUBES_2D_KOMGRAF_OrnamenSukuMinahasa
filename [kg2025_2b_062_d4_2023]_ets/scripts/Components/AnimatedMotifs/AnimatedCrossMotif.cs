using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;
using KG2025.Components;

// Use GodotVector2 alias to avoid ambiguity with System.Numerics.Vector2
using GodotVector2 = Godot.Vector2;

namespace KG2025.Components.AnimatedMotifs
{
	public class AnimatedCrossMotif : AnimatedMotifBase
	{
		private GodotVector2 centerPosition;
		private float patternSize;
		private float orbitRadius;
		private int numPatterns;
		private List<GodotVector2> orbitPositions = new List<GodotVector2>();

		public AnimatedCrossMotif(Node2D parent, KartesiusSystem kartesiusSystem, GodotVector2 centerPos,
								float patternSize, float orbitRadius, int numPatterns = 6)
			: base(parent, kartesiusSystem)
		{
			this.centerPosition = centerPos;
			this.patternSize = patternSize;
			this.orbitRadius = orbitRadius;
			this.numPatterns = numPatterns;

			// Initialize orbit positions
			SetupOrbitPositions();
		}

		private void SetupOrbitPositions()
		{
			orbitPositions.Clear();
			for (int i = 0; i < numPatterns; i++)
			{
				float angle = i * (2 * Mathf.Pi / numPatterns);
				orbitPositions.Add(new GodotVector2(
					centerPosition.X + orbitRadius * Mathf.Cos(angle),
					centerPosition.Y + orbitRadius * Mathf.Sin(angle)
				));
			}
		}

		public override void Draw()
		{
			// Draw center pattern with EXACT same scale as original Karya2
			DrawCrossPattern(centerPosition.X, centerPosition.Y, patternSize * 1.1f);

			// Draw orbiting patterns
			DrawOrbitingCrossPatterns();
		}

		private void DrawOrbitingCrossPatterns()
		{
			// For each position, calculate its new orbit position
			for (int i = 0; i < numPatterns; i++)
			{
				float baseAngle = (i * (2 * Mathf.Pi / numPatterns));
				float currentAngle = baseAngle + orbitAngle;

				// Calculate new position on the orbit - EXACT same calculation as original
				float x = centerPosition.X + orbitRadius * Mathf.Cos(currentAngle);
				float y = centerPosition.Y + orbitRadius * Mathf.Sin(currentAngle);

				// Draw cross pattern at its orbit position with exact same scale as original
				DrawCrossPattern(x, y, patternSize * 0.8f);
			}
		}

		private void DrawCrossPattern(float x, float y, float size)
		{
			// Draw hexagon
			DrawHexagon(x, y, size);

			// Draw cross (plus sign) - EXACT same proportions as original
			DrawCrossShapeAt(x, y, size * 0.5f, size);

			// Draw inner circle - EXACT same proportion as original
			DrawCircle(x, y, size * 0.15f);
		}

		private void DrawHexagon(float x, float y, float size)
		{
			// Create hexagon points
			GodotVector2[] hexPoints = new GodotVector2[7]; // Extra point to close the shape
			for (int i = 0; i < 6; i++)
			{
				float angle = i * Mathf.Pi / 3; // 60 degrees in radians
				hexPoints[i] = new GodotVector2(
					x + size * Mathf.Cos(angle),
					y + size * Mathf.Sin(angle)
				);
			}
			hexPoints[6] = hexPoints[0]; // Close the shape

			// Draw the hexagon outline
			DrawPolygon(hexPoints);
		}

		private void DrawCrossShapeAt(float x, float y, float width, float height)
		{
			// Define the cross/plus shape dimensions
			float halfWidth = width / 2;
			float halfHeight = height / 2;

			// Horizontal rectangle of the cross
			GodotVector2[] horizontalRect = new GodotVector2[5];
			horizontalRect[0] = new GodotVector2(x - halfHeight, y - halfWidth);
			horizontalRect[1] = new GodotVector2(x + halfHeight, y - halfWidth);
			horizontalRect[2] = new GodotVector2(x + halfHeight, y + halfWidth);
			horizontalRect[3] = new GodotVector2(x - halfHeight, y + halfWidth);
			horizontalRect[4] = new GodotVector2(x - halfHeight, y - halfWidth); // Close the shape

			// Vertical rectangle of the cross
			GodotVector2[] verticalRect = new GodotVector2[5];
			verticalRect[0] = new GodotVector2(x - halfWidth, y - halfHeight);
			verticalRect[1] = new GodotVector2(x + halfWidth, y - halfHeight);
			verticalRect[2] = new GodotVector2(x + halfWidth, y + halfHeight);
			verticalRect[3] = new GodotVector2(x - halfWidth, y + halfHeight);
			verticalRect[4] = new GodotVector2(x - halfWidth, y - halfHeight); // Close the shape

			// Draw both rectangles
			DrawPolygon(horizontalRect);
			DrawPolygon(verticalRect);
		}
	}
}
