using Godot;
using System;

public partial class karya_btn : Button
{
	private void _on_backBtn_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/062_MainMenu.tscn");
	}
}
