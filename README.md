# GarticPhonk - Gartic Phone Auto Drawer

An implementation of Gartic Phone socket communications in C# with an advanced auto-drawing feature that uses human-like strokes.

## Features

- **DFS Drawing Algorithm**: Uses a Depth-First Search algorithm to create natural-looking strokes
- **Human-like Drawing**: Simulates human drawing with variable stroke pressure, speed, and direction
- **Color Grouping**: Intelligently groups similar colors for more efficient drawing
- **Customizable Stroke Size**: Adjust the stroke size to match your drawing style

## How It Works

The auto drawer uses several techniques to create drawings that look like they were made by a human:

1. **Color Analysis**: Analyzes the image to find the most common background color
2. **DFS Traversal**: Uses a depth-first search algorithm to find connected regions of similar colors
3. **Stroke Simulation**: Creates strokes with variable pressure and speed, just like a human would draw
4. **Natural Pauses**: Adds small delays between strokes to simulate human drawing rhythm

## How to Use

1. Start the application
2. Enter your nickname
3. Paste the Gartic Phone invite link (works with any language version)
4. Wait for your turn to draw
5. When prompted, select an image from your computer
6. Choose a stroke size (1-5)
7. Watch as the program draws your image with human-like strokes!

## Requirements

- .NET Framework 4.8
- Windows OS (for WinForms support)

## Building from Source

1. Open the solution in Visual Studio
2. Restore NuGet packages
3. Build the solution
4. Run the GarticTest project

## Legal Notice

This tool is for educational purposes only. Use responsibly and respect Gartic Phone's terms of service.

## Credits

- Original GarticPhonk implementation by [mocxlbor4](https://github.com/mocxlbor4)
- DFS Drawing Algorithm implementation added as an enhancement
