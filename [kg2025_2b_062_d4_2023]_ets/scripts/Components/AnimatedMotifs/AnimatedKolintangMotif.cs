using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;

namespace KG2025.Components.AnimatedMotifs
{
    public class AnimatedKolintangMotif
    {
        private Node2D parent;
        private KartesiusSystem kartesiusSystem;
        private Color outlineColor = Colors.White;

        // Animation properties
        private List<float> barAnimations = new List<float>(); // Track animation state for each bar
        private List<bool> barIsAnimating = new List<bool>(); // Track which bars are currently animating
        private List<float> malletPositions = new List<float>(); // X positions of mallets
        private List<float> malletAnimations = new List<float>(); // Mallet animation progress
        private List<int> targetBarIndices = new List<int>(); // Which bars are being targeted

        // Animation parameters
        private float animationSpeed = 8.0f; // Increased from 5.0f for even faster vibration
        private float maxBarDisplacement = 4.0f;
        private float horizontalVibrationAmount = 2.5f;
        private float vibrationFrequency = 10.0f; // Increased from 8.0f for faster vibration cycles
        private float malletSpeed = 1.2f; // Doubled from 0.6f for much faster mallet movement

        // Sequential animation parameters
        private bool isSequentialAnimation = true;
        private bool[] barSequence;
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

        public AnimatedKolintangMotif(Node2D parent, KartesiusSystem kartesiusSystem, float leftWidth, float rightWidth, float length)
        {
            this.parent = parent;
            this.kartesiusSystem = kartesiusSystem;
            this.leftWidth = leftWidth;
            this.rightWidth = rightWidth;
            this.totalLength = length;

            // Initialize animation state for each bar
            for (int i = 0; i < numberOfBars; i++)
            {
                barAnimations.Add(0f);
                barIsAnimating.Add(false);
            }

            // Initialize bar sequence tracking
            barSequence = new bool[numberOfBars];
            rightMalletCurrentBar = numberOfBars - 1; // Start at rightmost bar

            // Initialize mallets (two mallets)
            for (int i = 0; i < 2; i++)
            {
                malletPositions.Add(0f); // Will be set during draw
                malletAnimations.Add(0f);
                targetBarIndices.Add(-1); // No target initially
            }
        }

        public void SetColors(Color outlineColor)
        {
            this.outlineColor = outlineColor;
        }

        public void Update(float delta)
        {
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

        // Move to the next bar in the sequence
        private void MoveToNextBarInSequence(int malletIndex)
        {
            if (malletIndex == 0) // Left mallet
            {
                // Move in the current direction (left to right)
                if (isLeftMalletMovingRight)
                {
                    leftMalletCurrentBar++;
                    // If reached the end, change direction
                    if (leftMalletCurrentBar >= numberOfBars / 2)
                    {
                        isLeftMalletMovingRight = false;
                        leftMalletCurrentBar = numberOfBars / 2 - 1;
                    }
                }
                else // Moving left
                {
                    leftMalletCurrentBar--;
                    // If reached the start, change direction
                    if (leftMalletCurrentBar < 0)
                    {
                        isLeftMalletMovingRight = true;
                        leftMalletCurrentBar = 0;
                    }
                }

                // Set the target bar
                targetBarIndices[malletIndex] = leftMalletCurrentBar;
            }
            else // Right mallet
            {
                // Move in the current direction (right to left)
                if (isRightMalletMovingLeft)
                {
                    rightMalletCurrentBar--;
                    // If reached the middle, change direction
                    if (rightMalletCurrentBar < numberOfBars / 2)
                    {
                        isRightMalletMovingLeft = false;
                        rightMalletCurrentBar = numberOfBars / 2;
                    }
                }
                else // Moving right
                {
                    rightMalletCurrentBar++;
                    // If reached the end, change direction
                    if (rightMalletCurrentBar >= numberOfBars)
                    {
                        isRightMalletMovingLeft = true;
                        rightMalletCurrentBar = numberOfBars - 1;
                    }
                }

                // Set the target bar
                targetBarIndices[malletIndex] = rightMalletCurrentBar;
            }
        }

        private void StartNewMalletAnimation(int malletIndex)
        {
            // Start animation
            malletAnimations[malletIndex] = 0.01f; // Just above 0 to start the animation

            if (isSequentialAnimation)
            {
                // In sequential mode, set the initial target based on mallet position
                if (malletIndex == 0) // Left mallet
                {
                    targetBarIndices[malletIndex] = leftMalletCurrentBar;
                }
                else // Right mallet
                {
                    targetBarIndices[malletIndex] = rightMalletCurrentBar;
                }
            }
            else
            {
                // In random mode, select a random target
                targetBarIndices[malletIndex] = new Random().Next(0, numberOfBars);
            }
        }

        public void Draw(float centerX, float centerY)
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
            points[0] = new Vector2(leftX, leftTopY);      // Left top
            points[1] = new Vector2(rightX, rightTopY);    // Right top
            points[2] = new Vector2(rightX, rightBottomY); // Right bottom
            points[3] = new Vector2(leftX, leftBottomY);   // Left bottom

            // Use DrawingUtils to draw outline
            DrawingUtils.DrawPolygonOutline(parent, points, outlineColor);

            // Calculate spacing between bars
            float totalAvailableLength = totalLength - leftMargin - rightMargin;
            float totalBarsWidth = barWidth * numberOfBars;
            float spacing = (totalAvailableLength - totalBarsWidth) / (numberOfBars - 1);

            // Starting position for first bar
            float startX = leftX + leftMargin;

            // Draw each wooden bar with animation
            for (int i = 0; i < numberOfBars; i++)
            {
                float barX = startX + (barWidth + spacing) * i;

                // Calculate base height based on position along trapezoid
                float progress = (barX - leftX) / (rightX - leftX);
                float baseHeight = leftWidth + progress * (rightWidth - leftWidth);
                float barHeight = baseHeight + barExtension * 2;

                // Apply vibration effect - combine vertical and horizontal vibrations 
                // with higher frequency for more realistic vibration
                float verticalDisplacement = 0;
                float horizontalDisplacement = 0;
                if (barIsAnimating[i])
                {
                    // Main sine wave for damping
                    float dampingFactor = Mathf.Sin(barAnimations[i]);

                    // Higher frequency vibration effect
                    float vibration = Mathf.Sin(barAnimations[i] * vibrationFrequency);

                    // Combined vibration with damping (gets less intense over time)
                    verticalDisplacement = vibration * maxBarDisplacement * dampingFactor;
                    horizontalDisplacement = Mathf.Cos(barAnimations[i] * vibrationFrequency) *
                                           horizontalVibrationAmount * dampingFactor;
                }

                // Center the bar vertically with vibration displacement
                float barTop = centerY - barHeight / 2 + verticalDisplacement;
                // Adjust horizontal position with vibration
                float adjustedBarX = barX + horizontalDisplacement;

                // Draw the animated bar with vibration
                DrawRoundedRectangle(adjustedBarX, barTop, barWidth, barHeight, cornerRadius, outlineColor);

                // Calculate where this bar intersects with the base
                float topIntersection = centerY - baseHeight / 2;
                float bottomIntersection = centerY + baseHeight / 2;

                // Draw the two holes on each bar - also apply vibration effect to them
                float holeRadius = 4f;
                float holeOffset = 23f;
                float holeX = adjustedBarX + barWidth / 2; // Apply horizontal displacement to holes too

                // Top and bottom holes (with vibration offset)
                float topHoleY = topIntersection + holeOffset + verticalDisplacement;
                float bottomHoleY = bottomIntersection - holeOffset + verticalDisplacement;
                DrawCircleOutline(holeX, topHoleY, holeRadius, outlineColor);
                DrawCircleOutline(holeX, bottomHoleY, holeRadius, outlineColor);

                // Store bar positions for mallet targeting
                if (i == 0)
                {
                    // Initialize mallets if this is the first run
                    if (malletPositions[0] == 0)
                    {
                        malletPositions[0] = barX - spacing;
                        malletPositions[1] = barX + (barWidth + spacing) * (numberOfBars - 1) + spacing;

                        // Start animations
                        StartNewMalletAnimation(0);
                        StartNewMalletAnimation(1);
                    }
                }
            }

            // Draw mallets with animation
            DrawAnimatedMallets(leftX, rightX, centerY, leftWidth, rightWidth);
        }

        private void DrawAnimatedMallets(float leftX, float rightX, float centerY, float baseLeftWidth, float baseRightWidth)
        {
            float totalAvailableLength = totalLength - leftMargin - rightMargin;
            float spacing = (totalAvailableLength - barWidth * numberOfBars) / (numberOfBars - 1);
            float startX = leftX + leftMargin;

            for (int i = 0; i < 2; i++)
            {
                // Skip if no animation is happening
                if (malletAnimations[i] <= 0 || targetBarIndices[i] < 0)
                    continue;

                // Target the center of the bar precisely
                float targetBarX = startX + (barWidth + spacing) * targetBarIndices[i] + barWidth / 2;
                float targetY = centerY; // Target the vertical center of the kolintang for hitting

                // Get the current animation progress
                float moveProgress = malletAnimations[i];

                // Mallet head parameters
                float headRadius = 68f / 2f;

                // Position mallet for direct movement between bars
                float malletX, malletY;

                // If this is the first animation or we don't have a last position yet
                if (lastMalletPositions[i] == Vector2.Zero)
                {
                    // Set initial position for first animation
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

                // Direct linear movement from last hit position to next hit position
                malletX = Mathf.Lerp(lastMalletPositions[i].X, targetBarX, moveProgress);

                // Small vertical arc to simulate hitting motion (just enough to see the hit)
                float verticalOffset = Mathf.Sin(moveProgress * Mathf.Pi) * 30; // Small arc of 30px height
                malletY = targetY - verticalOffset;

                // When hit is complete, remember this position for next movement
                if (moveProgress >= 0.98f)
                {
                    lastMalletPositions[i] = new Vector2(targetBarX, targetY);
                }

                // Draw the mallet
                DrawMallet(malletX, malletY);
            }
        }

        // Draw a mallet (pemukul) for the kolintang
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

        // Draw outline of a rounded rectangle
        private void DrawRoundedRectangle(float x, float y, float width, float height, float radius, Color outlineColor)
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

        // Draw only the outline of a circle
        private void DrawCircleOutline(float centerX, float centerY, float radius, Color outlineColor)
        {
            List<Vector2> circlePoints = DrawingUtils.Lingkaran(centerX, centerY, radius);
            DrawingUtils.PutPixelAll(parent, circlePoints, outlineColor);
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
