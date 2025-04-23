using Godot;
using System;

public partial class IconButton : TextureButton
{
    [Signal]
    public delegate void IconSelectedEventHandler(string iconName);

    private string _iconName;

    public void Initialize(string normalPath, string activePath, string iconName)
    {
        _iconName = iconName;

        var textureNormal = GD.Load<Texture2D>(normalPath);
        var textureActive = GD.Load<Texture2D>(activePath);

        if (textureNormal != null && textureActive != null)
        {
            TextureNormal = textureNormal;
            TextureHover = textureActive;
            TexturePressed = textureActive;
            TextureFocused = textureActive;

            ToggleMode = true;
            TextureDisabled = textureNormal;

            IgnoreTextureSize = false;
            StretchMode = TextureButton.StretchModeEnum.KeepAspectCentered;

            Pressed += () => AudioManager.Instance.PlayButtonSound(this, Name);

            Toggled += OnToggled;
        }
        else
        {
            GD.Print($"Couldn't load button images: {normalPath} or {activePath}");
        }
    }

    private void OnToggled(bool toggled)
    {
        if (toggled)
        {
            EmitSignal(SignalName.IconSelected, _iconName);
        }
    }
}
