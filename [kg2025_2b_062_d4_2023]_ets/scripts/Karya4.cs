using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;
using KG2025.Components;
using KG2025.Components.AnimatedMotifs;
using KG2025.Components.UI;

// Add aliases to prevent ambiguity between Vector2 types
using GodotVector2 = Godot.Vector2;

public partial class Karya4 : Node2D
{
	// Core components
	private KartesiusSystem kartesiusSystem;
	private InputManager inputManager;
	private ColorThemeManager colorThemeManager;
	private InteractiveMotifController animController;
	private HelpOverlay helpOverlay;

	// Colored motifs for quadrants
	private ColoredCrossMotif q1CrossMotif;
	private ColoredEyeMotif q2EyeMotif;
	private ColoredCrossMotif q3CrossMotif;
	private ColoredEyeMotif q4EyeMotif;

	// Center diamond and stepped diamond motifs
	private List<ColoredDiamondMotif> diamondMotifs = new List<ColoredDiamondMotif>();
	private List<ColoredSteppedDiamondMotif> steppedDiamondMotifs = new List<ColoredSteppedDiamondMotif>();

	// Trapezoid motifs
	private ColoredTrapezoidMotif trapezoidMotif;

	// Kolintang motif
	private ColoredKolintangMotif kolintangMotif;

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

	// Kolintang settings
	private float trapezoidTopWidth = 130;
	private float trapezoidBottomWidth = 329;
	private float trapezoidHeight = 1076;

	// Settings (preserved from original Karya4 for exact same appearance)
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
		// Initialize components
		inputManager = new InputManager();
		colorThemeManager = new ColorThemeManager();
		animController = new InteractiveMotifController(diamondSize, stepSize);
		helpOverlay = new HelpOverlay(this);

		// Set animation parameters
		animController.SetAnimationParams(0.3f, breathingSpeed, minScale, maxScale);

		// Get screen dimensions
		screenWidth = (int)GetViewportRect().Size.X;
		screenHeight = (int)GetViewportRect().Size.Y;

		// Calculate center positions
		centerX = screenWidth / 2;
		centerY = screenHeight / 2;

		// Initialize KartesiusSystem
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
	}

	private void InitializeMotifs()
	{
		// Calculate quadrant centers
		var (q1Center, q2Center, q3Center, q4Center) = kartesiusSystem.CalculateQuadrantCenters();

		// Initialize quadrant motifs
		q1CrossMotif = new ColoredCrossMotif(this, kartesiusSystem, q1Center,
										   crossPatternSize, quadrantOrbitRadius, numPatterns);
		q1CrossMotif.SetColorPalette(colorThemeManager);
		q1CrossMotif.SetAnimationParams(0.3f, breathingSpeed, minScale, maxScale);

		q2EyeMotif = new ColoredEyeMotif(this, kartesiusSystem, q2Center,
										eyePatternSize, quadrantOrbitRadius, numPatterns);
		q2EyeMotif.SetColorPalette(colorThemeManager);
		q2EyeMotif.SetAnimationParams(0.3f, breathingSpeed, minScale, maxScale);

		q3CrossMotif = new ColoredCrossMotif(this, kartesiusSystem, q3Center,
										   crossPatternSize, quadrantOrbitRadius, numPatterns);
		q3CrossMotif.SetColorPalette(colorThemeManager);
		q3CrossMotif.SetAnimationParams(0.3f, breathingSpeed, minScale, maxScale);

		q4EyeMotif = new ColoredEyeMotif(this, kartesiusSystem, q4Center,
										eyePatternSize, quadrantOrbitRadius, numPatterns);
		q4EyeMotif.SetColorPalette(colorThemeManager);
		q4EyeMotif.SetAnimationParams(0.3f, breathingSpeed, minScale, maxScale);

		// Initialize diamond and stepped diamond motifs
		InitializeDiamondMotifs();

		// Initialize trapezoid motif
		trapezoidMotif = new ColoredTrapezoidMotif(this, kartesiusSystem,
												 trapezoidSize, trapezoidSpacing,
												 trapezoidMovementSpeed, numTrapezoids, -Mathf.Pi / 6);
		trapezoidMotif.SetColorPalette(colorThemeManager);

		// Initialize kolintang motif
		kolintangMotif = new ColoredKolintangMotif(this, kartesiusSystem,
												  trapezoidTopWidth, trapezoidBottomWidth,
												  trapezoidHeight);
		kolintangMotif.SetColorPalette(colorThemeManager);
		kolintangMotif.SetInteractiveMode(true); // Enable interactive mode for kolintang
	}

	private void InitializeDiamondMotifs()
	{
		// Add diamond motifs with the exact same positions as original Karya4
		diamondMotifs.Add(new ColoredDiamondMotif(this, kartesiusSystem,
			new GodotVector2(centerX, centerY - 400), diamondSize));
		diamondMotifs.Add(new ColoredDiamondMotif(this, kartesiusSystem,
			new GodotVector2(centerX, centerY), diamondSize));
		diamondMotifs.Add(new ColoredDiamondMotif(this, kartesiusSystem,
			new GodotVector2(centerX, centerY + 400), diamondSize));

		// Set colors for all diamond motifs
		foreach (var motif in diamondMotifs)
		{
			motif.SetColorPalette(colorThemeManager);
			motif.SetAnimationParams(0.3f, breathingSpeed, minScale, maxScale);
		}

		// Add stepped diamond motifs with exact same positions/inversions as original
		steppedDiamondMotifs.Add(new ColoredSteppedDiamondMotif(this, kartesiusSystem,
			new GodotVector2(centerX - 10, centerY - 200), stepSize, true)); // Above center, inverse breathing
		steppedDiamondMotifs.Add(new ColoredSteppedDiamondMotif(this, kartesiusSystem,
			new GodotVector2(centerX - 10, centerY + 180), stepSize)); // Below center

		// Set colors for stepped diamond motifs
		foreach (var motif in steppedDiamondMotifs)
		{
			motif.SetColorPalette(colorThemeManager);
			motif.SetAnimationParams(0.3f, breathingSpeed, minScale, maxScale);
		}
	}

	public override void _Process(double delta)
	{
		float deltaF = (float)delta;

		// Process user input
		inputManager.ProcessInput();

		// Update color theme based on input
		colorThemeManager.UpdateColors(inputManager);

		// Update animation controller
		animController.Update(deltaF, inputManager);

		// Update motifs based on current scene
		switch (currentScene)
		{
			case 0:
				// Update original motifs
				UpdateMotifs(deltaF);
				break;
			case 1:
				// Update kolintang motif
				kolintangMotif.Update(deltaF);
				break;
		}

		// Queue redraw to update animation
		QueueRedraw();
	}

	private void UpdateMotifs(float delta)
	{
		// Update quadrant motifs
		q1CrossMotif.SetOrbitAngle(animController.OrbitAngle);
		q1CrossMotif.SetBreathingFactor(animController.BreathingFactor);
		q1CrossMotif.Update(delta);

		q2EyeMotif.SetOrbitAngle(animController.OrbitAngle);
		q2EyeMotif.SetBreathingFactor(animController.BreathingFactor);
		q2EyeMotif.Update(delta);

		q3CrossMotif.SetOrbitAngle(animController.OrbitAngle);
		q3CrossMotif.SetBreathingFactor(animController.BreathingFactor);
		q3CrossMotif.Update(delta);

		q4EyeMotif.SetOrbitAngle(animController.OrbitAngle);
		q4EyeMotif.SetBreathingFactor(animController.BreathingFactor);
		q4EyeMotif.Update(delta);

		// Update diamond motifs
		foreach (var motif in diamondMotifs)
		{
			motif.SetBreathingFactor(animController.DiamondBreathingFactor);
			motif.Update(delta);
		}

		// Update stepped diamond motifs
		foreach (var motif in steppedDiamondMotifs)
		{
			motif.SetBreathingFactor(animController.SteppedDiamondBreathingFactor);
			motif.Update(delta);
		}

		// Update trapezoid motif
		float totalHeight = numTrapezoids * trapezoidSpacing;
		trapezoidMotif.SetYOffset(animController.GetTrapezoidYOffset(totalHeight, centerY));
		trapezoidMotif.SetReverseMovement(inputManager.ReverseTrapezoidMovement);
		trapezoidMotif.Update(delta);
	}

	public override void _Draw()
	{
		// Draw cream background
		DrawRect(new Rect2(0, 0, screenWidth, screenHeight), colorThemeManager.Cream);

		// Draw based on current scene
		switch (currentScene)
		{
			case 0:
				// Draw diamond background and patterns
				DrawDiamondBackground();
				DrawDiamondPatterns();
				DrawDiamondBorders();

				// Draw quadrant patterns
				DrawQuadrantPatterns();

				// Draw trapezoid patterns
				trapezoidMotif.Draw();

				// Draw help overlay if enabled
				helpOverlay.Draw(inputManager);
				break;
			case 1:
				// Draw kolintang pattern
				kolintangMotif.Draw(centerX, centerY);

				// Draw instructions for kolintang
				DrawKolintangInstructions();
				break;
		}

		// Always draw the navigation button
		DrawNextButton();
	}

	// Draw instructions for kolintang motif
	private void DrawKolintangInstructions()
	{
		// Only show instructions when in help mode
		if (!inputManager.ShowHelp)
			return;

		float padding = 20;
		float lineHeight = 24;
		float instructionHeight = 200; // Total height of instructions

		// Position in bottom left corner
		float x = 50;
		float y = screenHeight - instructionHeight - padding;

		// Background for instructions
		Vector2 textSize = new Vector2(400, instructionHeight - padding * 2);
		DrawRect(new Rect2(x - padding, y - padding, textSize.X + padding * 2, textSize.Y + padding * 2),
				 new Color(0, 0, 0, 0.7f));

		// Title - Use correct parameter order for DrawString in Godot 4.x
		// Parameters: font, position, text, alignment, width, fontSize, color
		DrawString(ThemeDB.FallbackFont, new Vector2(x, y), "Kolintang Instructions:",
				   HorizontalAlignment.Left, -1, 22, new Color(1, 1, 1));
		y += lineHeight * 1.5f;

		// Instructions content - Use correct parameter order
		Color instructionColor = new Color(1, 0.9f, 0.7f);
		DrawString(ThemeDB.FallbackFont, new Vector2(x, y), "• Click on any wooden bar to play it",
				   HorizontalAlignment.Left, -1, 16, instructionColor);
		y += lineHeight;

		DrawString(ThemeDB.FallbackFont, new Vector2(x, y), "• Press 'A' key to toggle auto-play mode",
				   HorizontalAlignment.Left, -1, 16, instructionColor);
		y += lineHeight;

		DrawString(ThemeDB.FallbackFont, new Vector2(x, y), "• Press 'H' key to show/hide this help",
				   HorizontalAlignment.Left, -1, 16, instructionColor);
		y += lineHeight;

		DrawString(ThemeDB.FallbackFont, new Vector2(x, y), "• Click the arrow button to switch motifs",
				   HorizontalAlignment.Left, -1, 16, instructionColor);
		y += lineHeight * 1.5f;

		// Add note about traditional instrument
		DrawString(ThemeDB.FallbackFont, new Vector2(x, y), "Kolintang is a traditional wooden percussion",
				   HorizontalAlignment.Left, -1, 16, new Color(0.8f, 0.8f, 1));
		y += lineHeight;

		DrawString(ThemeDB.FallbackFont, new Vector2(x, y), "instrument from North Sulawesi, Indonesia.",
				   HorizontalAlignment.Left, -1, 16, new Color(0.8f, 0.8f, 1));
	}

	private void DrawDiamondBackground()
	{
		// Create rectangle background for diamond section
		float verticalLineOffset = 110;
		float topY = centerY - 500;
		float bottomY = centerY + 500;

		DrawRect(new Rect2(centerX - verticalLineOffset - 40, topY,
						 (verticalLineOffset + 40) * 2, bottomY - topY),
				colorThemeManager.LightOrange, true);
	}

	private void DrawQuadrantPatterns()
	{
		// Draw all quadrant motifs
		q1CrossMotif.Draw();
		q2EyeMotif.Draw();
		q3CrossMotif.Draw();
		q4EyeMotif.Draw();
	}

	private void DrawDiamondPatterns()
	{
		// Draw all diamond motifs
		foreach (var motif in diamondMotifs)
		{
			motif.Draw();
		}

		// Draw all stepped diamond motifs
		foreach (var motif in steppedDiamondMotifs)
		{
			motif.Draw();
		}
	}

	private void DrawDiamondBorders()
	{
		// Draw vertical border rectangles
		float verticalLineOffset = 110;
		float topY = centerY - 500;
		float bottomY = centerY + 500;

		// Left vertical border
		DrawRect(new Rect2(centerX - verticalLineOffset - 40, topY, 40, bottomY - topY),
				colorThemeManager.DarkOrange, true);

		// Right vertical border
		DrawRect(new Rect2(centerX + verticalLineOffset, topY, 40, bottomY - topY),
				colorThemeManager.DarkOrange, true);
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
			if (buttonEvent.Pressed)
			{
				if (nextButtonRect.HasPoint(buttonEvent.Position))
				{
					isNextButtonPressed = true;
					QueueRedraw();
				}
				else if (currentScene == 1)
				{
					// Handle clicks on kolintang bars when in kolintang scene
					kolintangMotif.HandleMouseClick(buttonEvent.Position);
					QueueRedraw();
				}
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
		else if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			// Check for A key press to toggle auto-play for kolintang
			if (keyEvent.Keycode == Key.A && currentScene == 1)
			{
				kolintangMotif.ToggleAutoPlayMode();
			}
		}
	}
}
