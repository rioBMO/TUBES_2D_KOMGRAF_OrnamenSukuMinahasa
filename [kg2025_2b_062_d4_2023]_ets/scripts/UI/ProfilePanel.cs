using Godot;
using System;

public partial class ProfilePanel : Control
{
    // Constants
    private const float ANIMATION_DURATION = 0.4f;
    private const int BUTTON_SIZE = 50;
    private const int BUTTON_SPACING = 91;

    // UI Elements
    private ColorRect _topRectangle;
    private ColorRect _bottomRectangle;
    private TextureRect _iconDisplay;
    private Label _iconTextLabel;
    private Label[] _profileLabels = new Label[4];
    private FontFile _iconFont;

    // State
    private bool _isVisible = false;
    private Vector2 _originalTopPosition;
    private Vector2 _originalBottomPosition;

    [Signal]
    public delegate void IconSelectedEventHandler(string iconName);

    public override void _Ready()
    {
        // Load font
        _iconFont = GD.Load<FontFile>("res://assets/fonts/SAOUITT-Regular.ttf");
        if (_iconFont == null)
        {
            _iconFont = GD.Load<FontFile>("res://fonts/SAOUITT-Regular.ttf");
        }
    }

    public void Initialize(Vector2 position)
    {
        // Initialize top rectangle
        _topRectangle = new ColorRect
        {
            Size = new Vector2(510, 595),
            Color = new Color("#FBFBFB", 0.9f),
            Position = position
        };
        AddChild(_topRectangle);

        // Initialize bottom rectangle
        _bottomRectangle = new ColorRect
        {
            Size = new Vector2(510, 295),
            Color = new Color("#D7D7D7", 0.9f),
            Position = new Vector2(position.X, position.Y + _topRectangle.Size.Y)
        };
        AddChild(_bottomRectangle);

        // Store original positions
        _originalTopPosition = _topRectangle.Position;
        _originalBottomPosition = _bottomRectangle.Position;

        // Create horizontal line
        var horizontalLine = new ColorRect
        {
            Size = new Vector2(442, 2),
            Position = new Vector2((_topRectangle.Size.X - 442) / 2, 91),
            Color = new Color("#333333")
        };
        _topRectangle.AddChild(horizontalLine);

        // Create profile image
        var profileImage = new TextureRect
        {
            Texture = GD.Load<Texture2D>("res://assets/ui/profil.png"),
            CustomMinimumSize = new Vector2(213, 384),
            Size = new Vector2(213, 384),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            Position = new Vector2(
                (_topRectangle.Size.X - 213) / 2,
                horizontalLine.Position.Y + horizontalLine.Size.Y + 15
            )
        };
        _topRectangle.AddChild(profileImage);

        // Create title label
        var titleLabel = new Label
        {
            Text = "rioBMO",
            HorizontalAlignment = HorizontalAlignment.Center,
            Position = new Vector2(0, 45),
            Size = new Vector2(_topRectangle.Size.X, 40)
        };

        if (_iconFont != null)
        {
            titleLabel.AddThemeFontOverride("font", _iconFont);
            titleLabel.AddThemeFontSizeOverride("font_size", 37);
        }

        titleLabel.AddThemeColorOverride("font_color", new Color("#4D4D4D"));
        _topRectangle.AddChild(titleLabel);

        // Create icon buttons
        CreateIconButtons(profileImage);

        // Create icon display
        _iconDisplay = new TextureRect
        {
            Position = new Vector2(36, 30),
            Size = new Vector2(52.5f, 52.5f),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            Texture = null
        };
        _bottomRectangle.AddChild(_iconDisplay);

        // Create icon text label
        _iconTextLabel = new Label
        {
            Position = new Vector2(36 + 52.5f + 15, 30),
            Size = new Vector2(_bottomRectangle.Size.X - (36 + 52.5f + 30), 52.5f),
            VerticalAlignment = VerticalAlignment.Center,
            Text = ""
        };

        if (_iconFont != null)
        {
            _iconTextLabel.AddThemeFontOverride("font", _iconFont);
            _iconTextLabel.AddThemeFontSizeOverride("font_size", 33);
        }

        _iconTextLabel.AddThemeColorOverride("font_color", new Color("#4D4D4D"));
        _bottomRectangle.AddChild(_iconTextLabel);

        // Hide panels initially
        _topRectangle.Visible = false;
        _bottomRectangle.Visible = false;
    }

    private void CreateIconButtons(TextureRect profileImage)
    {
        float buttonY = profileImage.Position.Y + profileImage.Size.Y + 28;
        float totalWidth = (3 * BUTTON_SIZE) + (2 * BUTTON_SPACING);
        float startX = (_topRectangle.Size.X - totalWidth) / 2;

        string[] iconNames = { "Man", "Quest", "Location" };

        for (int i = 0; i < 3; i++)
        {
            string iconName = iconNames[i];
            string normalPath = $"res://assets/ui/{iconName}-Ringed.png";
            string activePath = normalPath.Replace(".png", "-Active.png");

            var iconButton = new IconButton();
            iconButton.Initialize(normalPath, activePath, iconName);
            iconButton.Size = new Vector2(BUTTON_SIZE, BUTTON_SIZE);
            iconButton.Position = new Vector2(startX + (i * (BUTTON_SIZE + BUTTON_SPACING)), buttonY);
            iconButton.Name = $"IconButton{i}";

            // Connect the icon selected signal
            iconButton.IconSelected += OnIconSelected;

            _topRectangle.AddChild(iconButton);
        }
    }

    private void OnIconSelected(string iconName)
    {
        // Unselect other buttons
        foreach (var child in _topRectangle.GetChildren())
        {
            if (child is IconButton button && button.ButtonPressed)
            {
                if (button.Name != $"IconButton{Array.IndexOf(new[] { "Man", "Quest", "Location" }, iconName)}")
                {
                    button.ButtonPressed = false;
                }
            }
        }

        ChangeDisplayedIcon(iconName);
    }

    private void ChangeDisplayedIcon(string iconName)
    {
        GD.Print($"Changing displayed icon to: {iconName}");

        string iconPath = $"res://assets/ui/{iconName}.png";
        var newTexture = GD.Load<Texture2D>(iconPath);

        if (newTexture != null)
        {
            // Make sure bottom rectangle is visible
            _bottomRectangle.Visible = true;

            // Display icon
            _iconDisplay.Texture = newTexture;

            // Remove old profile labels
            for (int i = 0; i < _profileLabels.Length; i++)
            {
                if (_profileLabels[i] != null)
                {
                    _profileLabels[i].QueueFree();
                    _profileLabels[i] = null;
                }
            }

            // Set text based on selected icon
            switch (iconName)
            {
                case "Man":
                    _iconTextLabel.Text = "Profile";
                    CreateProfileInfo(new[] {
                        "Nama  :  Satryo Haryo Bimo",
                        "NIM     : 231524062",
                        "Kelas  : 2B",
                        "Prodi  : D4 - Teknik Informatika"
                    });
                    break;
                case "Quest":
                    _iconTextLabel.Text = "Informasi Proyek";
                    CreateProfileInfo(new[] {
                        "Tema : Suku Minahasa",
                        "Motif :",
                        " - Kain Bentenan",
                        " - Alat Musik Kolintang"
                    });
                    break;
                case "Location":
                    _iconTextLabel.Text = "Contact";
                    CreateProfileInfo(new[] {
                        "Email         : harioobmo@gmail.com",
                        "Github       : rioBMO",
                        "Instagram : ryobmo_"
                    });
                    break;
                default:
                    _iconTextLabel.Text = "";
                    break;
            }
        }
        else
        {
            GD.Print($"Couldn't load icon: {iconPath}");
        }
    }

    private void CreateProfileInfo(string[] infoLines)
    {
        GetTree().CreateTimer(0.05).Timeout += () =>
        {
            for (int i = 0; i < infoLines.Length; i++)
            {
                _profileLabels[i] = new Label
                {
                    Text = infoLines[i],
                    Position = new Vector2(
                        _iconTextLabel.Position.X,
                        _iconTextLabel.Position.Y + _iconTextLabel.Size.Y + 24 + (i * (24 + 4))
                    ),
                    Size = new Vector2(_bottomRectangle.Size.X - _iconTextLabel.Position.X - 36, 24)
                };

                if (_iconFont != null)
                {
                    _profileLabels[i].AddThemeFontOverride("font", _iconFont);
                    _profileLabels[i].AddThemeFontSizeOverride("font_size", 24);
                }

                _profileLabels[i].AddThemeColorOverride("font_color", new Color("#4D4D4D"));
                _bottomRectangle.AddChild(_profileLabels[i]);
            }
        };
    }

    public bool Toggle()
    {
        _isVisible = !_isVisible;

        if (_isVisible)
        {
            AudioManager.Instance.PlayObjectAppearSound(this);
            ShowProfilePanel();
        }
        else
        {
            AudioManager.Instance.PlayObjectAppearSound(this);
            HideProfilePanel();
        }

        return _isVisible;
    }

    private void ShowProfilePanel()
    {
        // Reset panels to original positions
        _topRectangle.Position = _originalTopPosition;
        _bottomRectangle.Position = _originalBottomPosition;

        // Show top rectangle
        _topRectangle.Visible = true;
        // Hide bottom rectangle initially
        _bottomRectangle.Visible = false;

        // Prepare top rectangle for animation (height 0)
        _topRectangle.Size = new Vector2(_topRectangle.Size.X, 0);

        var tween = CreateTween();

        // Animation 1: Top rectangle extends from top to bottom
        tween.TweenProperty(_topRectangle, "size:y", 595, ANIMATION_DURATION)
             .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);

        // Animation 2: Wait for top rectangle to finish
        tween.TweenInterval(0.2);

        // Animation 3: After top rectangle is done, position bottom rectangle
        tween.TweenCallback(Callable.From(() =>
        {
            _bottomRectangle.Visible = true;
            _bottomRectangle.Position = new Vector2(
                _originalBottomPosition.X,
                _originalTopPosition.Y + _topRectangle.Size.Y
            );
            _bottomRectangle.Size = new Vector2(_bottomRectangle.Size.X, 0);
        }));

        // Animation 4: Bottom rectangle extends downward
        tween.TweenProperty(_bottomRectangle, "size:y", 295, ANIMATION_DURATION - 0.1)
             .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
    }

    private void HideProfilePanel()
    {
        // Hide text and info labels first
        _iconTextLabel.Visible = false;

        for (int i = 0; i < _profileLabels.Length; i++)
        {
            if (_profileLabels[i] != null)
            {
                _profileLabels[i].Visible = false;
            }
        }

        // Hide all children in top rectangle except title and horizontal line
        foreach (var child in _topRectangle.GetChildren())
        {
            if (child is CanvasItem canvasItem)
            {
                if (!(child is Label label && label.Text == "rioBMO") &&
                    !(child is ColorRect rect && rect.Size.Y == 2))
                {
                    canvasItem.Visible = false;
                }
            }
        }

        var tween = CreateTween();

        // Animation 1: Bottom rectangle shrinks
        tween.TweenProperty(_bottomRectangle, "size:y", 0, 0.3)
             .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

        // Animation 2: Hide bottom rectangle
        tween.TweenCallback(Callable.From(() => _bottomRectangle.Visible = false));

        // Animation 3: Wait briefly
        tween.TweenInterval(0.1);

        // Animation 4: Top rectangle shrinks
        tween.TweenProperty(_topRectangle, "size:y", 0, 0.4)
             .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

        // Animation 5: Hide top rectangle and reset properties
        tween.TweenCallback(Callable.From(() =>
        {
            _topRectangle.Visible = false;

            // Reset original sizes
            _topRectangle.Size = new Vector2(510, 595);
            _bottomRectangle.Size = new Vector2(510, 295);

            // Reset visibility of all children
            foreach (var child in _topRectangle.GetChildren())
            {
                if (child is CanvasItem canvasItem)
                {
                    canvasItem.Visible = true;
                }
            }

            _iconTextLabel.Visible = true;

            for (int i = 0; i < _profileLabels.Length; i++)
            {
                if (_profileLabels[i] != null)
                {
                    _profileLabels[i].Visible = true;
                }
            }
        }));
    }
}
