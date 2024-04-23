using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SkiaSharp;
using System.Diagnostics.CodeAnalysis;

namespace BenchSkiaSharp;

// Benchmarks must be instance methods, static methods are not supported.
[SuppressMessage("Performance", "CA1822:Mark members as static")]
public class ImageLoader
{
	[Benchmark]
	public IntPtr LoadImageAndGetPixels()
	{
		SKBitmap bitmap = SKBitmap.Decode("assets/textures/container.jpg");
		bitmap.SetImmutable();
		return bitmap.GetPixels();
	}
}

static internal class Program
{
	private static void Main(string[] args)
	{
		Console.WriteLine(new ImageLoader().LoadImageAndGetPixels()); //make sure it works first
		BenchmarkRunner.Run<ImageLoader>();
	}
}
