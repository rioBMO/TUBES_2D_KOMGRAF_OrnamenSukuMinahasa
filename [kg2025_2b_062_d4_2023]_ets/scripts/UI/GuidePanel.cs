using Godot;
using System;

public partial class GuidePanel : Control
{
    // Constants
    private const float ANIMATION_DURATION = 0.4f;
    // Added constant for panel width
    private const float PANEL_WIDTH = 510f;

    // UI Elements
    private ColorRect _topRectangle;
    private ColorRect _middleRectangle;
    private ColorRect _bottomRectangle;
    private Label _guideLabel;
    private Label _instructionsLabel;
    private TextureButton _noButton;
    private FontFile _customFont;

    // State
    private bool _isVisible = false;
    private Vector2 _originalTopPosition;
    private Vector2 _originalMiddlePosition;
    private Vector2 _originalBottomPosition;

    public override void _Ready()
    {
        // Load font
        _customFont = GD.Load<FontFile>("res://assets/fonts/SAOUITT-Regular.ttf");
        if (_customFont == null)
        {
            _customFont = GD.Load<FontFile>("res://fonts/SAOUITT-Regular.ttf");
        }
    }

    public void Initialize(Vector2 position)
    {
        float screenWidth = GetViewportRect().Size.X;
        float rightColumnX = screenWidth - 50 - PANEL_WIDTH; // 50px from right edge

        // Create top rectangle
        _topRectangle = new ColorRect
        {
            Size = new Vector2(PANEL_WIDTH, 91),
            Color = new Color("#F9F9F9"),
            Position = new Vector2(rightColumnX, position.Y)
        };
        AddChild(_topRectangle);

        // Create middle rectangle
        _middleRectangle = new ColorRect
        {
            Size = new Vector2(PANEL_WIDTH, 450),
            Color = new Color("#D8D8D8"),
            Position = new Vector2(rightColumnX, position.Y + 91)
        };
        AddChild(_middleRectangle);

        // Create bottom rectangle
        _bottomRectangle = new ColorRect
        {
            Size = new Vector2(PANEL_WIDTH, 101),
            Color = new Color("#FFFFFF"),
            Position = new Vector2(rightColumnX, position.Y + 91 + 450)
        };
        AddChild(_bottomRectangle);

        // Store original positions
        _originalTopPosition = _topRectangle.Position;
        _originalMiddlePosition = _middleRectangle.Position;
        _originalBottomPosition = _bottomRectangle.Position;

        // Create guide title label
        _guideLabel = new Label
        {
            Text = "Guide",
            HorizontalAlignment = HorizontalAlignment.Center,
            ClipText = true,
            Position = new Vector2(0, (_topRectangle.Size.Y - 40) / 2),
            Size = new Vector2(_topRectangle.Size.X, 40),
            PivotOffset = new Vector2(255, 20),
            GrowHorizontal = Control.GrowDirection.Both,
            GrowVertical = Control.GrowDirection.Both
        };

        if (_customFont != null)
        {
            _guideLabel.AddThemeFontOverride("font", _customFont);
            _guideLabel.AddThemeFontSizeOverride("font_size", 37);
        }

        _guideLabel.AddThemeColorOverride("font_color", new Color("#4D4D4D"));
        _topRectangle.AddChild(_guideLabel);

        // Create instruction label
        _instructionsLabel = new Label
        {
            Text =
            @"Instructions for Karya 4:

            Controls:
            R - Reverse Orbit Rotation
            C - Swap Eye / Cross Colors
            + - Increase Animation Speed
            - - Decrease Animation Speed

            Kolintang Motif:
            - Click arrow button to switch to kolintang
            - Click on wooden bars to play notes
            - Press A to toggle auto-play mode

            Press H to hide instructions",

            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Position = new Vector2(20, 20),
            Size = new Vector2(_middleRectangle.Size.X - 40, _middleRectangle.Size.Y - 40)
        };

        if (_customFont != null)
        {
            _instructionsLabel.AddThemeFontOverride("font", _customFont);
            _instructionsLabel.AddThemeFontSizeOverride("font_size", 24);
        }

        _instructionsLabel.AddThemeColorOverride("font_color", new Color("#4D4D4D"));
        _middleRectangle.AddChild(_instructionsLabel);

        // Create No button
        _noButton = CreateNoButton();
        _bottomRectangle.AddChild(_noButton);

        // Hide panels initially
        _topRectangle.Visible = false;
        _middleRectangle.Visible = false;
        _bottomRectangle.Visible = false;
    }

    private TextureButton CreateNoButton()
    {
        var noButton = new TextureButton();

        var noTexture = GD.Load<Texture2D>("res://assets/ui/No.png");
        var noOnTexture = GD.Load<Texture2D>("res://assets/ui/No_on.png");

        if (noTexture != null && noOnTexture != null)
        {
            noButton.TextureNormal = noTexture;
            noButton.TextureHover = noOnTexture;
            noButton.TexturePressed = noOnTexture;
            noButton.TextureFocused = noOnTexture;

            noButton.IgnoreTextureSize = true;
            noButton.StretchMode = TextureButton.StretchModeEnum.KeepAspectCentered;

            noButton.CustomMinimumSize = new Vector2(42, 42);
            noButton.Size = new Vector2(42, 42);

            float centerX = (_bottomRectangle.Size.X - noButton.Size.X) / 2;
            float centerY = (_bottomRectangle.Size.Y - noButton.Size.Y) / 2;

            noButton.Position = new Vector2(centerX, centerY);

            noButton.Pressed += () =>
            {
                AudioManager.Instance.PlayButtonSound(this, "NoButton");
                GetTree().CreateTimer(0.34).Timeout += () => Toggle();
            };

            noButton.Visible = false;
        }
        else
        {
            GD.Print("Failed to load No button textures");
        }

        return noButton;
    }

    public bool Toggle()
    {
        _isVisible = !_isVisible;

        if (_isVisible)
        {
            AudioManager.Instance.PlayObjectAppearSound(this);
            ShowGuidePanel();
        }
        else
        {
            AudioManager.Instance.PlayObjectAppearSound(this);
            HideGuidePanel();
        }

        return _isVisible;
    }

    private void ShowGuidePanel()
    {
        // Reset panels to original positions for height only
        _topRectangle.Position = new Vector2(_originalTopPosition.X, _originalTopPosition.Y);

        // Important change: Position bottom rectangle right below top rectangle initially
        float adjacentBottomY = _originalTopPosition.Y + _topRectangle.Size.Y;
        _bottomRectangle.Position = new Vector2(_originalBottomPosition.X, adjacentBottomY);

        // Set initial sizes
        _topRectangle.Size = new Vector2(0, 91);
        _bottomRectangle.Size = new Vector2(0, 101);

        // Calculate center point for animation
        float centerX = _originalTopPosition.X + PANEL_WIDTH / 2;

        // Set positions for starting animation
        _topRectangle.Position = new Vector2(centerX, _originalTopPosition.Y);
        // Bottom rectangle stays attached to top rectangle
        _bottomRectangle.Position = new Vector2(centerX, adjacentBottomY);

        // Hide labels initially
        _guideLabel.Visible = false;
        _instructionsLabel.Visible = false;
        _noButton.Visible = false;

        // Show panels for animation
        _topRectangle.Visible = true;
        _bottomRectangle.Visible = true;

        var tween = CreateTween();

        // Animate both rectangles simultaneously
        // Top rectangle
        tween.Parallel().TweenProperty(_topRectangle, "size:x", PANEL_WIDTH, ANIMATION_DURATION + 0.1)
             .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
        tween.Parallel().TweenProperty(_topRectangle, "position:x", _originalTopPosition.X, ANIMATION_DURATION + 0.1)
             .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);

        // Bottom rectangle - now positioned right under the top rectangle
        tween.Parallel().TweenProperty(_bottomRectangle, "size:x", PANEL_WIDTH, ANIMATION_DURATION + 0.1)
             .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
        tween.Parallel().TweenProperty(_bottomRectangle, "position:x", _originalTopPosition.X, ANIMATION_DURATION + 0.1)
             .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);

        // After initial animation, show labels and animate middle rectangle
        tween.TweenCallback(Callable.From(() =>
        {
            _guideLabel.Visible = true;
            _noButton.Visible = true;

            // Prepare middle rectangle for animation
            _middleRectangle.Size = new Vector2(PANEL_WIDTH, 0);
            _middleRectangle.Position = new Vector2(
                _originalTopPosition.X,
                _originalTopPosition.Y + _topRectangle.Size.Y
            );
            _middleRectangle.Visible = true;

            // Show instructions label
            _instructionsLabel.Visible = true;

            var middleTween = CreateTween();

            // Animate middle rectangle expanding downward
            middleTween.TweenProperty(_middleRectangle, "size:y", 450, ANIMATION_DURATION - 0.1)
                      .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);

            // Move bottom rectangle down to its final position as middle rectangle expands
            middleTween.Parallel().TweenProperty(_bottomRectangle, "position:y",
                                            _originalBottomPosition.Y, ANIMATION_DURATION - 0.1)
                      .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        }));
    }

    private void HideGuidePanel()
    {
        // Hide all text and buttons first
        _guideLabel.Visible = false;
        _instructionsLabel.Visible = false;
        _noButton.Visible = false;

        // Calculate center point for animation
        float centerX = _originalBottomPosition.X + PANEL_WIDTH / 2;

        var tween = CreateTween();

        // Middle rectangle shrinks vertically first
        tween.TweenProperty(_middleRectangle, "size:y", 0, 0.3)
             .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

        // Move bottom rectangle up to be adjacent to top rectangle again
        tween.Parallel().TweenProperty(_bottomRectangle, "position:y",
                                  _originalTopPosition.Y + _topRectangle.Size.Y, 0.3)
             .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

        // Wait briefly
        tween.TweenInterval(0.1);

        // Both top and bottom rectangles shrink horizontally
        // Top rectangle
        tween.Parallel().TweenProperty(_topRectangle, "size:x", 0, ANIMATION_DURATION - 0.1)
             .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
        tween.Parallel().TweenProperty(_topRectangle, "position:x", centerX, ANIMATION_DURATION - 0.1)
             .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

        // Bottom rectangle
        tween.Parallel().TweenProperty(_bottomRectangle, "size:x", 0, ANIMATION_DURATION - 0.1)
             .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
        tween.Parallel().TweenProperty(_bottomRectangle, "position:x", centerX, ANIMATION_DURATION - 0.1)
             .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

        // Hide all panels and reset properties
        tween.TweenCallback(Callable.From(() =>
        {
            _topRectangle.Visible = false;
            _middleRectangle.Visible = false;
            _bottomRectangle.Visible = false;

            // Reset sizes and positions
            _topRectangle.Size = new Vector2(510, 91);
            _middleRectangle.Size = new Vector2(510, 450);
            _bottomRectangle.Size = new Vector2(510, 101);

            _topRectangle.Position = _originalTopPosition;
            _middleRectangle.Position = _originalMiddlePosition;
            _bottomRectangle.Position = _originalBottomPosition;
        }));
    }
}
