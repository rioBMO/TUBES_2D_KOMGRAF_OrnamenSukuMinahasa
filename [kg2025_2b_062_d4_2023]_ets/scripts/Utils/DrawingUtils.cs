using Godot;
using System;
using System.Collections.Generic;

namespace KG2025.Utils
{
    public static class DrawingUtils
    {
        // Line Drawing Algorithm (DDA)
        public static List<Vector2> LineDDA(float xa, float ya, float xb, float yb)
        {
            float dx = xb - xa;
            float dy = yb - ya;
            float steps;
            float xIncrement;
            float yIncrement;
            float x = xa;
            float y = ya;

            List<Vector2> res = new List<Vector2>();

            if (Mathf.Abs(dx) > Mathf.Abs(dy))
            {
                steps = Mathf.Abs(dx);
            }
            else
            {
                steps = Mathf.Abs(dy);
            }

            xIncrement = dx / steps;
            yIncrement = dy / steps;

            res.Add(new Vector2(Mathf.Round(x), Mathf.Round(y)));

            for (int k = 0; k < steps; k++)
            {
                x += xIncrement;
                y += yIncrement;
                res.Add(new Vector2(Mathf.Round(x), Mathf.Round(y)));
            }

            return res;
        }

        // Line Drawing Algorithm (Bresenham)
        public static List<Vector2> LineBresenham(float xa, float ya, float xb, float yb)
        {
            List<Vector2> res = new List<Vector2>();
            int x1 = (int)xa;
            int y1 = (int)ya;
            int x2 = (int)xb;
            int y2 = (int)yb;

            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                res.Add(new Vector2(x1, y1));
                if (x1 == x2 && y1 == y2) break;
                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x1 += sx; }
                if (e2 < dx) { err += dx; y1 += sy; }
            }
            return res;
        }

        // Circle Drawing Algorithm
        public static List<Vector2> Lingkaran(float xCenter, float yCenter, float radius)
        {
            List<Vector2> points = new List<Vector2>();

            int x = 0;
            int y = (int)radius;
            int p = 1 - (int)radius;

            // Plot first set of points
            PlotCirclePoints(points, xCenter, yCenter, x, y);

            while (x < y)
            {
                x++;
                if (p < 0)
                {
                    p += 2 * x + 1;
                }
                else
                {
                    y--;
                    p += 2 * (x - y) + 1;
                }

                PlotCirclePoints(points, xCenter, yCenter, x, y);
            }

            return points;
        }

        private static void PlotCirclePoints(List<Vector2> points, float xCenter, float yCenter, int x, int y)
        {
            // Plot all 8 symmetric points
            points.Add(new Vector2(xCenter + x, yCenter + y));
            points.Add(new Vector2(xCenter - x, yCenter + y));
            points.Add(new Vector2(xCenter + x, yCenter - y));
            points.Add(new Vector2(xCenter - x, yCenter - y));
            points.Add(new Vector2(xCenter + y, yCenter + x));
            points.Add(new Vector2(xCenter - y, yCenter + x));
            points.Add(new Vector2(xCenter + y, yCenter - x));
            points.Add(new Vector2(xCenter - y, yCenter - x));
        }

        // Put a single pixel on the screen
        public static void PutPixel(Node2D targetNode, float x, float y, Color? color = null)
        {
            Color actualColor = color ?? Colors.White;
            Vector2[] points = new Vector2[]
            {
                new Vector2(Mathf.Round(x), Mathf.Round(y)),
            };
            Vector2[] uvs = new Vector2[]
            {
                Vector2.Zero,
                Vector2.Down,
                Vector2.One,
                Vector2.Right,
            };
            targetNode.DrawPrimitive(points, new Color[] { actualColor }, uvs);
        }

        // Draw multiple pixels at once
        public static void PutPixelAll(Node2D targetNode, List<Vector2> points, Color? color = null)
        {
            foreach (Vector2 point in points)
                PutPixel(targetNode, point.X, point.Y, color);
        }

        // Draw a polygon outline
        public static void DrawPolygonOutline(Node2D targetNode, Vector2[] points, Color color)
        {
            // Draw each side of the polygon
            for (int i = 0; i < points.Length; i++)
            {
                // Get next point (wrap around to first point for the last segment)
                int nextIdx = (i + 1) % points.Length;

                // Use Bresenham algorithm
                List<Vector2> linePoints = LineBresenham(
                    points[i].X, points[i].Y,
                    points[nextIdx].X, points[nextIdx].Y
                );

                // Draw each point in the line
                PutPixelAll(targetNode, linePoints, color);
            }
        }
    }
}
