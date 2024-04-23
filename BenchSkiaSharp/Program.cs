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
		SKBitmap original = SKBitmap.Decode("assets/textures/container.jpg");
		original.SetImmutable();
		SKBitmap flipped = BitmapFlipped(original);
		flipped.SetImmutable();
		return flipped.GetPixels();
	}

	private static SKBitmap BitmapFlipped(SKBitmap bitmap)
	{
		SKBitmap flipped = new(bitmap.Width, bitmap.Height);
		using SKCanvas canvas = new(flipped);
		canvas.Scale(1, -1, 0, bitmap.Height / 2.0f);
		canvas.DrawBitmap(bitmap, 0, 0);
		return flipped;
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
