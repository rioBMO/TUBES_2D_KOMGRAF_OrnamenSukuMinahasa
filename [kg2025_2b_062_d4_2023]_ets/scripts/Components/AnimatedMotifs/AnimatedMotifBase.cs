using Godot;
using System;
using System.Collections.Generic;
using KG2025.Utils;
using KG2025.Components;

namespace KG2025.Components.AnimatedMotifs
{
    public abstract class AnimatedMotifBase
    {
        protected Node2D parent;
        protected KartesiusSystem kartesiusSystem;
        protected Color lineColor = Colors.White;

        // Animation properties
        protected float orbitAngle = 0f;
        protected float orbitSpeed = 0.3f; // Radians per second

        // Breathing animation properties
        protected float breathingFactor = 1.0f;
        protected float breathingSpeed = 0.5f;
        protected float breathingTime = 0f;
        protected float minScale = 0.7f;
        protected float maxScale = 1.3f;

        public AnimatedMotifBase(Node2D parent, KartesiusSystem kartesiusSystem)
        {
            this.parent = parent;
            this.kartesiusSystem = kartesiusSystem;
        }

        // Set colors for the motif
        public void SetColors(Color lineColor)
        {
            this.lineColor = lineColor;
        }

        // Set animation parameters
        public void SetAnimationParams(float orbitSpeed, float breathingSpeed, float minScale, float maxScale)
        {
            this.orbitSpeed = orbitSpeed;
            this.breathingSpeed = breathingSpeed;
            this.minScale = minScale;
            this.maxScale = maxScale;
        }

        // Update animation state
        public virtual void Update(float delta)
        {
            // Update orbit angle
            orbitAngle += delta * orbitSpeed;
            if (orbitAngle > 2 * Mathf.Pi)
                orbitAngle -= 2 * Mathf.Pi;

            // Update breathing animation
            breathingTime += delta * breathingSpeed;
            if (breathingTime > Mathf.Pi * 2)
                breathingTime -= Mathf.Pi * 2;

            // Calculate breathing factor (0 to 1 to 0)
            breathingFactor = minScale + ((Mathf.Sin(breathingTime) + 1) / 2) * (maxScale - minScale);
        }

        // Abstract draw method to be implemented by derived classes
        public abstract void Draw();

        // Helper methods for derived classes
        protected void DrawCircle(float x, float y, float radius)
        {
            List<Vector2> points = DrawingUtils.Lingkaran(x, y, radius);
            DrawingUtils.PutPixelAll(parent, points, lineColor);
        }

        protected void DrawLine(float x1, float y1, float x2, float y2)
        {
            List<Vector2> points = DrawingUtils.LineDDA(x1, y1, x2, y2);
            DrawingUtils.PutPixelAll(parent, points, lineColor);
        }

        protected void DrawPolygon(Vector2[] points)
        {
            DrawingUtils.DrawPolygonOutline(parent, points, lineColor);
        }
    }
}
