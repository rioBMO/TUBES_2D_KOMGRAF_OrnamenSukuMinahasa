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

        private Color baseColor = new Color("#513822");
        private Color baseOutlineColor = new Color("#513822");
        private Color barColor = new Color("#D19453");
        private Color handleColor = new Color("#513822");
        private Color headColor = new Color("#8F7300");
        private Color holeColor = new Color("#513822");

        private List<float> barAnimations = new List<float>();
        private List<bool> barIsAnimating = new List<bool>();
        private List<float> malletPositions = new List<float>();
        private List<float> malletAnimations = new List<float>();
        private List<int> targetBarIndices = new List<int>();

        private float animationSpeed = 8.0f;
        private float maxBarDisplacement = 4.0f;
        private float horizontalVibrationAmount = 2.5f;
        private float vibrationFrequency = 10.0f;
        private float malletSpeed = 1.2f;

        private bool isSequentialAnimation = true;
        private int leftMalletCurrentBar = 0;
        private int rightMalletCurrentBar;
        private bool isLeftMalletMovingRight = true;
        private bool isRightMalletMovingLeft = true;

        private Vector2[] lastMalletPositions = new Vector2[2];

        private float leftWidth;
        private float rightWidth;
        private float totalLength;
        private float barWidth = 52f;
        private float cornerRadius = 10f;
        private float leftMargin = 32f;
        private float rightMargin = 60f;
        private float barExtension = 50f;
        private int numberOfBars = 12;

        private bool interactiveMode = false;
        private List<Rect2> barHitBoxes = new List<Rect2>();
        private int selectedBarIndex = -1;
        private bool isAnimatingMallet = false;
        private float malletAnimationProgress = 0f;
        private float malletAnimationSpeed = 2.5f;

        private float malletRestY;
        private float barBottom;

        private List<Vector2> barCenters = new List<Vector2>();

        private bool autoPlayMode = false;
        private float autoPlayTimer = 0f;
        private float autoPlayInterval = 0.5f;
        private int currentAutoPlayBarIndex = 0;
        private bool autoPlayForward = true;

        public ColoredKolintangMotif(Node2D parent, KartesiusSystem kartesiusSystem, float leftWidth, float rightWidth, float length)
        {
            this.parent = parent;
            this.kartesiusSystem = kartesiusSystem;
            this.leftWidth = leftWidth;
            this.rightWidth = rightWidth;
            this.totalLength = length;

            for (int i = 0; i < numberOfBars; i++)
            {
                barAnimations.Add(0f);
                barIsAnimating.Add(false);
                barHitBoxes.Add(new Rect2());
                barCenters.Add(new Vector2());
            }

            rightMalletCurrentBar = numberOfBars - 1;

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
        }

        public void SetColorPalette(ColorThemeManager themeManager)
        {
            usingThemeManager = true;
            this.colorThemeManager = themeManager;
        }

        public void SetInteractiveMode(bool interactive)
        {
            interactiveMode = interactive;

            if (interactive)
            {
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

        public void ToggleAutoPlayMode()
        {
            autoPlayMode = !autoPlayMode;

            if (autoPlayMode)
            {
                currentAutoPlayBarIndex = 0;
                autoPlayTimer = 0f;
                autoPlayForward = true;
                isAnimatingMallet = false;
                selectedBarIndex = -1;
            }
        }

        public bool HandleMouseClick(Vector2 mousePosition)
        {
            if (!interactiveMode || isAnimatingMallet)
                return false;

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
                for (int i = 0; i < numberOfBars; i++)
                {
                    if (barIsAnimating[i])
                    {
                        barAnimations[i] += delta * animationSpeed;

                        if (barAnimations[i] >= Mathf.Pi)
                        {
                            barAnimations[i] = 0;
                            barIsAnimating[i] = false;
                        }
                    }
                }

                for (int i = 0; i < malletAnimations.Count; i++)
                {
                    if (malletAnimations[i] > 0)
                    {
                        malletAnimations[i] += delta * malletSpeed;

                        if (malletAnimations[i] >= 0.5f && malletAnimations[i] - delta * malletSpeed < 0.5f)
                        {
                            if (targetBarIndices[i] >= 0 && targetBarIndices[i] < numberOfBars)
                            {
                                barIsAnimating[targetBarIndices[i]] = true;
                                barAnimations[targetBarIndices[i]] = 0;
                            }
                        }

                        if (malletAnimations[i] >= 1.0f)
                        {
                            malletAnimations[i] = 0.01f;

                            if (isSequentialAnimation)
                            {
                                MoveToNextBarInSequence(i);
                            }
                            else
                            {
                                targetBarIndices[i] = new Random().Next(0, numberOfBars);
                            }
                        }
                    }
                }
            }
            else if (autoPlayMode)
            {
                autoPlayTimer += delta;

                if (autoPlayTimer >= autoPlayInterval && !isAnimatingMallet)
                {
                    autoPlayTimer = 0f;
                    selectedBarIndex = currentAutoPlayBarIndex;
                    isAnimatingMallet = true;
                    malletAnimationProgress = 0f;

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

                if (isAnimatingMallet && selectedBarIndex >= 0)
                {
                    malletAnimationProgress += delta * malletAnimationSpeed;

                    if (malletAnimationProgress >= 0.5f && malletAnimationProgress - delta * malletAnimationSpeed < 0.5f)
                    {
                        barIsAnimating[selectedBarIndex] = true;
                        barAnimations[selectedBarIndex] = 0;
                    }

                    if (malletAnimationProgress >= 1.0f)
                    {
                        malletAnimationProgress = 0f;
                        isAnimatingMallet = false;
                    }
                }

                for (int i = 0; i < numberOfBars; i++)
                {
                    if (barIsAnimating[i])
                    {
                        barAnimations[i] += delta * animationSpeed;

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
                if (isAnimatingMallet && selectedBarIndex >= 0)
                {
                    malletAnimationProgress += delta * malletAnimationSpeed;

                    if (malletAnimationProgress >= 0.5f && malletAnimationProgress - delta * malletAnimationSpeed < 0.5f)
                    {
                        barIsAnimating[selectedBarIndex] = true;
                        barAnimations[selectedBarIndex] = 0;
                    }

                    if (malletAnimationProgress >= 1.0f)
                    {
                        malletAnimationProgress = 0f;
                        isAnimatingMallet = false;
                    }
                }

                for (int i = 0; i < numberOfBars; i++)
                {
                    if (barIsAnimating[i])
                    {
                        barAnimations[i] += delta * animationSpeed;

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
            DrawKolintangBase(centerX, centerY);

            float leftX = centerX - totalLength / 2;
            float rightX = centerX + totalLength / 2;
            float totalAvailableLength = totalLength - leftMargin - rightMargin;
            float totalBarsWidth = barWidth * numberOfBars;
            float spacing = (totalAvailableLength - totalBarsWidth) / (numberOfBars - 1);
            float startX = leftX + leftMargin;

            barBottom = 0;

            for (int i = 0; i < numberOfBars; i++)
            {
                float barX = startX + (barWidth + spacing) * i;

                float progress = (barX - leftX) / (rightX - leftX);
                float baseHeight = leftWidth + progress * (rightWidth - leftWidth);
                float barHeight = baseHeight + barExtension * 2;

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

                float barTop = centerY - barHeight / 2 + verticalDisplacement;
                float adjustedBarX = barX + horizontalDisplacement;

                float currentBarBottom = barTop + barHeight;
                if (currentBarBottom > barBottom) barBottom = currentBarBottom;

                barCenters[i] = new Vector2(
                    adjustedBarX + barWidth / 2,
                    barTop + barHeight / 2
                );

                barHitBoxes[i] = new Rect2(adjustedBarX, barTop, barWidth, barHeight);

                DrawWoodenBar(adjustedBarX, barTop, barWidth, barHeight, baseHeight, centerY, verticalDisplacement, i);

                if (i == 0 && malletPositions[0] == 0)
                {
                    malletPositions[0] = barX - spacing;
                    malletPositions[1] = barX + (barWidth + spacing) * (numberOfBars - 1) + spacing;

                    if (!interactiveMode)
                    {
                        StartNewMalletAnimation(0);
                        StartNewMalletAnimation(1);
                    }
                }
            }

            malletRestY = barBottom + 50;

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
            Vector2[] points = new Vector2[4];

            float leftTopY = centerY - leftWidth / 2;
            float leftBottomY = centerY + leftWidth / 2;
            float leftX = centerX - totalLength / 2;

            float rightTopY = centerY - rightWidth / 2;
            float rightBottomY = centerY + rightWidth / 2;
            float rightX = centerX + totalLength / 2;

            points[0] = new Vector2(leftX, leftTopY);
            points[1] = new Vector2(rightX, rightTopY);
            points[2] = new Vector2(rightX, rightBottomY);
            points[3] = new Vector2(leftX, leftBottomY);

            parent.DrawPolygon(points, new Color[] { baseColor });
            parent.DrawPolyline(points, baseOutlineColor, 2.0f, true);
        }

        private void DrawWoodenBar(float x, float y, float width, float height, float baseHeight, float centerY, float verticalDisplacement, int barIndex)
        {
            float hueShift = (float)barIndex / numberOfBars * 0.2f;
            Color currentBarColor = barColor.Lightened(hueShift);

            Vector2[] rect = new Vector2[4];
            rect[0] = new Vector2(x, y);
            rect[1] = new Vector2(x + width, y);
            rect[2] = new Vector2(x + width, y + height);
            rect[3] = new Vector2(x, y + height);

            parent.DrawPolygon(rect, new Color[] { currentBarColor });

            parent.DrawPolyline(rect, baseOutlineColor, 2.0f, true);

            DrawRoundedCorners(x, y, width, height, cornerRadius);

            float topIntersection = centerY - baseHeight / 2;
            float bottomIntersection = centerY + baseHeight / 2;

            float holeRadius = 4f;
            float holeOffset = 23f;
            float holeX = x + width / 2;

            float topHoleY = topIntersection + holeOffset + verticalDisplacement;
            parent.DrawCircle(new Vector2(holeX, topHoleY), holeRadius, holeColor);
            parent.DrawArc(new Vector2(holeX, topHoleY), holeRadius, 0, Mathf.Tau, 12, baseOutlineColor, 1.5f);

            float bottomHoleY = bottomIntersection - holeOffset + verticalDisplacement;
            parent.DrawCircle(new Vector2(holeX, bottomHoleY), holeRadius, holeColor);
            parent.DrawArc(new Vector2(holeX, bottomHoleY), holeRadius, 0, Mathf.Tau, 12, baseOutlineColor, 1.5f);
        }

        private void DrawRoundedCorners(float x, float y, float width, float height, float radius)
        {
            parent.DrawArc(new Vector2(x + radius, y + radius), radius, Mathf.Pi, Mathf.Pi * 1.5f, 8, baseOutlineColor, 2.0f);

            parent.DrawArc(new Vector2(x + width - radius, y + radius), radius, Mathf.Pi * 1.5f, Mathf.Pi * 2, 8, baseOutlineColor, 2.0f);

            parent.DrawArc(new Vector2(x + width - radius, y + height - radius), radius, 0, Mathf.Pi * 0.5f, 8, baseOutlineColor, 2.0f);

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
                    if (i == 0)
                    {
                        lastMalletPositions[i] = new Vector2(startX + barWidth / 2, targetY);
                    }
                    else
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

                DrawMallet(malletX, malletY);
            }
        }

        private void DrawInteractiveMallet(float leftX, float rightX, float centerY)
        {
            float totalAvailableLength = totalLength - leftMargin - rightMargin;
            float spacing = (totalAvailableLength - barWidth * numberOfBars) / (numberOfBars - 1);
            float startX = leftX + leftMargin;
            float malletX;
            float malletY;

            float restX = startX + ((barWidth + spacing) * numberOfBars) / 2;

            if (selectedBarIndex < 0 || !isAnimatingMallet)
            {
                malletX = restX;
                DrawMallet(malletX, malletRestY);
                return;
            }

            Vector2 targetBarCenter = barCenters[selectedBarIndex];

            float moveProgress = malletAnimationProgress;

            if (moveProgress < 0.5f)
            {
                float t = moveProgress * 2;

                Vector2 controlPoint = new Vector2(
                    (restX + targetBarCenter.X) / 2,
                    Mathf.Min(malletRestY, targetBarCenter.Y) - 70
                );

                malletX = Mathf.Lerp(Mathf.Lerp(restX, controlPoint.X, t),
                                    Mathf.Lerp(controlPoint.X, targetBarCenter.X, t), t);
                malletY = Mathf.Lerp(Mathf.Lerp(malletRestY, controlPoint.Y, t),
                                    Mathf.Lerp(controlPoint.Y, targetBarCenter.Y, t), t);
            }
            else
            {
                float t = (moveProgress - 0.5f) * 2;

                Vector2 controlPoint = new Vector2(
                    (targetBarCenter.X + restX) / 2,
                    Mathf.Min(targetBarCenter.Y, malletRestY) - 40
                );

                malletX = Mathf.Lerp(Mathf.Lerp(targetBarCenter.X, controlPoint.X, t),
                                    Mathf.Lerp(controlPoint.X, restX, t), t);
                malletY = Mathf.Lerp(Mathf.Lerp(targetBarCenter.Y, controlPoint.Y, t),
                                    Mathf.Lerp(controlPoint.Y, malletRestY, t), t);
            }

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

        private void MoveToNextBarInSequence(int malletIndex)
        {
            if (malletIndex == 0)
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
            else
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

            parent.DrawCircle(new Vector2(x, y), headRadius, headColor);
            parent.DrawArc(new Vector2(x, y), headRadius, 0, Mathf.Tau, 32, baseOutlineColor, 2.0f);

            Rect2 handleRect = new Rect2(x - handleWidth / 2, y + headRadius, handleWidth, handleHeight);
            parent.DrawRect(handleRect, handleColor);

            Vector2[] handleOutline = new Vector2[4];
            handleOutline[0] = new Vector2(x - handleWidth / 2, y + headRadius);
            handleOutline[1] = new Vector2(x + handleWidth / 2, y + headRadius);
            handleOutline[2] = new Vector2(x + handleWidth / 2, y + headRadius + handleHeight);
            handleOutline[3] = new Vector2(x - handleWidth / 2, y + headRadius + handleHeight);
            parent.DrawPolyline(handleOutline, baseOutlineColor, 2.0f, true);
        }
    }
}
