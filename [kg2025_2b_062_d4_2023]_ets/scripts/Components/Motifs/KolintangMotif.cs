using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;

namespace KG2025.Components.Motifs
{
	public class KolintangMotif
	{
		private Node2D parent;
		private KartesiusSystem kartesiusSystem;
		private Color outlineColor = Colors.White;
		private Color fillColor = new Color("#513822"); // Brown wooden color for kolintang base
		private Color barColor = Colors.White; // Color for wooden bars

		// Mallet properties - EXACT match with AnimatedKolintangMotif
		private float malletHeadRadius = 68f / 2f; // Exact same value as in AnimatedKolintangMotif 
		private float malletHandleWidth = 7f;      // Exact same value as in AnimatedKolintangMotif
		private float malletHandleHeight = 120f;   // Exact same value as in AnimatedKolintangMotif
		private float malletOffset = 100f;        // Increased to ensure visibility
		private bool showMallets = false;

		public KolintangMotif(Node2D parent, KartesiusSystem kartesiusSystem)
		{
			this.parent = parent;
			this.kartesiusSystem = kartesiusSystem;
		}

		public void SetColors(Color outlineColor, Color fillColor)
		{
			this.outlineColor = outlineColor;
			this.fillColor = fillColor;
		}

		// Set color for wooden bars
		public void SetBarColor(Color barColor)
		{
			this.barColor = barColor;
		}

		// Draw a horizontal trapezoid (rotated 90 degrees) with wooden bars
		public void Draw(float centerX, float centerY, float leftWidth, float rightWidth, float length)
		{
			// Calculate trapezoid points for horizontal orientation (90 degrees rotated)
			Vector2[] points = new Vector2[4];

			// Left side of trapezoid (would be "top" in vertical orientation)
			float leftTopY = centerY - leftWidth / 2;
			float leftBottomY = centerY + leftWidth / 2;
			float leftX = centerX - length / 2;

			// Right side of trapezoid (would be "bottom" in vertical orientation)
			float rightTopY = centerY - rightWidth / 2;
			float rightBottomY = centerY + rightWidth / 2;
			float rightX = centerX + length / 2;

			// Define trapezoid points in clockwise order
			points[0] = new Vector2(leftX, leftTopY);      // Left top
			points[1] = new Vector2(rightX, rightTopY);    // Right top
			points[2] = new Vector2(rightX, rightBottomY); // Right bottom
			points[3] = new Vector2(leftX, leftBottomY);   // Left bottom

			// Use DrawingUtils to draw outline instead of built-in functions
			DrawingUtils.DrawPolygonOutline(parent, points, outlineColor);

			// Draw the wooden bars on top of the kolintang base
			DrawWoodenBars(leftX, leftWidth, rightX, rightWidth, centerY);

			// Draw the mallets if they should be shown
			if (showMallets)
			{
				// NEW PLACEMENT: Position mallets to the left and right of the kolintang
				// Left mallet - place it at the left edge of the kolintang
				float leftMalletX = leftX - 100; // 100 pixels to the left of the kolintang
												 // Right mallet - place it at the right edge of the kolintang
				float rightMalletX = rightX + 100; // 100 pixels to the right of the kolintang

				// Place both mallets at the vertical center
				DrawMallet(leftMalletX, centerY);
				DrawMallet(rightMalletX, centerY);

				// Debug output
				GD.Print($"Drawing left mallet at: ({leftMalletX}, {centerY})");
				GD.Print($"Drawing right mallet at: ({rightMalletX}, {centerY})");
			}
		}

		// Enable or disable drawing of mallets
		public void SetMalletsVisibility(bool visible)
		{
			this.showMallets = visible;
		}

		// Draw kolintang mallets (outline only) - match exactly with AnimatedKolintangMotif
		private void DrawMallets(float centerX, float centerY)
		{
			// Calculate positions for left and right mallets
			float spacing = 200f; // Much wider spacing for better visibility
			float leftMalletX = centerX - spacing / 2;
			float rightMalletX = centerX + spacing / 2;

			// Draw the mallets exactly as in AnimatedKolintangMotif
			DrawMallet(leftMalletX, centerY);
			DrawMallet(rightMalletX, centerY);

			// Debug output for debugging - remove in production
			GD.Print($"Drawing mallets at: ({leftMalletX}, {centerY}) and ({rightMalletX}, {centerY})");
		}

		// Draw a single mallet - EXACT MATCH with AnimatedKolintangMotif implementation
		private void DrawMallet(float x, float y)
		{
			// Mallet head parameters
			float headRadius = 68f / 2f;
			float handleWidth = 7f;
			float handleHeight = 120f;

			// Position handle BELOW the head (fix the inverted orientation)
			float handleX = x - handleWidth / 2;
			float handleY = y + headRadius; // Start from bottom of head

			// Draw handle outline
			DrawLine(handleX, handleY, handleX, handleY + handleHeight, outlineColor); // Left vertical
			DrawLine(handleX + handleWidth, handleY, handleX + handleWidth, handleY + handleHeight, outlineColor); // Right vertical
			DrawLine(handleX, handleY + handleHeight, handleX + handleWidth, handleY + handleHeight, outlineColor); // Bottom line

			// Draw head outline (circle)
			DrawCircleOutline(x, y, headRadius, outlineColor);
		}

		// Add the exact DrawCircleOutline function from AnimatedKolintangMotif
		private void DrawCircleOutline(float centerX, float centerY, float radius, Color outlineColor)
		{
			List<Vector2> circlePoints = DrawingUtils.Lingkaran(centerX, centerY, radius);
			DrawingUtils.PutPixelAll(parent, circlePoints, outlineColor);
		}

		// Draw wooden bars on the kolintang
		private void DrawWoodenBars(float leftX, float leftWidth, float rightX, float rightWidth, float centerY)
		{
			// Parameters for wooden bars
			float barWidth = 52f;
			float cornerRadius = 10f;
			float leftMargin = 32f;  // Starting 60px from left edge
			float rightMargin = 60f; // Ending 32px from right edge
			float barExtension = 50f; // How much bars extend beyond base on each side

			// Change to 12 bars as requested
			int numberOfBars = 12;

			// Update bar color to match the reference image (light brown)
			Color woodColor = new Color("#E5A358"); // Light brown for wooden bars
			Color holeColor = new Color("#513822"); // Dark brown for the holes

			// Calculate total available length and space between bars
			float totalLength = rightX - leftX;
			float availableLength = totalLength - leftMargin - rightMargin;

			// Calculate spacing between bars
			float totalBarsWidth = barWidth * numberOfBars;
			float spacing = (availableLength - totalBarsWidth) / (numberOfBars - 1);

			// Starting position for first bar
			float startX = leftX + leftMargin;

			// Draw each wooden bar
			for (int i = 0; i < numberOfBars; i++)
			{
				float barX = startX + (barWidth + spacing) * i;

				// Calculate the height of the bar based on its x position along the trapezoid
				float progress = (barX - leftX) / (rightX - leftX);
				float baseHeight = leftWidth + progress * (rightWidth - leftWidth);

				// Make the bar extend beyond the base on top and bottom
				float barHeight = baseHeight + barExtension * 2;

				// Center the bar vertically
				float barTop = centerY - barHeight / 2;

				// Draw rounded rectangle for the wooden bar with white outline
				DrawRoundedRectangle(barX, barTop, barWidth, barHeight, cornerRadius, woodColor, Colors.White);

				// Calculate where this bar intersects with the base kolintang
				float topIntersection = centerY - baseHeight / 2;
				float bottomIntersection = centerY + baseHeight / 2;

				// Draw holes on each bar exactly 23 pixels INWARD from the base edges
				float holeRadius = 4f;
				float holeOffset = 23f;
				float holeX = barX + barWidth / 2;

				// Top hole - 23 pixels INWARD from where bar meets base top
				float topHoleY = topIntersection + holeOffset; // Changed to inward direction
				DrawCircle(holeX, topHoleY, holeRadius, holeColor, Colors.White);

				// Bottom hole - 23 pixels INWARD from where bar meets base bottom
				float bottomHoleY = bottomIntersection - holeOffset; // Changed to inward direction
				DrawCircle(holeX, bottomHoleY, holeRadius, holeColor, Colors.White);

			}
		}

		// Draw a circle using custom DrawingUtils with separate outline color
		private void DrawCircle(float centerX, float centerY, float radius, Color fillColor, Color outlineColor)
		{
			List<Vector2> circlePoints = DrawingUtils.Lingkaran(centerX, centerY, radius);
			DrawingUtils.PutPixelAll(parent, circlePoints, outlineColor);
		}

		// Keep original method for backward compatibility
		private void DrawCircle(float centerX, float centerY, float radius, Color color)
		{
			DrawCircle(centerX, centerY, radius, color, color);
		}

		// Updated to accept separate outline color
		private void DrawRoundedRectangle(float x, float y, float width, float height, float radius, Color fillColor, Color outlineColor)
		{
			// Calculate corner positions
			float x2 = x + width;
			float y2 = y + height;

			// Adjust corner radius if it's too large for the rectangle
			radius = Math.Min(radius, Math.Min(width / 2, height / 2));

			// Draw straight edges with outline color
			// Top edge (excluding corners)
			DrawLine(x + radius, y, x2 - radius, y, outlineColor);
			// Bottom edge (excluding corners)
			DrawLine(x + radius, y2, x2 - radius, y2, outlineColor);
			// Left edge (excluding corners)
			DrawLine(x, y + radius, x, y2 - radius, outlineColor);
			// Right edge (excluding corners)
			DrawLine(x2, y + radius, x2, y2 - radius, outlineColor);

			// Draw rounded corners using approximated arcs with outline color
			DrawCornerArc(x + radius, y + radius, radius, 180, 270, outlineColor);  // Top-left
			DrawCornerArc(x2 - radius, y + radius, radius, 270, 360, outlineColor); // Top-right
			DrawCornerArc(x2 - radius, y2 - radius, radius, 0, 90, outlineColor);   // Bottom-right
			DrawCornerArc(x + radius, y2 - radius, radius, 90, 180, outlineColor);  // Bottom-left
		}

		// Keep original method for backward compatibility
		private void DrawRoundedRectangle(float x, float y, float width, float height, float radius, Color color)
		{
			DrawRoundedRectangle(x, y, width, height, radius, color, color);
		}

		// Draw a line using custom DrawingUtils
		private void DrawLine(float x1, float y1, float x2, float y2, Color color)
		{
			List<Vector2> linePoints = DrawingUtils.LineBresenham(x1, y1, x2, y2);
			DrawingUtils.PutPixelAll(parent, linePoints, color);
		}

		// Draw an approximated arc using small line segments
		private void DrawCornerArc(float centerX, float centerY, float radius, float startAngle, float endAngle, Color color)
		{
			// Approximate the arc with line segments
			int segments = 8; // Number of segments to approximate the corner
			float angleStep = (endAngle - startAngle) * Mathf.Pi / 180 / segments;

			for (int i = 0; i < segments; i++)
			{
				float angle1 = (startAngle + i * (endAngle - startAngle) / segments) * Mathf.Pi / 180;
				float angle2 = (startAngle + (i + 1) * (endAngle - startAngle) / segments) * Mathf.Pi / 180;

				float x1 = centerX + radius * Mathf.Cos(angle1);
				float y1 = centerY + radius * Mathf.Sin(angle1);
				float x2 = centerX + radius * Mathf.Cos(angle2);
				float y2 = centerY + radius * Mathf.Sin(angle2);

				DrawLine(x1, y1, x2, y2, color);
			}
		}
	}
}
