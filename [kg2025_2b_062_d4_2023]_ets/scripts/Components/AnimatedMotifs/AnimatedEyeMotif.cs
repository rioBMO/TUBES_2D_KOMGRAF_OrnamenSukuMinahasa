using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;
using KG2025.Components;

// Use GodotVector2 alias to avoid ambiguity with System.Numerics.Vector2
using GodotVector2 = Godot.Vector2;

namespace KG2025.Components.AnimatedMotifs
{
	public class AnimatedEyeMotif : AnimatedMotifBase
	{
		private GodotVector2 centerPosition;
		private float patternSize;
		private float orbitRadius;
		private int numPatterns;
		private List<GodotVector2> orbitPositions = new List<GodotVector2>();

		public AnimatedEyeMotif(Node2D parent, KartesiusSystem kartesiusSystem, GodotVector2 centerPos,
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
			DrawEyePattern(centerPosition.X, centerPosition.Y, patternSize * 1.1f);

			// Draw orbiting patterns
			DrawOrbitingEyePatterns();
		}

		private void DrawOrbitingEyePatterns()
		{
			// For each position, calculate its new orbit position
			for (int i = 0; i < numPatterns; i++)
			{
				float baseAngle = (i * (2 * Mathf.Pi / numPatterns));
				float currentAngle = baseAngle + orbitAngle;

				// Calculate new position on the orbit - EXACT same calculation as original
				float x = centerPosition.X + orbitRadius * Mathf.Cos(currentAngle);
				float y = centerPosition.Y + orbitRadius * Mathf.Sin(currentAngle);

				// Draw eye pattern at its orbit position - EXACT same scale as original
				DrawEyePattern(x, y, patternSize * 0.8f);
			}
		}

		private void DrawEyePattern(float x, float y, float size)
		{
			// Draw hexagon
			DrawHexagon(x, y, size);

			// Draw outer circle - EXACT same proportion (0.7f) as original
			DrawCircle(x, y, size * 0.7f);

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
	}
}
