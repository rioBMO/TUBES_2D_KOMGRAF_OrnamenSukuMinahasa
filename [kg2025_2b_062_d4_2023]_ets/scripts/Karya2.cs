using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;
using KG2025.Components;
using KG2025.Components.AnimatedMotifs;

// Add aliases to prevent ambiguity between Vector2 types
using GodotVector2 = Godot.Vector2;

public partial class Karya2 : Node2D
{
	// Core components
	private KartesiusSystem kartesiusSystem;

	// Animated motifs for quadrants
	private AnimatedCrossMotif q1CrossMotif;
	private AnimatedEyeMotif q2EyeMotif;
	private AnimatedCrossMotif q3CrossMotif;
	private AnimatedEyeMotif q4EyeMotif;

	// Center diamond and stepped diamond motifs
	private List<AnimatedDiamondMotif> diamondMotifs = new List<AnimatedDiamondMotif>();
	private List<AnimatedSteppedDiamondMotif> steppedDiamondMotifs = new List<AnimatedSteppedDiamondMotif>();

	// Trapezoid motifs
	private AnimatedTrapezoidMotif trapezoidMotif;

	// State tracking for scenes
	private int currentScene = 0;
	private const int TOTAL_SCENES = 2;

	// Button properties
	private Texture2D nextButtonTexture;
	private Texture2D nextButtonHoverTexture;
	private Rect2 nextButtonRect;
	private bool isNextButtonHovered = false;
	private bool isNextButtonPressed = false;
	private float buttonSize = 54;

	// Animated Kolintang
	private AnimatedKolintangMotif kolintangMotif;
	private float trapezoidTopWidth = 130;
	private float trapezoidBottomWidth = 329;
	private float trapezoidHeight = 1076;

	// Settings (preserved from original Karya2 for exact same appearance)
	private Color lineColor = Colors.White;
	private float eyePatternSize = 70;
	private float crossPatternSize = 70;
	private float diamondSize = 50;
	private float stepSize = 20;
	private float orbitRadius = 130f;
	private float quadrantOrbitRadius = 130f;
	private int numPatterns = 6;
	private float trapezoidSize = 60;
	private float trapezoidSpacing = 100;
	private float trapezoidMovementSpeed = 60.0f;
	private int numTrapezoids = 24;

	// Breathing animation settings
	private float minScale = 0.7f;
	private float maxScale = 1.3f;
	private float breathingSpeed = 0.5f;

	// Screen properties
	private int screenWidth;
	private int screenHeight;
	private float centerX;
	private float centerY;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get screen dimensions
		screenWidth = (int)GetViewportRect().Size.X;
		screenHeight = (int)GetViewportRect().Size.Y;

		// Calculate center positions
		centerX = screenWidth / 2;
		centerY = screenHeight / 2;

		// Initialize KartesiusSystem with the same parameters as the original Karya2
		kartesiusSystem = new KartesiusSystem(this);
		kartesiusSystem.ShowKartesius = false;
		kartesiusSystem.MarginLeft = 50;
		kartesiusSystem.MarginTop = 50;
		kartesiusSystem.MarginRight = 50;
		kartesiusSystem.MarginBottom = 50;
		kartesiusSystem.ShowGrid = false;
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

		// Initialize all modularized components
		InitializeMotifs();

		// Initialize the animated kolintang
		kolintangMotif = new AnimatedKolintangMotif(this, kartesiusSystem,
			trapezoidTopWidth, trapezoidBottomWidth, trapezoidHeight);
		kolintangMotif.SetColors(lineColor);
	}

	private void InitializeMotifs()
	{
		// Calculate quadrant centers exactly as in original Karya2
		var (q1Center, q2Center, q3Center, q4Center) = kartesiusSystem.CalculateQuadrantCenters();

		// Initialize quadrant motifs with EXACT same parameters as original Karya2
		q1CrossMotif = new AnimatedCrossMotif(this, kartesiusSystem, q1Center,
											 crossPatternSize, quadrantOrbitRadius, numPatterns);
		q1CrossMotif.SetColors(lineColor);
		q1CrossMotif.SetAnimationParams(0.3f, breathingSpeed, minScale, maxScale);

		q2EyeMotif = new AnimatedEyeMotif(this, kartesiusSystem, q2Center,
										 eyePatternSize, quadrantOrbitRadius, numPatterns);
		q2EyeMotif.SetColors(lineColor);
		q2EyeMotif.SetAnimationParams(0.3f, breathingSpeed, minScale, maxScale);

		q3CrossMotif = new AnimatedCrossMotif(this, kartesiusSystem, q3Center,
											 crossPatternSize, quadrantOrbitRadius, numPatterns);
		q3CrossMotif.SetColors(lineColor);
		q3CrossMotif.SetAnimationParams(0.3f, breathingSpeed, minScale, maxScale);

		q4EyeMotif = new AnimatedEyeMotif(this, kartesiusSystem, q4Center,
										 eyePatternSize, quadrantOrbitRadius, numPatterns);
		q4EyeMotif.SetColors(lineColor);
		q4EyeMotif.SetAnimationParams(0.3f, breathingSpeed, minScale, maxScale);

		// Initialize diamond and stepped diamond motifs
		InitializeDiamondMotifs();

		// Initialize trapezoid motif with exact same parameters as original
		trapezoidMotif = new AnimatedTrapezoidMotif(this, kartesiusSystem,
												  trapezoidSize, trapezoidSpacing,
												  trapezoidMovementSpeed, numTrapezoids, -Mathf.Pi / 6);
		trapezoidMotif.SetColors(lineColor);
	}

	private void InitializeDiamondMotifs()
	{
		// Add diamond motifs with the exact same positions as original Karya2
		diamondMotifs.Add(new AnimatedDiamondMotif(this, kartesiusSystem,
			new GodotVector2(centerX, centerY - 400), diamondSize));
		diamondMotifs.Add(new AnimatedDiamondMotif(this, kartesiusSystem,
			new GodotVector2(centerX, centerY), diamondSize));
		diamondMotifs.Add(new AnimatedDiamondMotif(this, kartesiusSystem,
			new GodotVector2(centerX, centerY + 400), diamondSize));

		// Set colors for all diamond motifs
		foreach (var motif in diamondMotifs)
		{
			motif.SetColors(lineColor);
			motif.SetAnimationParams(0.3f, breathingSpeed, minScale, maxScale);
		}

		// Add stepped diamond motifs with exact same positions/inversions as original Karya2
		steppedDiamondMotifs.Add(new AnimatedSteppedDiamondMotif(this, kartesiusSystem,
			new GodotVector2(centerX - 10, centerY - 200), stepSize, true)); // Above center
		steppedDiamondMotifs.Add(new AnimatedSteppedDiamondMotif(this, kartesiusSystem,
			new GodotVector2(centerX - 10, centerY + 180), stepSize)); // Below center

		// Set colors for stepped diamond motifs
		foreach (var motif in steppedDiamondMotifs)
		{
			motif.SetColors(lineColor);
			motif.SetAnimationParams(0.3f, breathingSpeed, minScale, maxScale);
		}
	}

	public override void _Process(double delta)
	{
		float deltaF = (float)delta;

		// Update animations based on current scene
		switch (currentScene)
		{
			case 0:
				// Update original animated motifs
				q1CrossMotif.Update(deltaF);
				q2EyeMotif.Update(deltaF);
				q3CrossMotif.Update(deltaF);
				q4EyeMotif.Update(deltaF);

				// Update diamond motifs
				foreach (var motif in diamondMotifs)
				{
					motif.Update(deltaF);
				}

				// Update stepped diamond motifs
				foreach (var motif in steppedDiamondMotifs)
				{
					motif.Update(deltaF);
				}

				// Update trapezoid motif
				trapezoidMotif.Update(deltaF);
				break;

			case 1:
				// Update kolintang animation
				kolintangMotif.Update(deltaF);
				break;
		}

		// Queue redraw to update animation
		QueueRedraw();
	}

	public override void _Draw()
	{
		// Draw based on current scene
		switch (currentScene)
		{
			case 0:
				// Draw original patterns
				DrawQuadrantPatterns();
				DrawDiamondPatterns();
				trapezoidMotif.Draw();
				break;

			case 1:
				// Draw kolintang
				kolintangMotif.Draw(centerX, centerY);
				break;
		}

		// Always draw the navigation button
		DrawNextButton();
	}

	private void DrawQuadrantPatterns()
	{
		// Draw quadrant motifs using the modular components
		q1CrossMotif.Draw();
		q2EyeMotif.Draw();
		q3CrossMotif.Draw();
		q4EyeMotif.Draw();
	}

	private void DrawDiamondPatterns()
	{
		// Draw diamond motifs
		foreach (var motif in diamondMotifs)
		{
			motif.Draw();
		}

		// Draw stepped diamond motifs
		foreach (var motif in steppedDiamondMotifs)
		{
			motif.Draw();
		}

		// Draw vertical boundary lines exactly as in original Karya2
		float verticalLineOffset = 110;
		float topY = 500;
		float bottomY = -500;

		kartesiusSystem.DrawVerticalLine(-verticalLineOffset, topY, bottomY);
		kartesiusSystem.DrawVerticalLine(-verticalLineOffset - 40, topY, bottomY);
		kartesiusSystem.DrawVerticalLine(verticalLineOffset, topY, bottomY);
		kartesiusSystem.DrawVerticalLine(verticalLineOffset + 40, topY, bottomY);
	}

	// Handle input events for button interaction
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

	// Draw the Next button
	private void DrawNextButton()
	{
		Texture2D currentTexture = isNextButtonHovered ? nextButtonHoverTexture : nextButtonTexture;
		DrawTextureRect(currentTexture, nextButtonRect, false);
	}
}
