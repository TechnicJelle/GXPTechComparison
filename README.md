# GXP Tech Comparison
I've been planning on modernising the GXP Engine for a while now.  
So this project is a comparison between the current technology stack and a new proposed technology stack.  
This was done for my "Advanced Tools" course at Saxion CMGT.

## Evaluation proposal
I want to compare the (graphics) performance of
 Legacy OpenGL paired with (Windows) System.Drawing running on .NET Framework 4.8,
 with Modern OpenGL paired with SkiaSharp running on .NET 7 or 8.

The former is the technology currently used in the GXP Engine,
the latter is the technology we're interested in using for NeoGXP.

## Project Differences
| Project        | Legacy               | Modern          |
|----------------|----------------------|-----------------|
| Language       | C#                   | C#              |
| Runtime        | .NET Framework 4.8   | .NET 8          |
| OpenGL Version | Legacy (1, I think?) | 3.3             |
| GLFW Version   | 2.6                  | 3.3.2+          |
| Image Loading  | System.Drawing       | SkiaSharp       |
| Architecture   | 32-bit               | 64-bit          |
| Platform       | Windows              | Linux & Windows |

Project Legacy used to be called "ProjWinDrawing", and Project Modern used to be called "ProjSkiaSharp".  
I have changed the names to better suit the slightly larger comparison scope.
