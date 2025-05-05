using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace GarticTest
{
    public class GarticDFSDrawer
    {
        private readonly GarticDrawController _drawController;
        private readonly Random _random = new Random();
        
        // Canvas dimensions for Gartic Phone
        private const int CanvasWidth = 760;
        private const int CanvasHeight = 425;
        
        // Parameters for human-like drawing
        private const int MinStrokeLength = 5;
        private const int MaxStrokeLength = 30;
        private const int MinStrokeDelay = 10;
        private const int MaxStrokeDelay = 100;
        private const int MinPressureVariation = 1;
        private const int MaxPressureVariation = 3;
        
        // Directions for DFS traversal (8 directions)
        private static readonly Point[] Directions = new Point[]
        {
            new Point(1, 0),   // right
            new Point(0, 1),   // down
            new Point(-1, 0),  // left
            new Point(0, -1),  // up
            new Point(1, 1),   // down-right
            new Point(-1, 1),  // down-left
            new Point(-1, -1), // up-left
            new Point(1, -1)   // up-right
        };

        public GarticDFSDrawer(GarticDrawController drawController)
        {
            _drawController = drawController;
        }

        /// <summary>
        /// Draws an image using DFS algorithm with human-like strokes
        /// </summary>
        public void DrawImageWithDFS(string filename, int strokeSize = 2, float opacity = 1.0f)
        {
            // Load and resize the image to fit Gartic Phone canvas
            Bitmap image = Image.FromFile(filename).ResizeImage(CanvasWidth, CanvasHeight);
            
            // Get the most used color for background
            Color backgroundColor = image.GetMostUsedColor();
            if (backgroundColor != Color.White && backgroundColor != Color.Transparent)
            {
                _drawController.DrawFilledRectangle(backgroundColor, new Rectangle(0, 0, CanvasWidth, CanvasHeight));
                Thread.Sleep(_random.Next(300, 800)); // Pause like a human would after drawing background
            }
            
            // Create a visited array to track which pixels we've processed
            bool[,] visited = new bool[image.Width, image.Height];
            
            // Group similar colors for more efficient drawing
            Dictionary<Color, List<Point>> colorGroups = GroupSimilarColors(image);
            
            // Draw each color group using DFS with human-like strokes
            foreach (var colorGroup in colorGroups)
            {
                Color color = colorGroup.Key;
                List<Point> points = colorGroup.Value;
                
                // Skip background color or transparent pixels
                if (color.ToArgb() == backgroundColor.ToArgb() || color.A < 10)
                    continue;
                
                // Draw this color group with DFS
                DrawColorGroupWithDFS(image, color, points, visited, strokeSize, opacity);
                
                // Pause between color groups like a human would
                Thread.Sleep(_random.Next(200, 500));
            }
        }
        
        /// <summary>
        /// Groups similar colors together to make drawing more efficient
        /// </summary>
        private Dictionary<Color, List<Point>> GroupSimilarColors(Bitmap image)
        {
            Dictionary<Color, List<Point>> colorGroups = new Dictionary<Color, List<Point>>();
            
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    
                    // Skip transparent pixels
                    if (pixelColor.A < 10)
                        continue;
                    
                    // Find the closest color group or create a new one
                    Color closestColor = FindClosestColor(pixelColor, colorGroups.Keys.ToList());
                    
                    if (closestColor == Color.Empty)
                    {
                        // Create a new color group
                        colorGroups[pixelColor] = new List<Point> { new Point(x, y) };
                    }
                    else
                    {
                        // Add to existing color group
                        colorGroups[closestColor].Add(new Point(x, y));
                    }
                }
            }
            
            return colorGroups;
        }
        
        /// <summary>
        /// Finds the closest color in a list of colors, or returns Color.Empty if no close match
        /// </summary>
        private Color FindClosestColor(Color targetColor, List<Color> colors)
        {
            if (colors.Count == 0)
                return Color.Empty;
            
            const int colorThreshold = 20; // Threshold for considering colors similar
            
            foreach (Color color in colors)
            {
                int rDiff = Math.Abs(targetColor.R - color.R);
                int gDiff = Math.Abs(targetColor.G - color.G);
                int bDiff = Math.Abs(targetColor.B - color.B);
                
                if (rDiff + gDiff + bDiff < colorThreshold)
                    return color;
            }
            
            return Color.Empty;
        }
        
        /// <summary>
        /// Draws a color group using DFS with human-like strokes
        /// </summary>
        private void DrawColorGroupWithDFS(Bitmap image, Color color, List<Point> points, bool[,] visited, int strokeSize, float opacity)
        {
            // Sort points by proximity to create more natural drawing order
            points = points.OrderBy(p => p.X + p.Y).ToList();
            
            foreach (Point startPoint in points)
            {
                if (visited[startPoint.X, startPoint.Y])
                    continue;
                
                // Start a new stroke from this point
                Stack<Point> stack = new Stack<Point>();
                List<Point> currentStroke = new List<Point>();
                
                stack.Push(startPoint);
                
                while (stack.Count > 0)
                {
                    Point current = stack.Pop();
                    
                    // Skip if already visited
                    if (current.X < 0 || current.X >= image.Width || 
                        current.Y < 0 || current.Y >= image.Height || 
                        visited[current.X, current.Y])
                        continue;
                    
                    // Check if color matches our target color group
                    Color pixelColor = image.GetPixel(current.X, current.Y);
                    if (!IsColorSimilar(pixelColor, color))
                        continue;
                    
                    // Mark as visited
                    visited[current.X, current.Y] = true;
                    
                    // Add to current stroke
                    currentStroke.Add(current);
                    
                    // If stroke is long enough, draw it and start a new one
                    if (currentStroke.Count >= _random.Next(MinStrokeLength, MaxStrokeLength))
                    {
                        DrawHumanLikeStroke(currentStroke, color, strokeSize, opacity);
                        currentStroke = new List<Point>();
                        
                        // Add a small delay between strokes to simulate human drawing
                        Thread.Sleep(_random.Next(MinStrokeDelay, MaxStrokeDelay));
                    }
                    
                    // Shuffle directions for more natural looking strokes
                    Point[] shuffledDirections = ShuffleDirections();
                    
                    // Add neighbors to stack in random order (DFS)
                    foreach (Point dir in shuffledDirections)
                    {
                        stack.Push(new Point(current.X + dir.X, current.Y + dir.Y));
                    }
                }
                
                // Draw any remaining points in the stroke
                if (currentStroke.Count > 0)
                {
                    DrawHumanLikeStroke(currentStroke, color, strokeSize, opacity);
                }
            }
        }
        
        /// <summary>
        /// Checks if two colors are similar enough to be considered the same
        /// </summary>
        private bool IsColorSimilar(Color c1, Color c2)
        {
            const int threshold = 20;
            int rDiff = Math.Abs(c1.R - c2.R);
            int gDiff = Math.Abs(c1.G - c2.G);
            int bDiff = Math.Abs(c1.B - c2.B);
            
            return rDiff + gDiff + bDiff < threshold;
        }
        
        /// <summary>
        /// Shuffles the direction array for more natural looking strokes
        /// </summary>
        private Point[] ShuffleDirections()
        {
            Point[] shuffled = Directions.ToArray();
            
            // Fisher-Yates shuffle
            for (int i = shuffled.Length - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                Point temp = shuffled[i];
                shuffled[i] = shuffled[j];
                shuffled[j] = temp;
            }
            
            return shuffled;
        }
        
        /// <summary>
        /// Draws a stroke with human-like variations in pressure and speed
        /// </summary>
        private void DrawHumanLikeStroke(List<Point> points, Color color, int baseStrokeSize, float baseOpacity)
        {
            if (points.Count < 2)
            {
                // For single points, just draw a point
                if (points.Count == 1)
                {
                    _drawController.DrawPoint(color, points[0], baseStrokeSize, baseOpacity);
                }
                return;
            }
            
            // Break the stroke into smaller segments for more natural drawing
            for (int i = 0; i < points.Count - 1; i += 2)
            {
                // Vary stroke size slightly to simulate pressure changes
                int strokeVariation = _random.Next(MinPressureVariation, MaxPressureVariation);
                int currentStrokeSize = Math.Max(1, baseStrokeSize + (_random.Next(2) == 0 ? -strokeVariation : strokeVariation));
                
                // Vary opacity slightly
                float opacityVariation = (float)_random.NextDouble() * 0.2f;
                float currentOpacity = Math.Min(1.0f, Math.Max(0.5f, baseOpacity + (_random.Next(2) == 0 ? -opacityVariation : opacityVariation)));
                
                if (i + 1 < points.Count)
                {
                    // Draw line between consecutive points
                    _drawController.DrawLine(color, points[i], points[i + 1], currentStrokeSize, currentOpacity);
                    
                    // Small delay to simulate human drawing speed
                    Thread.Sleep(_random.Next(5, 20));
                }
            }
        }
    }
}