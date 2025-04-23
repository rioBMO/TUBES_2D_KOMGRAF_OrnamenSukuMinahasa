using Godot;
using System;

public partial class MainMenu : Control
{
	// UI components
	private ProfilePanel _profilePanel;
	private GuidePanel _guidePanel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Initialize audio manager for singleton access
		var audioManager = AudioManager.Instance;

		// Initialize profile panel
		_profilePanel = new ProfilePanel();
		AddChild(_profilePanel);
		_profilePanel.Initialize(new Vector2(50, 50));

		// Initialize guide panel
		_guidePanel = new GuidePanel();
		AddChild(_guidePanel);
		_guidePanel.Initialize(new Vector2(50, 50));
	}

	// Handle button press events
	private void _on_karya1Btn_pressed()
	{
		AudioManager.Instance.PlayButtonSound(this, "karya1Btn",
			() => GetTree().ChangeSceneToFile("res://scenes/062_Motif2D.tscn"));
	}

	private void _on_karya2Btn_pressed()
	{
		AudioManager.Instance.PlayButtonSound(this, "karya2Btn",
			() => GetTree().ChangeSceneToFile("res://scenes/062_Motif2D_Animasi.tscn"));
	}

	private void _on_karya3Btn_pressed()
	{
		AudioManager.Instance.PlayButtonSound(this, "karya3Btn",
			() => GetTree().ChangeSceneToFile("res://scenes/062_Motif2D_Polygon_Animasi.tscn"));
	}

	private void _on_karya4Btn_pressed()
	{
		AudioManager.Instance.PlayButtonSound(this, "karya4Btn",
			() => GetTree().ChangeSceneToFile("res://scenes/062_Motif2D_Animasi_dan_Interaksi.tscn"));
	}

	private void _on_guideBtn_pressed()
	{
		// Play button sound first
		AudioManager.Instance.PlayButtonSound(this, "guideBtn");

		// Delay to wait for button-click sound to finish (0.34 seconds)
		GetTree().CreateTimer(0.34).Timeout += () => _guidePanel.Toggle();
	}

	private void _on_aboutBtn_pressed()
	{
		// Play button sound first
		AudioManager.Instance.PlayButtonSound(this, "aboutBtn");

		// Delay to wait for button-click sound to finish (0.34 seconds)
		GetTree().CreateTimer(0.34).Timeout += () => _profilePanel.Toggle();
	}

	private void _on_exitBtn_pressed()
	{
		AudioManager.Instance.PlayButtonSound(this, "exitBtn", () => GetTree().Quit());
	}
}
