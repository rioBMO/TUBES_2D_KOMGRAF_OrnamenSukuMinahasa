using Godot;
using System;
using System.Collections.Generic;

namespace KG2025.Utils
{
    public class InputManager
    {
        private Dictionary<Key, bool> previousKeyStates = new Dictionary<Key, bool>();

        // Speed modifiers
        public float OrbitSpeedModifier { get; private set; } = 1.0f;
        public float BreathingSpeedModifier { get; private set; } = 1.0f;
        public float TrapezoidSpeedModifier { get; private set; } = 1.0f;

        // Direction controls
        public bool ReverseOrbitRotation { get; private set; } = false;
        public bool ReverseTrapezoidMovement { get; private set; } = false;

        // Color controls
        public bool SwapEyeCrossColors { get; private set; } = false;

        // UI controls
        public bool ShowHelp { get; private set; } = true;

        public InputManager()
        {
            // Initialize the previous key states dictionary
            previousKeyStates[Key.R] = false;
            previousKeyStates[Key.C] = false;
            previousKeyStates[Key.Equal] = false; // + key
            previousKeyStates[Key.Minus] = false; // - key
            previousKeyStates[Key.H] = false;
        }

        public void ProcessInput()
        {
            // Reverse orbit rotation (R key)
            if (IsKeyJustPressed(Key.R))
            {
                ReverseOrbitRotation = !ReverseOrbitRotation;
            }

            // Swap colors (C key)
            if (IsKeyJustPressed(Key.C))
            {
                SwapEyeCrossColors = !SwapEyeCrossColors;
            }

            // Increase animation speeds (+ key)
            if (IsKeyJustPressed(Key.Equal))
            {
                OrbitSpeedModifier = Mathf.Min(OrbitSpeedModifier + 0.1f, 3.0f);
                BreathingSpeedModifier = Mathf.Min(BreathingSpeedModifier + 0.1f, 3.0f);
                TrapezoidSpeedModifier = Mathf.Min(TrapezoidSpeedModifier + 0.1f, 3.0f);
            }

            // Decrease animation speeds (- key)
            if (IsKeyJustPressed(Key.Minus))
            {
                OrbitSpeedModifier = Mathf.Max(OrbitSpeedModifier - 0.1f, 0.1f);
                BreathingSpeedModifier = Mathf.Max(BreathingSpeedModifier - 0.1f, 0.1f);
                TrapezoidSpeedModifier = Mathf.Max(TrapezoidSpeedModifier - 0.1f, 0.1f);
            }

            // Toggle help display (H key or ESC)
            if (IsKeyJustPressed(Key.H) || Input.IsActionJustPressed("ui_cancel"))
            {
                ShowHelp = !ShowHelp;
            }

            // Update key states for next frame
            UpdatePreviousKeyStates();
        }

        // Check if a key was just pressed (pressed this frame but not last frame)
        private bool IsKeyJustPressed(Key key)
        {
            bool isCurrentlyPressed = Input.IsKeyPressed(key);

            // Get previous state, default to false if not found
            if (!previousKeyStates.TryGetValue(key, out bool wasPreviouslyPressed))
            {
                wasPreviouslyPressed = false;
            }

            // Return true only if key is pressed now but wasn't before
            return isCurrentlyPressed && !wasPreviouslyPressed;
        }

        // Update previous key states for the next frame
        private void UpdatePreviousKeyStates()
        {
            previousKeyStates[Key.R] = Input.IsKeyPressed(Key.R);
            previousKeyStates[Key.C] = Input.IsKeyPressed(Key.C);
            previousKeyStates[Key.Equal] = Input.IsKeyPressed(Key.Equal);
            previousKeyStates[Key.Minus] = Input.IsKeyPressed(Key.Minus);
            previousKeyStates[Key.H] = Input.IsKeyPressed(Key.H);
        }

        // Reset all modifiers to default values
        public void ResetModifiers()
        {
            OrbitSpeedModifier = 1.0f;
            BreathingSpeedModifier = 1.0f;
            TrapezoidSpeedModifier = 1.0f;
            ReverseOrbitRotation = false;
            ReverseTrapezoidMovement = false;
            SwapEyeCrossColors = false;
        }
    }
}
