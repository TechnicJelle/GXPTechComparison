# GXP Tech Comparison
I've been planning on modernising the GXP Engine for a while now.  
So this project is a comparison between the current technology stack and a new proposed technology stack.  
This was done for my "Advanced Tools" course at Saxion CMGT.

<!-- TOC -->
* [GXP Tech Comparison](#gxp-tech-comparison)
  * [Evaluation proposal](#evaluation-proposal)
  * [Method](#method)
  * [Project Differences](#project-differences)
  * [Graphics Applications](#graphics-applications)
  * [Image Loader Benchmarks](#image-loader-benchmarks)
  * [Experiences](#experiences)
    * [On Legacy OpenGL](#on-legacy-opengl)
    * [On .NET Framework](#on-net-framework)
  * [Conclusion](#conclusion)
  * [References](#references)
<!-- TOC -->

## Evaluation proposal
I want to compare the (graphics) performance of
 Legacy OpenGL paired with (Windows) System.Drawing running on .NET Framework 4.8,
 with Modern OpenGL paired with SkiaSharp running on .NET 8.

The former is the technology currently used in the GXP Engine,
the latter is the technology we're interested in using for NeoGXP.

## Method
To compare these two technology stacks, I have created two projects:  
One project uses the [Legacy](./Legacy) technology stack, the other uses the [Modern](./Modern) technology stack.

I have also created two other projects for benchmarking specifically
the image loading aspect of the two technology stacks,
which you can find in [BenchSysDrawing](./BenchSysDrawing) and [BenchSkiaSharp](./BenchSkiaSharp).

## Project Differences
Here's a nice table outlining the differences between the two tech stacks:

| Project                | Legacy                 | Modern                                            |
|------------------------|------------------------|---------------------------------------------------|
| Language               | `C#`                   | `C#`                                              |
| Runtime                | `.NET Framework 4.8`   | `.NET 8`                                          |
| OpenGL Version         | `Legacy (1, I think?)` | `3.3`                                             |
| GLFW Version           | `2.6`                  | `3.3.2+`                                          |
| OpenGL & GLFW bindings | `Custom ones`          | [`Arqan`](https://github.com/TheBoneJarmer/Arqan) |
| Image Loading          | `System.Drawing`       | `SkiaSharp`                                       |
| Architecture           | `32-bit`               | `64-bit`                                          |
| Platform               | `Windows`              | `Linux & Windows`                                 |

(Project Legacy used to be called "ProjWinDrawing", and Project Modern used to be called "ProjSkiaSharp".  
I have changed the names to better suit the slightly larger comparison scope.)

## Graphics Applications
To compare the two technology stacks, I have created a simple application for both.
It loads an image and displays it in a window using OpenGL and GLFW.

All the image assets for these projects came from either LearnOpenGL or the GXP Engine.

![Screenshot of the Modern project](./.github/readme_assets/demo.png)

The Legacy project was modeled as closely as possible to the rendering part of the current GXP Engine,
and the Modern project was then in turn modeled after the Legacy project, but with the new technology stack.

I have deliberately kept the projects as simple as possible, to make the comparison as fair as possible.
All the code is in a single file.

The measured performance data are the delta time between frames, for every frame rendered, over a period of 10 seconds,
after a warm-up period of 1 second.  
By default, however, the program is not in Benchmark mode, so it will run indefinitely.
By starting the program with the `benchmark` argument, it will run in Benchmark mode.

Here are the relevant parts of the code that logs the delta times:
```csharp
// The list of measured delta times
//  It is prepopulared with enough space for 10 seconds at around 2500 fps
//  This is done to prevent the list from resizing during the benchmark,
//   which would cause the program to stutter.
private static readonly List<double> Milliseconds = new(capacity: 25000);

// Before the game loop:
double lastTime = glfwGetTime();
double fpsTimer = glfwGetTime();

// In the game loop:
double deltaTime = glfwGetTime() - lastTime;
lastTime = glfwGetTime();

if (glfwGetTime() >= 11.0)
{
    // Close the window after 10 seconds:
	glfwSetWindowShouldClose(_window, 1);
}
else if (glfwGetTime() > 1.0) // skips the first second, to avoid the initial lag
{
    // Log the delta time:
	Milliseconds.Add(deltaTime * 1000); // in milliseconds
}

// Once the program is done:
File.WriteAllLines("milliseconds.txt", Milliseconds.ConvertAll(d => d.ToString()));
```
Here is the result of the graphics application benchmarks:

In this graph, you can see the delta times of the two projects over time.
![](https://technicjelle.github.io/GXPTechComparison/plot.png)
(I've done my best to style this graph to be as clear and informative as possible,
but there's only so much I can do when I have around 30 000 data points.)

The reason the Legacy project has fewer data points is because it ran at a lower frame rate.
This naturally caused there to be fewer data points in the same time frame of 10 seconds.

In this graph, you can see the histogram of the delta times of the two projects.
That shows how often a certain delta time occurred.
![](https://technicjelle.github.io/GXPTechComparison/hist.png)
(The values over 0.60 ms have been clipped off to free up more space for the important parts of the data.
There were hardly any data points above 0.60 ms anyway.)

## Image Loader Benchmarks
The image loading benchmark projects are much simpler: it just loads an image,
and gets the pointer to the pixel data in memory.

The measuring here is done more "properly", by using [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet).
Using it made it incredibly simple to very accurately and properly compare the two image loading methods.  
Due to the way C# works, in regard to JIT compilation and garbage collection,
the first run of the benchmarks is usually slower than the rest, because it has to compile the code.
BenchmarkDotNet takes care of this by running the benchmark multiple times and discarding the first run.

I had actually already used a tool like this before, for one of my previous Java projects,
so learning this one was a breeze.  
I even managed to get it to work with the .NET Framework 4.8 project, which was a **huge** hassle.

Here is the relevant code for the image loading benchmarks:
```csharp
class ImageLoader
{
	[Benchmark]
	IntPtr LoadImageAndGetPixelsSkiaSharp()
	{
		SKBitmap bitmap = SKBitmap.Decode("assets/textures/container.jpg");
		bitmap.SetImmutable();
		return bitmap.GetPixels();
	}

	[Benchmark]
	IntPtr LoadImageAndGetPixelsSysDrawing()
	{
		Bitmap bitmap = new("assets/textures/container.jpg");
		BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
		bitmap.UnlockBits(data);
		return data.Scan0;
	}
}

BenchmarkRunner.Run<ImageLoader>();
```
(Here, I have combined the code for System.Drawing and SkiaSharp in one code snippet.
In reality, they are two separate projects entirely, due to using different .NET versions.)

I have also managed to create a GitHub Actions workflow that runs these benchmarks on every push to the repository,
and publishes a report to the Actions tab:
![](./.github/readme_assets/benches-summaries.png)
It also publishes the raw benchmark results as artifacts, so they can be downloaded and inspected.
With those artifacts, it also automatically creates a plot of the results.

Here is the result of the image loading benchmarks:
![](https://technicjelle.github.io/GXPTechComparison/benches.png)

## Experiences

Here are some various thoughts and experiences I had during the development of this project:

### On Legacy OpenGL
The GXP Engine started as an educational tool. And while NeoGXP doesn't have that same goal,
using more modern OpenGL does make it a better resource to learn from anyway.  
For people who want to look under the hood to see how it works, they'll learn something useful,
instead of something that had already become outdated before I was born.

Legacy OpenGL is not supported or taught basically anywhere anymore,
as I painstakingly found out during the making of this project.
It has been extremely difficult to find good resources on it.  
The only good guide I found was at https://nehe.gamedev.net/, and it is already half-broken;
all the example projects and images have disappeared.

### On .NET Framework
During the development of this project, I have really felt the age of the .NET Framework.
It is extremely clunky, especially in the building and packaging department.
I have really come to appreciate the new `csproj` format, and the new `dotnet` CLI tool.
They really managed to streamline the process of building and running .NET projects with .NET 5 and later.

Trying to make GitHub Actions retrieve the dependencies, build, and run the .NET Framework project was a true nightmare.
Many of the commands required were surprisingly badly documented, for Microsoft standards,
and some were downright contradictory. Aside from that, they were also rather nonsensical, in my opinion.
Setting up the .NET 8 project went without any issues at all, on the other hand.  
Thankfully, I managed to get it to work in the end with a lot of help from friends,
but it was not a pleasant experience at all.

One more reason to move to .NET 8.

## Conclusion
The Modern technology stack is more performant than the Legacy technology stack,
while also being more modern, better supported, better documented, and just plain simpler,
while still offering more control and features in case you want it at the same time.

Even though SkiaSharp may be a bit slower at image loading than System.Drawing,
I don't think that's a big enough reason to not switch to it,
considering all the other advantages of SkiaSharp, like cross-platform support.  
It really is a pretty small difference, and it's only in the image loading part of the program,
which should not happen very often anyway.

## References

Bavoil, L. (2023, February 13). _The Peak-Performance-Percentage Analysis Method for Optimizing Any GPU Workload_. NVIDIA Technical Blog.
Retrieved April 20, 2024, from https://developer.nvidia.com/blog/the-peak-performance-analysis-method-for-optimizing-any-gpu-workload

De Vries, J. and contributors. (2015, March 23). _Learn OpenGL, extensive tutorial resource for learning Modern OpenGL_. Learn OpenGL.
Retrieved February 21, 2023, from https://learnopengl.com/

Molofee, J. (2000, January). _Your First Polygon_. NeHe Productions.
Retrieved April 12, 2024, from https://nehe.gamedev.net/tutorial/your_first_polygon/13002/
