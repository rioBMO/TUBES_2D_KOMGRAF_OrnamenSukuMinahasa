using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;
using KG2025.Components;

namespace KG2025.Components.AnimatedMotifs
{
    public class ColoredKolintangMotif
    {
        private Node2D parent;
        private KartesiusSystem kartesiusSystem;
        private ColorPalette colorPalette;
        private ColorThemeManager colorThemeManager;
        private bool usingThemeManager = false;

        // Colors for the kolintang with specified hex values
        private Color baseColor = new Color("#513822"); // Dark brown for kolintang base
        private Color baseOutlineColor = new Color("#513822"); // Dark brown outline
        private Color barColor = new Color("#D19453"); // Medium brown for wooden bars
        private Color handleColor = new Color("#513822"); // Dark brown for mallet handles
        private Color headColor = new Color("#8F7300"); // Gold/bronze for mallet heads
        private Color holeColor = new Color("#513822"); // Dark brown for holes on bars

        // Animation properties
        private List<float> barAnimations = new List<float>();
        private List<bool> barIsAnimating = new List<bool>();
        private List<float> malletPositions = new List<float>();
        private List<float> malletAnimations = new List<float>();
        private List<int> targetBarIndices = new List<int>();

        // Animation parameters
        private float animationSpeed = 8.0f;
        private float maxBarDisplacement = 4.0f;
        private float horizontalVibrationAmount = 2.5f;
        private float vibrationFrequency = 10.0f;
        private float malletSpeed = 1.2f;

        // Sequential animation parameters
        private bool isSequentialAnimation = true;
        private int leftMalletCurrentBar = 0;
        private int rightMalletCurrentBar;
        private bool isLeftMalletMovingRight = true;
        private bool isRightMalletMovingLeft = true;

        // Track the last position of each mallet for direct movement
        private Vector2[] lastMalletPositions = new Vector2[2];

        // Dimensions for kolintang
        private float leftWidth;
        private float rightWidth;
        private float totalLength;
        private float barWidth = 52f;
        private float cornerRadius = 10f;
        private float leftMargin = 32f;
        private float rightMargin = 60f;
        private float barExtension = 50f;
        private int numberOfBars = 12;

        // Interactive mode properties
        private bool interactiveMode = false;
        private List<Rect2> barHitBoxes = new List<Rect2>();
        private int selectedBarIndex = -1;
        private bool isAnimatingMallet = false;
        private float malletAnimationProgress = 0f;
        private float malletAnimationSpeed = 2.5f; // Faster for better response

        // Positioning for interactive mode
        private float malletRestY;
        private float barBottom;

        // Store bar center positions for hitting
        private List<Vector2> barCenters = new List<Vector2>();

        // Auto-play mode properties
        private bool autoPlayMode = false;
        private float autoPlayTimer = 0f;
        private float autoPlayInterval = 0.5f; // Time between hits
        private int currentAutoPlayBarIndex = 0;
        private bool autoPlayForward = true;  // Direction of auto-play

        public ColoredKolintangMotif(Node2D parent, KartesiusSystem kartesiusSystem, float leftWidth, float rightWidth, float length)
        {
            this.parent = parent;
            this.kartesiusSystem = kartesiusSystem;
            this.leftWidth = leftWidth;
            this.rightWidth = rightWidth;
            this.totalLength = length;

            // Initialize animation state
            for (int i = 0; i < numberOfBars; i++)
            {
                barAnimations.Add(0f);
                barIsAnimating.Add(false);
                barHitBoxes.Add(new Rect2());
                barCenters.Add(new Vector2());
            }

            rightMalletCurrentBar = numberOfBars - 1; // Start at rightmost bar

            for (int i = 0; i < 2; i++)
            {
                malletPositions.Add(0f);
                malletAnimations.Add(0f);
                targetBarIndices.Add(-1);
            }
        }

        public void SetColorPalette(ColorPalette palette)
        {
            usingThemeManager = false;
            this.colorPalette = palette;

            // Keep the explicitly specified colors (don't use palette colors)
            // baseOutlineColor = new Color("#513822");
            // barColor = new Color("#D19453");
            // headColor = new Color("#8F7300");
            // handleColor = new Color("#513822");
            // holeColor = new Color("#513822");
        }

        // Add a new method to support ColorThemeManager directly
        public void SetColorPalette(ColorThemeManager themeManager)
        {
            usingThemeManager = true;
            this.colorThemeManager = themeManager;

            // Keep the explicitly specified colors (don't use palette colors)
            // baseOutlineColor = new Color("#513822");
            // barColor = new Color("#D19453");
            // headColor = new Color("#8F7300");
            // handleColor = new Color("#513822");
            // holeColor = new Color("#513822");
        }

        // Set interactive mode (true for user control, false for automatic animation)
        public void SetInteractiveMode(bool interactive)
        {
            interactiveMode = interactive;

            // Reset animations when switching modes
            if (interactive)
            {
                // Reset all animations
                for (int i = 0; i < numberOfBars; i++)
                {
                    barIsAnimating[i] = false;
                    barAnimations[i] = 0f;
                }

                malletAnimationProgress = 0f;
                isAnimatingMallet = false;
                selectedBarIndex = -1;
            }
        }

        // Toggle auto-play mode
        public void ToggleAutoPlayMode()
        {
            autoPlayMode = !autoPlayMode;

            if (autoPlayMode)
            {
                // Reset auto-play state when enabling
                currentAutoPlayBarIndex = 0;
                autoPlayTimer = 0f;
                autoPlayForward = true;
                isAnimatingMallet = false;
                selectedBarIndex = -1;
            }
        }

        // Handle mouse click to trigger bar animation
        public bool HandleMouseClick(Vector2 mousePosition)
        {
            if (!interactiveMode || isAnimatingMallet)
                return false;

            // Check if a bar was clicked
            for (int i = 0; i < barHitBoxes.Count; i++)
            {
                if (barHitBoxes[i].HasPoint(mousePosition))
                {
                    selectedBarIndex = i;
                    isAnimatingMallet = true;
                    malletAnimationProgress = 0f;
                    return true;
                }
            }

            return false;
        }

        public void Update(float delta)
        {
            if (!interactiveMode && !autoPlayMode)
            {
                // Original automatic animation
                // Update bar animations
                for (int i = 0; i < numberOfBars; i++)
                {
                    if (barIsAnimating[i])
                    {
                        barAnimations[i] += delta * animationSpeed;

                        // Reset animation after completion
                        if (barAnimations[i] >= Mathf.Pi)
                        {
                            barAnimations[i] = 0;
                            barIsAnimating[i] = false;
                        }
                    }
                }

                // Update mallet animations
                for (int i = 0; i < malletAnimations.Count; i++)
                {
                    if (malletAnimations[i] > 0)
                    {
                        malletAnimations[i] += delta * malletSpeed;

                        // When mallet reaches the bar (hits it)
                        if (malletAnimations[i] >= 0.5f && malletAnimations[i] - delta * malletSpeed < 0.5f)
                        {
                            // Start animation for the target bar
                            if (targetBarIndices[i] >= 0 && targetBarIndices[i] < numberOfBars)
                            {
                                barIsAnimating[targetBarIndices[i]] = true;
                                barAnimations[targetBarIndices[i]] = 0;
                            }
                        }

                        // When animation completes, set up the next target in sequence
                        if (malletAnimations[i] >= 1.0f)
                        {
                            // Reset animation but keep the mallet in motion
                            malletAnimations[i] = 0.01f;

                            if (isSequentialAnimation)
                            {
                                // Set up the next target in sequence
                                MoveToNextBarInSequence(i);
                            }
                            else
                            {
                                // Keep the random target selection for non-sequential mode
                                targetBarIndices[i] = new Random().Next(0, numberOfBars);
                            }
                        }
                    }
                }
            }
            else if (autoPlayMode)
            {
                // Auto-play mode - sequential playing
                autoPlayTimer += delta;

                if (autoPlayTimer >= autoPlayInterval && !isAnimatingMallet)
                {
                    // Time to hit the next bar
                    autoPlayTimer = 0f;
                    selectedBarIndex = currentAutoPlayBarIndex;
                    isAnimatingMallet = true;
                    malletAnimationProgress = 0f;

                    // Move to the next bar in sequence
                    if (autoPlayForward)
                    {
                        currentAutoPlayBarIndex++;
                        if (currentAutoPlayBarIndex >= numberOfBars)
                        {
                            autoPlayForward = false;
                            currentAutoPlayBarIndex = numberOfBars - 2;
                        }
                    }
                    else
                    {
                        currentAutoPlayBarIndex--;
                        if (currentAutoPlayBarIndex < 0)
                        {
                            autoPlayForward = true;
                            currentAutoPlayBarIndex = 1;
                        }
                    }
                }

                // Interactive animation for mallet and bars
                if (isAnimatingMallet && selectedBarIndex >= 0)
                {
                    malletAnimationProgress += delta * malletAnimationSpeed;

                    // When mallet reaches the bar (hits it)
                    if (malletAnimationProgress >= 0.5f && malletAnimationProgress - delta * malletAnimationSpeed < 0.5f)
                    {
                        // Start animation for the selected bar
                        barIsAnimating[selectedBarIndex] = true;
                        barAnimations[selectedBarIndex] = 0;
                    }

                    // Reset animation after completion
                    if (malletAnimationProgress >= 1.0f)
                    {
                        malletAnimationProgress = 0f;
                        isAnimatingMallet = false;
                    }
                }

                // Update bar animations (same as interactive mode)
                for (int i = 0; i < numberOfBars; i++)
                {
                    if (barIsAnimating[i])
                    {
                        barAnimations[i] += delta * animationSpeed;

                        // Reset animation after completion
                        if (barAnimations[i] >= Mathf.Pi)
                        {
                            barAnimations[i] = 0;
                            barIsAnimating[i] = false;
                        }
                    }
                }
            }
            else
            {
                // Interactive mode animation
                if (isAnimatingMallet && selectedBarIndex >= 0)
                {
                    malletAnimationProgress += delta * malletAnimationSpeed;

                    // When mallet reaches the bar (hits it)
                    if (malletAnimationProgress >= 0.5f && malletAnimationProgress - delta * malletAnimationSpeed < 0.5f)
                    {
                        // Start animation for the selected bar
                        barIsAnimating[selectedBarIndex] = true;
                        barAnimations[selectedBarIndex] = 0;
                    }

                    // Reset animation after completion
                    if (malletAnimationProgress >= 1.0f)
                    {
                        malletAnimationProgress = 0f;
                        isAnimatingMallet = false;
                    }
                }

                // Update bar animations (same as automatic mode)
                for (int i = 0; i < numberOfBars; i++)
                {
                    if (barIsAnimating[i])
                    {
                        barAnimations[i] += delta * animationSpeed;

                        // Reset animation after completion
                        if (barAnimations[i] >= Mathf.Pi)
                        {
                            barAnimations[i] = 0;
                            barIsAnimating[i] = false;
                        }
                    }
                }
            }
        }

        public void Draw(float centerX, float centerY)
        {
            // Draw the trapezoid base
            DrawKolintangBase(centerX, centerY);

            // Calculate spacing between bars
            float leftX = centerX - totalLength / 2;
            float rightX = centerX + totalLength / 2;
            float totalAvailableLength = totalLength - leftMargin - rightMargin;
            float totalBarsWidth = barWidth * numberOfBars;
            float spacing = (totalAvailableLength - totalBarsWidth) / (numberOfBars - 1);
            float startX = leftX + leftMargin;

            // Save the Y position of the bottom of the bars for mallet positioning
            barBottom = 0;

            // Draw each wooden bar with animation
            for (int i = 0; i < numberOfBars; i++)
            {
                float barX = startX + (barWidth + spacing) * i;

                // Calculate base height based on position along trapezoid
                float progress = (barX - leftX) / (rightX - leftX);
                float baseHeight = leftWidth + progress * (rightWidth - leftWidth);
                float barHeight = baseHeight + barExtension * 2;

                // Apply vibration effect
                float verticalDisplacement = 0;
                float horizontalDisplacement = 0;
                if (barIsAnimating[i])
                {
                    float dampingFactor = Mathf.Sin(barAnimations[i]);
                    float vibration = Mathf.Sin(barAnimations[i] * vibrationFrequency);

                    verticalDisplacement = vibration * maxBarDisplacement * dampingFactor;
                    horizontalDisplacement = Mathf.Cos(barAnimations[i] * vibrationFrequency) *
                                           horizontalVibrationAmount * dampingFactor;
                }

                // Center the bar vertically with vibration displacement
                float barTop = centerY - barHeight / 2 + verticalDisplacement;
                float adjustedBarX = barX + horizontalDisplacement;

                // Save the bottom Y position for mallet positioning
                float currentBarBottom = barTop + barHeight;
                if (currentBarBottom > barBottom) barBottom = currentBarBottom;

                // Store the bar's center position for targeting
                barCenters[i] = new Vector2(
                    adjustedBarX + barWidth / 2,
                    barTop + barHeight / 2 // Center of the bar
                );

                // Store the bar's hit box for interaction
                barHitBoxes[i] = new Rect2(adjustedBarX, barTop, barWidth, barHeight);

                // Draw the bar using Godot's built-in functions
                DrawWoodenBar(adjustedBarX, barTop, barWidth, barHeight, baseHeight, centerY, verticalDisplacement, i);

                // Initialize mallet rest position if not already done
                if (i == 0 && malletPositions[0] == 0)
                {
                    // In interactive mode, we place mallets at the bottom
                    malletPositions[0] = barX - spacing;
                    malletPositions[1] = barX + (barWidth + spacing) * (numberOfBars - 1) + spacing;

                    // Start animations if in automatic mode
                    if (!interactiveMode)
                    {
                        StartNewMalletAnimation(0);
                        StartNewMalletAnimation(1);
                    }
                }
            }

            // Calculate mallet rest position below the bars
            malletRestY = barBottom + 50;

            // Draw animated mallets
            if (interactiveMode)
            {
                DrawInteractiveMallet(leftX, rightX, centerY);
            }
            else
            {
                DrawAnimatedMallets(leftX, rightX, centerY);
            }
        }

        private void DrawKolintangBase(float centerX, float centerY)
        {
            // Calculate trapezoid points for horizontal orientation
            Vector2[] points = new Vector2[4];

            // Left side of trapezoid 
            float leftTopY = centerY - leftWidth / 2;
            float leftBottomY = centerY + leftWidth / 2;
            float leftX = centerX - totalLength / 2;

            // Right side of trapezoid
            float rightTopY = centerY - rightWidth / 2;
            float rightBottomY = centerY + rightWidth / 2;
            float rightX = centerX + totalLength / 2;

            // Define trapezoid points in clockwise order
            points[0] = new Vector2(leftX, leftTopY);
            points[1] = new Vector2(rightX, rightTopY);
            points[2] = new Vector2(rightX, rightBottomY);
            points[3] = new Vector2(leftX, leftBottomY);

            // Draw trapezoid base using Godot's polygon function with specified base color
            parent.DrawPolygon(points, new Color[] { baseColor }); // Use baseColor instead of colorPalette.LightBrown
            parent.DrawPolyline(points, baseOutlineColor, 2.0f, true);
        }

        private void DrawWoodenBar(float x, float y, float width, float height, float baseHeight, float centerY, float verticalDisplacement, int barIndex)
        {
            // Calculate color variation based on bar index for visual interest
            float hueShift = (float)barIndex / numberOfBars * 0.2f;
            Color currentBarColor = barColor.Lightened(hueShift);

            // Draw rounded rectangle for bar
            Vector2[] rect = new Vector2[4];
            rect[0] = new Vector2(x, y);
            rect[1] = new Vector2(x + width, y);
            rect[2] = new Vector2(x + width, y + height);
            rect[3] = new Vector2(x, y + height);

            // Fill bar with solid color
            parent.DrawPolygon(rect, new Color[] { currentBarColor });

            // Draw border
            parent.DrawPolyline(rect, baseOutlineColor, 2.0f, true);

            // Draw rounded corners
            DrawRoundedCorners(x, y, width, height, cornerRadius);

            // Calculate where this bar intersects with the base
            float topIntersection = centerY - baseHeight / 2;
            float bottomIntersection = centerY + baseHeight / 2;

            // Draw holes on the bar
            float holeRadius = 4f;
            float holeOffset = 23f;
            float holeX = x + width / 2;

            // Draw top hole
            float topHoleY = topIntersection + holeOffset + verticalDisplacement;
            parent.DrawCircle(new Vector2(holeX, topHoleY), holeRadius, holeColor);
            parent.DrawArc(new Vector2(holeX, topHoleY), holeRadius, 0, Mathf.Tau, 12, baseOutlineColor, 1.5f);

            // Draw bottom hole
            float bottomHoleY = bottomIntersection - holeOffset + verticalDisplacement;
            parent.DrawCircle(new Vector2(holeX, bottomHoleY), holeRadius, holeColor);
            parent.DrawArc(new Vector2(holeX, bottomHoleY), holeRadius, 0, Mathf.Tau, 12, baseOutlineColor, 1.5f);
        }

        private void DrawRoundedCorners(float x, float y, float width, float height, float radius)
        {
            // Top left corner
            parent.DrawArc(new Vector2(x + radius, y + radius), radius, Mathf.Pi, Mathf.Pi * 1.5f, 8, baseOutlineColor, 2.0f);

            // Top right corner
            parent.DrawArc(new Vector2(x + width - radius, y + radius), radius, Mathf.Pi * 1.5f, Mathf.Pi * 2, 8, baseOutlineColor, 2.0f);

            // Bottom right corner
            parent.DrawArc(new Vector2(x + width - radius, y + height - radius), radius, 0, Mathf.Pi * 0.5f, 8, baseOutlineColor, 2.0f);

            // Bottom left corner
            parent.DrawArc(new Vector2(x + radius, y + height - radius), radius, Mathf.Pi * 0.5f, Mathf.Pi, 8, baseOutlineColor, 2.0f);
        }

        private void DrawAnimatedMallets(float leftX, float rightX, float centerY)
        {
            float totalAvailableLength = totalLength - leftMargin - rightMargin;
            float spacing = (totalAvailableLength - barWidth * numberOfBars) / (numberOfBars - 1);
            float startX = leftX + leftMargin;

            for (int i = 0; i < 2; i++)
            {
                if (malletAnimations[i] <= 0 || targetBarIndices[i] < 0)
                    continue;

                float targetBarX = startX + (barWidth + spacing) * targetBarIndices[i] + barWidth / 2;
                float targetY = centerY;
                float moveProgress = malletAnimations[i];

                float malletX, malletY;

                if (lastMalletPositions[i] == Vector2.Zero)
                {
                    if (i == 0) // Left mallet
                    {
                        lastMalletPositions[i] = new Vector2(startX + barWidth / 2, targetY);
                    }
                    else // Right mallet
                    {
                        lastMalletPositions[i] = new Vector2(
                            startX + (barWidth + spacing) * (numberOfBars - 1) + barWidth / 2,
                            targetY
                        );
                    }
                }

                malletX = Mathf.Lerp(lastMalletPositions[i].X, targetBarX, moveProgress);
                float verticalOffset = Mathf.Sin(moveProgress * Mathf.Pi) * 30;
                malletY = targetY - verticalOffset;

                if (moveProgress >= 0.98f)
                {
                    lastMalletPositions[i] = new Vector2(targetBarX, targetY);
                }

                // Draw the mallet with Godot's built-in functions
                DrawMallet(malletX, malletY);
            }
        }

        private void DrawInteractiveMallet(float leftX, float rightX, float centerY)
        {
            // Calculate shared variables at the beginning of the method
            float totalAvailableLength = totalLength - leftMargin - rightMargin;
            float spacing = (totalAvailableLength - barWidth * numberOfBars) / (numberOfBars - 1);
            float startX = leftX + leftMargin;
            float malletX;
            float malletY;

            // Start from center bottom - we'll use this as the rest position for the mallet
            float restX = startX + ((barWidth + spacing) * numberOfBars) / 2;

            if (selectedBarIndex < 0 || !isAnimatingMallet)
            {
                // Draw mallet at rest position
                malletX = restX;
                DrawMallet(malletX, malletRestY);
                return;
            }

            // When animating, calculate position to hit the selected bar
            // Get the target bar's center position
            Vector2 targetBarCenter = barCenters[selectedBarIndex];

            // Calculate mallet position based on animation progress
            float moveProgress = malletAnimationProgress;

            // Use Bezier curve to create a natural arc motion from rest position to target
            if (moveProgress < 0.5f)
            {
                // First half of animation - approach the bar
                float t = moveProgress * 2; // Normalize to 0-1 range

                // Quadratic Bezier curve
                Vector2 controlPoint = new Vector2(
                    (restX + targetBarCenter.X) / 2,
                    Mathf.Min(malletRestY, targetBarCenter.Y) - 70 // Control point above path
                );

                malletX = Mathf.Lerp(Mathf.Lerp(restX, controlPoint.X, t),
                                    Mathf.Lerp(controlPoint.X, targetBarCenter.X, t), t);
                malletY = Mathf.Lerp(Mathf.Lerp(malletRestY, controlPoint.Y, t),
                                    Mathf.Lerp(controlPoint.Y, targetBarCenter.Y, t), t);
            }
            else
            {
                // Second half of animation - return to rest position
                float t = (moveProgress - 0.5f) * 2; // Normalize to 0-1 range

                // Quadratic Bezier curve for return path
                Vector2 controlPoint = new Vector2(
                    (targetBarCenter.X + restX) / 2,
                    Mathf.Min(targetBarCenter.Y, malletRestY) - 40 // Control point above path
                );

                malletX = Mathf.Lerp(Mathf.Lerp(targetBarCenter.X, controlPoint.X, t),
                                    Mathf.Lerp(controlPoint.X, restX, t), t);
                malletY = Mathf.Lerp(Mathf.Lerp(targetBarCenter.Y, controlPoint.Y, t),
                                    Mathf.Lerp(controlPoint.Y, malletRestY, t), t);
            }

            // Draw the mallet
            DrawMallet(malletX, malletY);
        }

        private void StartNewMalletAnimation(int malletIndex)
        {
            malletAnimations[malletIndex] = 0.01f;

            if (isSequentialAnimation)
            {
                if (malletIndex == 0)
                {
                    targetBarIndices[malletIndex] = leftMalletCurrentBar;
                }
                else
                {
                    targetBarIndices[malletIndex] = rightMalletCurrentBar;
                }
            }
            else
            {
                targetBarIndices[malletIndex] = new Random().Next(0, numberOfBars);
            }
        }

        // Move to the next bar in the sequence
        private void MoveToNextBarInSequence(int malletIndex)
        {
            if (malletIndex == 0) // Left mallet
            {
                if (isLeftMalletMovingRight)
                {
                    leftMalletCurrentBar++;
                    if (leftMalletCurrentBar >= numberOfBars / 2)
                    {
                        isLeftMalletMovingRight = false;
                        leftMalletCurrentBar = numberOfBars / 2 - 1;
                    }
                }
                else
                {
                    leftMalletCurrentBar--;
                    if (leftMalletCurrentBar < 0)
                    {
                        isLeftMalletMovingRight = true;
                        leftMalletCurrentBar = 0;
                    }
                }

                targetBarIndices[malletIndex] = leftMalletCurrentBar;
            }
            else // Right mallet
            {
                if (isRightMalletMovingLeft)
                {
                    rightMalletCurrentBar--;
                    if (rightMalletCurrentBar < numberOfBars / 2)
                    {
                        isRightMalletMovingLeft = false;
                        rightMalletCurrentBar = numberOfBars / 2;
                    }
                }
                else
                {
                    rightMalletCurrentBar++;
                    if (rightMalletCurrentBar >= numberOfBars)
                    {
                        isRightMalletMovingLeft = true;
                        rightMalletCurrentBar = numberOfBars - 1;
                    }
                }

                targetBarIndices[malletIndex] = rightMalletCurrentBar;
            }
        }

        private void DrawMallet(float x, float y)
        {
            float headRadius = 68f / 2f;
            float handleWidth = 7f;
            float handleHeight = 120f;

            // Draw mallet head
            parent.DrawCircle(new Vector2(x, y), headRadius, headColor);
            parent.DrawArc(new Vector2(x, y), headRadius, 0, Mathf.Tau, 32, baseOutlineColor, 2.0f);

            // Draw handle
            Rect2 handleRect = new Rect2(x - handleWidth / 2, y + headRadius, handleWidth, handleHeight);
            parent.DrawRect(handleRect, handleColor);

            // Draw handle outline
            Vector2[] handleOutline = new Vector2[4];
            handleOutline[0] = new Vector2(x - handleWidth / 2, y + headRadius);
            handleOutline[1] = new Vector2(x + handleWidth / 2, y + headRadius);
            handleOutline[2] = new Vector2(x + handleWidth / 2, y + headRadius + handleHeight);
            handleOutline[3] = new Vector2(x - handleWidth / 2, y + headRadius + handleHeight);
            parent.DrawPolyline(handleOutline, baseOutlineColor, 2.0f, true);
        }
    }
}
