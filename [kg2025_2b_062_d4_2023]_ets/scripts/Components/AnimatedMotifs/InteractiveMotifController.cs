using Godot;
using System;
using KG2025.Utils;

namespace KG2025.Components.AnimatedMotifs
{
    public class InteractiveMotifController
    {
        // Animation properties
        private float orbitAngle = 0f;
        private float orbitSpeed = 0.3f; // Default speed

        // Breathing animation properties
        private float breathingFactor = 1.0f;
        private float breathingSpeed = 0.5f; // Default speed
        private float breathingTime = 0f;
        private float minScale = 0.7f;
        private float maxScale = 1.3f;
        private float diamondBreathingFactor = 1.0f;
        private float steppedDiamondBreathingFactor = 1.0f;

        // Trapezoid animation
        private float trapezoidYOffset = 0f;
        private float trapezoidMovementSpeed = 60.0f; // Default speed

        // Boundary constraints
        private float verticalLineOffset = 110f;
        private float diamondSize;
        private float stepSize;

        public InteractiveMotifController(float diamondSize, float stepSize)
        {
            this.diamondSize = diamondSize;
            this.stepSize = stepSize;
        }

        public void Update(float delta, InputManager inputManager)
        {
            // Calculate effective speeds
            float effectiveOrbitSpeed = orbitSpeed * inputManager.OrbitSpeedModifier;
            float effectiveBreathingSpeed = breathingSpeed * inputManager.BreathingSpeedModifier;
            float effectiveTrapezoidSpeed = trapezoidMovementSpeed * inputManager.TrapezoidSpeedModifier;

            // Update orbit angle based on direction
            if (inputManager.ReverseOrbitRotation)
            {
                orbitAngle -= delta * effectiveOrbitSpeed;
                // Keep angle within 0 to 2π range
                if (orbitAngle < 0)
                    orbitAngle += 2 * Mathf.Pi;
            }
            else
            {
                orbitAngle += delta * effectiveOrbitSpeed;
                // Keep angle within 0 to 2π range
                if (orbitAngle > 2 * Mathf.Pi)
                    orbitAngle -= 2 * Mathf.Pi;
            }

            // Update breathing animation
            breathingTime += delta * effectiveBreathingSpeed;
            if (breathingTime > Mathf.Pi * 2)
                breathingTime -= Mathf.Pi * 2;

            // Calculate breathing factors
            CalculateBreathingFactors();

            // Update trapezoid animation with direction control
            if (inputManager.ReverseTrapezoidMovement)
            {
                trapezoidYOffset -= delta * effectiveTrapezoidSpeed;
            }
            else
            {
                trapezoidYOffset += delta * effectiveTrapezoidSpeed;
            }
        }

        // Handle trapezoid wraparound with the given total height
        public float GetTrapezoidYOffset(float totalHeight, float centerY)
        {
            float adjustedOffset = trapezoidYOffset;

            // Wrap around when exceeding limits
            if (adjustedOffset > totalHeight)
            {
                adjustedOffset -= totalHeight;
                trapezoidYOffset = adjustedOffset; // Reset the stored offset
            }
            else if (adjustedOffset < 0)
            {
                adjustedOffset += totalHeight;
                trapezoidYOffset = adjustedOffset; // Reset the stored offset
            }

            return adjustedOffset;
        }

        // Calculate breathing factors with constraints
        private void CalculateBreathingFactors()
        {
            // Calculate raw diamond breathing (0 to 1 to 0)
            float diamondBreathing = (Mathf.Sin(breathingTime) + 1) / 2;

            // Calculate boundary constraint
            float boundaryWidth = verticalLineOffset - 10; // Buffer from boundary

            // Calculate maximum allowed scale for diamond
            float maxAllowedDiamondScale = boundaryWidth / diamondSize;
            float constrainedMaxScale = Mathf.Min(maxScale, maxAllowedDiamondScale);

            // Scale between minScale and constrainedMaxScale
            diamondBreathingFactor = minScale + diamondBreathing * (constrainedMaxScale - minScale);

            // Stepped diamond has inverse breathing
            float steppedDiamondBreathing = (Mathf.Sin(breathingTime + Mathf.Pi) + 1) / 2;

            // Maximum steps in the stepped diamond
            float maxSteppedDiamondWidth = 4 * stepSize;
            float maxAllowedSteppedScale = boundaryWidth / maxSteppedDiamondWidth;

            // Constrain stepped diamond scaling
            float constrainedSteppedMaxScale = Mathf.Min(maxScale, maxAllowedSteppedScale);

            // Calculate stepped diamond factor
            steppedDiamondBreathingFactor = minScale + steppedDiamondBreathing * (constrainedSteppedMaxScale - minScale);

            // Set main breathing factor 
            breathingFactor = diamondBreathingFactor;
        }

        // Get animation properties
        public float OrbitAngle => orbitAngle;
        public float BreathingFactor => breathingFactor;
        public float DiamondBreathingFactor => diamondBreathingFactor;
        public float SteppedDiamondBreathingFactor => steppedDiamondBreathingFactor;

        // Set animation parameters
        public void SetAnimationParams(float orbitSpeed, float breathingSpeed, float minScale, float maxScale)
        {
            this.orbitSpeed = orbitSpeed;
            this.breathingSpeed = breathingSpeed;
            this.minScale = minScale;
            this.maxScale = maxScale;
        }
    }
}
