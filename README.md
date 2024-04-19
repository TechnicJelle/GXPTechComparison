# GXP Tech Comparison
For my "Advanced Tools" course at Saxion CMGT.

## Evaluation proposal
I want to compare the (graphics) performance of
 Legacy OpenGL paired with (Windows) System.Drawing running on .NET Framework 4.8,
 with Modern OpenGL paired with SkiaSharp running on .NET 7 or 8.

The former is the technology currently used in the GXP Engine,
the latter is the technology we're interested in using for NeoGXP.

## Project Differences
| Project | Win | Skia |
| ------- | --- | ---- |
| Language | C#	| C# |
| Runtime |	.NET Framework 4.8 | .NET 8 |
| OpenGL Version |	Legacy (1, I think?) | 3.3 |
| Image Loading |	System.Drawing | SkiaSharp |
