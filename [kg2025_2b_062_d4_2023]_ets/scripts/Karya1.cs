using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;
using KG2025.Components;
using KG2025.Components.Motifs;

public partial class Karya1 : Node2D
{
	// Components
	private KartesiusSystem kartesiusSystem;
	private CrossMotif crossMotif;
	private EyeMotif eyeMotif;
	private DiamondMotif diamondMotif;
	private SteppedDiamondMotif steppedDiamondMotif;
	private TrapezoidMotif trapezoidMotif;
	private KolintangMotif kolintangMotif; // Add KolintangMotif component

	// State tracking for "scenes"
	private int currentScene = 0;
	private const int TOTAL_SCENES = 2;

	// Trapezoid dimensions for kolintang
	private float trapezoidBottomWidth = 329;
	private float trapezoidTopWidth = 130;
	private float trapezoidHeight = 1076;

	// Button properties
	private Texture2D nextButtonTexture;
	private Texture2D nextButtonHoverTexture;
	private Rect2 nextButtonRect;
	private bool isNextButtonHovered = false;
	private bool isNextButtonPressed = false;
	private float buttonSize = 54;

	// Colors - keep the same colors as original
	private Color lineColor = Colors.White;
	private Color circleColor = Colors.White;

	// Original motif positions - keep these exactly as they were
	private float centerX = 400;
	private float centerY = 300;
	private float motif2X = 600; // X position for the second motif
	private float motif2Y = 300; // Y position for the second motif
	private float motif3X = 800; // X position for the third motif
	private float motif3Y = 300; // Y position for the third motif
	private float motif4X = 1000; // X position for the fourth motif
	private float motif4Y = 300; // Y position for the fourth motif

	// Motif sizes - maintain the same sizes
	private float hexagonSize = 90;
	private float outerCircleRadius = 60;
	private float innerCircleRadius = 10;
	private float crossWidth = 40;
	private float crossHeight = 120;
	private float quadrantMotifSize = 70; // For quadrant placement
	private float stepSize = 20; // For stepped diamond
	private float diamondSize = 30; // For diamond motif

	// Screen dimensions
	private int screenWidth;
	private int screenHeight;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Initialize Kartesius system with EXACT same settings as original
		kartesiusSystem = new KartesiusSystem(this);
		kartesiusSystem.ShowKartesius = true;
		kartesiusSystem.ShowGrid = false; // Keep this false as in the original
		kartesiusSystem.MarginLeft = 50;
		kartesiusSystem.MarginTop = 50;
		kartesiusSystem.MarginRight = 50;
		kartesiusSystem.MarginBottom = 50;
		kartesiusSystem.GridSpacing = 50;
		kartesiusSystem.GridColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);
		kartesiusSystem.AxisColor = new Color(0.5f, 0.5f, 0.5f);

		// Update screen dimensions
		screenWidth = (int)GetViewportRect().Size.X;
		screenHeight = (int)GetViewportRect().Size.Y;
		kartesiusSystem.UpdateScreenDimensions();

		// Load button textures
		nextButtonTexture = GD.Load<Texture2D>("res://assets/ui/Next.png");
		nextButtonHoverTexture = GD.Load<Texture2D>("res://assets/ui/Next-on.png");

		// Position the button at the bottom right corner
		nextButtonRect = new Rect2(
			screenWidth - buttonSize - 20,  // X position (20px margin from right)
			screenHeight - buttonSize - 20, // Y position (20px margin from bottom)
			buttonSize,
			buttonSize
		);

		// Initialize motifs
		crossMotif = new CrossMotif(this, kartesiusSystem);
		crossMotif.SetColors(lineColor);

		eyeMotif = new EyeMotif(this, kartesiusSystem);
		eyeMotif.SetColors(lineColor);

		diamondMotif = new DiamondMotif(this, kartesiusSystem);
		diamondMotif.SetColors(lineColor);

		steppedDiamondMotif = new SteppedDiamondMotif(this, kartesiusSystem);
		steppedDiamondMotif.SetColors(lineColor);

		trapezoidMotif = new TrapezoidMotif(this, kartesiusSystem);
		trapezoidMotif.SetColors(lineColor);

		// Initialize the new kolintang motif
		kolintangMotif = new KolintangMotif(this, kartesiusSystem);
		kolintangMotif.SetColors(Colors.White, Colors.White);
		kolintangMotif.SetMalletsVisibility(true); // Enable mallets display

		GD.Print("Kolintang initialized with mallets enabled"); // Debug log
	}

	public override void _Draw()
	{
		// Draw based on current scene
		switch (currentScene)
		{
			case 0:
				// Draw original scene
				if (kartesiusSystem.ShowKartesius)
				{
					kartesiusSystem.DrawKartesius();
					DrawMotifsInQuadrants();
				}
				break;

			case 1:
				// Draw kolintang scene
				DrawKolintang();
				break;
		}

		// Always draw the navigation button
		DrawNextButton();
	}

	// Draw the kolintang using the modular component
	private void DrawKolintang()
	{
		// Center coordinates
		float centerX = screenWidth / 2;
		float centerY = screenHeight / 2;

		GD.Print($"Drawing kolintang at ({centerX}, {centerY}) with trapezoid: {trapezoidTopWidth}x{trapezoidBottomWidth}x{trapezoidHeight}"); // Debug

		// Draw the horizontal kolintang (rotated 90 degrees)
		kolintangMotif.Draw(
			centerX,
			centerY,
			trapezoidTopWidth,    // Left width (was top width)
			trapezoidBottomWidth, // Right width (was bottom width)
			trapezoidHeight       // Length (was height)
		);
	}

	// Draw the Next button
	private void DrawNextButton()
	{
		Texture2D currentTexture = isNextButtonHovered ? nextButtonHoverTexture : nextButtonTexture;
		DrawTextureRect(currentTexture, nextButtonRect, false);
	}

	// Handle input events
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motionEvent)
		{
			// Check if mouse is hovering over the button
			isNextButtonHovered = nextButtonRect.HasPoint(motionEvent.Position);
			QueueRedraw();
		}
		else if (@event is InputEventMouseButton buttonEvent && buttonEvent.ButtonIndex == MouseButton.Left)
		{
			if (buttonEvent.Pressed && nextButtonRect.HasPoint(buttonEvent.Position))
			{
				isNextButtonPressed = true;
				QueueRedraw();
			}
			else if (!buttonEvent.Pressed && isNextButtonPressed)
			{
				isNextButtonPressed = false;

				// Check if button was released while hovering over it
				if (nextButtonRect.HasPoint(buttonEvent.Position))
				{
					// Switch to next scene
					currentScene = (currentScene + 1) % TOTAL_SCENES;
				}

				QueueRedraw();
			}
		}
	}

	// Draw motifs in specified quadrants
	private void DrawMotifsInQuadrants()
	{
		// Get the center of each quadrant - same calculation as original
		var (q1Center, q2Center, q3Center, q4Center) = kartesiusSystem.CalculateQuadrantCenters();

		// Draw motifs in quadrants - maintain exact same placement as in original
		crossMotif.Draw(q1Center.X, q1Center.Y, quadrantMotifSize); // Q1 (top-right)
		eyeMotif.Draw(q2Center.X, q2Center.Y, quadrantMotifSize);   // Q2 (top-left)
		crossMotif.Draw(q3Center.X, q3Center.Y, quadrantMotifSize); // Q3 (bottom-left)
		eyeMotif.Draw(q4Center.X, q4Center.Y, quadrantMotifSize);   // Q4 (bottom-right)

		// Draw diamond patterns - maintain exact same positions as original
		Vector2 aboveInPixels = kartesiusSystem.ConvertToPixel(0, 400);
		diamondMotif.Draw(aboveInPixels.X, aboveInPixels.Y, 50);

		// Draw stepped diamond patterns - maintain exact same positions
		Vector2 abovePositionInPixels = kartesiusSystem.ConvertToPixel(-10, 200);
		steppedDiamondMotif.Draw(abovePositionInPixels.X, abovePositionInPixels.Y, stepSize);

		Vector2 middleInPixels = kartesiusSystem.ConvertToPixel(0, 0);
		diamondMotif.Draw(middleInPixels.X, middleInPixels.Y, 50);

		Vector2 belowPositionInPixels = kartesiusSystem.ConvertToPixel(-10, -180);
		steppedDiamondMotif.Draw(belowPositionInPixels.X, belowPositionInPixels.Y, stepSize);

		Vector2 belowInPixels = kartesiusSystem.ConvertToPixel(0, -400);
		diamondMotif.Draw(belowInPixels.X, belowInPixels.Y, 50);

		// Draw vertical lines to enclose the diamond patterns - same positions as original
		float verticalLineOffset = 110;  // Distance from center where to place vertical lines
		float topY = 500;                // Top of the vertical lines
		float bottomY = -500;            // Bottom of the vertical lines

		// Draw vertical boundary lines - same as original
		kartesiusSystem.DrawVerticalLine(-verticalLineOffset, topY, bottomY);
		kartesiusSystem.DrawVerticalLine(-verticalLineOffset - 40, topY, bottomY);
		kartesiusSystem.DrawVerticalLine(verticalLineOffset, topY, bottomY);
		kartesiusSystem.DrawVerticalLine(verticalLineOffset + 40, topY, bottomY);

		// Draw trapezoid patterns - using same parameters as original
		float parallelogramSize = 60;
		float rotationOffset = -Mathf.Pi / 6;

		// Left side trapezoids - exactly the same pattern as original
		DrawTrapezoidLeftSide(parallelogramSize, rotationOffset);

		// Right side trapezoids - exactly the same pattern as original
		DrawTrapezoidRightSide(parallelogramSize, rotationOffset);

		// Add vertical boundary lines next to the trapezoids - same positions as original
		topY = 700;      // Top of the vertical lines
		bottomY = -700;  // Bottom of the vertical lines

		// Add trapezoid boundary lines - with exact same positions
		float leftTrapezoidEdge = -800;
		float rightTrapezoidEdge = 760;
		kartesiusSystem.DrawTrapezoidBoundaryLines(leftTrapezoidEdge, topY, bottomY);
		kartesiusSystem.DrawTrapezoidBoundaryLines(leftTrapezoidEdge - 142, topY, bottomY);
		kartesiusSystem.DrawTrapezoidBoundaryLines(rightTrapezoidEdge, topY, bottomY);
		kartesiusSystem.DrawTrapezoidBoundaryLines(rightTrapezoidEdge + 141, topY, bottomY);

		// Draw horizontal lines between quadrants - same positions as original
		float leftBound = -verticalLineOffset - 40;
		float rightBound = verticalLineOffset + 40;

		// Draw horizontal lines with exact same positions
		kartesiusSystem.DrawHorizontalLine(20, -760, leftBound);
		kartesiusSystem.DrawHorizontalLine(-20, -760, leftBound);
		kartesiusSystem.DrawHorizontalLine(20, rightBound, 759);
		kartesiusSystem.DrawHorizontalLine(-20, rightBound, 759);
	}

	// Draw the left side trapezoid pattern exactly as in the original
	private void DrawTrapezoidLeftSide(float size, float rotationOffset)
	{
		trapezoidMotif.DrawInCartesianWithOrientation(-800, 120 * 5, size, rotationOffset, false);
		trapezoidMotif.DrawInCartesianWithOrientation(-900, 120 * 4, size, rotationOffset, true);
		trapezoidMotif.DrawInCartesianWithOrientation(-800, 120 * 3, size, rotationOffset, false);
		trapezoidMotif.DrawInCartesianWithOrientation(-900, 120 * 2, size, rotationOffset, true);
		trapezoidMotif.DrawInCartesianWithOrientation(-800, 120, size, rotationOffset, false);
		trapezoidMotif.DrawInCartesianWithOrientation(-900, 0, size, rotationOffset, true);
		trapezoidMotif.DrawInCartesianWithOrientation(-800, -120, size, rotationOffset, false);
		trapezoidMotif.DrawInCartesianWithOrientation(-900, -120 * 2, size, rotationOffset, true);
		trapezoidMotif.DrawInCartesianWithOrientation(-800, -120 * 3, size, rotationOffset, false);
		trapezoidMotif.DrawInCartesianWithOrientation(-900, -120 * 4, size, rotationOffset, true);
		trapezoidMotif.DrawInCartesianWithOrientation(-800, -120 * 5, size, rotationOffset, false);
	}

	// Draw the right side trapezoid pattern exactly as in the original
	private void DrawTrapezoidRightSide(float size, float rotationOffset)
	{
		trapezoidMotif.DrawInCartesianWithOrientation(800, 120 * 5, size, rotationOffset, true);
		trapezoidMotif.DrawInCartesianWithOrientation(900, 120 * 4, size, rotationOffset, false);
		trapezoidMotif.DrawInCartesianWithOrientation(800, 120 * 3, size, rotationOffset, true);
		trapezoidMotif.DrawInCartesianWithOrientation(900, 120 * 2, size, rotationOffset, false);
		trapezoidMotif.DrawInCartesianWithOrientation(800, 120, size, rotationOffset, true);
		trapezoidMotif.DrawInCartesianWithOrientation(900, 0, size, rotationOffset, false);
		trapezoidMotif.DrawInCartesianWithOrientation(800, -120, size, rotationOffset, true);
		trapezoidMotif.DrawInCartesianWithOrientation(900, -120 * 2, size, rotationOffset, false);
		trapezoidMotif.DrawInCartesianWithOrientation(800, -120 * 3, size, rotationOffset, true);
		trapezoidMotif.DrawInCartesianWithOrientation(900, -120 * 4, size, rotationOffset, false);
		trapezoidMotif.DrawInCartesianWithOrientation(800, -120 * 5, size, rotationOffset, true);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		QueueRedraw(); // Ensure everything is continuously drawn
	}

	// Toggle Kartesius display - maintain the same functionality
	public void ToggleKartesius()
	{
		kartesiusSystem.ShowKartesius = !kartesiusSystem.ShowKartesius;
	}
}
