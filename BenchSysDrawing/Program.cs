using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace BenchSysDrawing;

public class ImageLoader
{
	public IntPtr LoadImageAndGetPixels()
	{
		Bitmap bitmap = new("assets/textures/container.jpg");
		BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
		bitmap.UnlockBits(data);
		return data.Scan0;
	}
}

static internal class Program
{
	private static void Main(string[] args)
	{
		Console.WriteLine(new ImageLoader().LoadImageAndGetPixels()); //make sure it works first
	}
}
