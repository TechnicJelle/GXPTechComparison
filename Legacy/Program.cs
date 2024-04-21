using System;
using System.Drawing;
using System.Drawing.Imaging;
using Legacy.bindings;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Legacy;

static internal class Program
{
	// Settings
	private const int SCR_WIDTH = 800;
	private const int SCR_HEIGHT = 600;

	private const string WINDOW_TITLE = "Project Legacy";

	private static bool _benchmark = false;
	private static readonly List<double> Milliseconds = new(25000); // 10 seconds at around 2500 fps

	private static readonly float[] Vertices =
	[
		-0.5f, 0.5f, // left top
		0.5f, 0.5f, // right top
		0.5f, -0.5f, // right bottom
		-0.5f, -0.5f, // left bottom
	];

	private static readonly float[] UVs =
	[
		0.0f, 0.0f, // left top
		1.0f, 0.0f, // right top
		1.0f, 1.0f, // right bottom
		0.0f, 1.0f, // left bottom
	];

	private static readonly int[] Texture = new int[1];

	private static readonly byte[] Colour = [255, 255, 255, 255]; // RGBA

	public static int Main(string[] args)
	{
		if (args.Contains("benchmark")) _benchmark = true;

		CreateWindow();

		CreateGLTexture();

		Run();

		Close();

		if (_benchmark)
		{
			// save recorded frame times to a file
			File.WriteAllLines("milliseconds_win.txt", Milliseconds.ConvertAll(d => d.ToString(CultureInfo.InvariantCulture)));
		}

		return 0;
	}

	private static void CreateWindow()
	{
		// Initialisation
		GLFW.Init();

		// Configuration
		// > Multisampling
		GLFW.OpenWindowHint(GLFW.FSAA_SAMPLES, 8);

		// Window Creation
		GLFW.OpenWindow(SCR_WIDTH, SCR_HEIGHT, 8, 8, 8, 8, 24, 0, GLFW.WINDOWED);
		GLFW.SetWindowTitle(WINDOW_TITLE);

		// VSync
		GLFW.SwapInterval(false);

		// Inputs
		GLFW.SetKeyCallback((key, action) =>
		{
			bool press = action == 1;
			if (press && key == Key.ESCAPE)
				GLFW.CloseWindow();
		});

		GLFW.SetWindowSizeCallback((int newWidth, int newHeight) =>
		{
			Console.WriteLine($"Window resized to {newWidth}x{newHeight}");
			GL.Viewport(0, 0, newWidth, newHeight);
			GL.Enable(GL.MULTISAMPLE);
			GL.Enable(GL.TEXTURE_2D);
			GL.Enable(GL.BLEND);
			GL.BlendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
			GL.Hint(GL.PERSPECTIVE_CORRECTION, GL.FASTEST);
			GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
		});

		Console.WriteLine("GLFW window created");
	}

	private static void CreateGLTexture()
	{
		GL.GenTextures(1, Texture);

		GL.BindTexture(GL.TEXTURE_2D, Texture[0]); // all upcoming GL_TEXTURE_2D operations now have effect on this texture object

		// set texture filtering parameters (not PixelArt)
		GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR);
		GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, GL.LINEAR);

		// set the texture wrapping parameters
		GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, GL.CLAMP_TO_EDGE_EXT);
		GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, GL.CLAMP_TO_EDGE_EXT);

		UpdateGLTexture();

		GL.BindTexture(GL.TEXTURE_2D, 0); //unbind texture
	}

	private static void UpdateGLTexture()
	{
		Bitmap bitmap = new("assets/textures/container.jpg");

		BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
		GL.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, bitmap.Width, bitmap.Height, 0, GL.BGRA, GL.UNSIGNED_BYTE, data.Scan0);
		bitmap.UnlockBits(data);
	}

	private static void Run()
	{
		GLFW.SetTime(0.0);

		double lastTime = GLFW.GetTime();
		double fpsTimer = GLFW.GetTime();
		do
		{
			double deltaTime = GLFW.GetTime() - lastTime;
			lastTime = GLFW.GetTime();

			// Change Window Title to show FPS, every second
			if (GLFW.GetTime() - fpsTimer >= 1.0)
			{
				GLFW.SetWindowTitle($"{WINDOW_TITLE} - FPS: " + Math.Round(1.0 / deltaTime) + " - Benchmarking: " + _benchmark + " - Time: " + Math.Round(GLFW.GetTime()));
				fpsTimer = GLFW.GetTime();
			}

			Display();

			if (_benchmark)
			{
				if (GLFW.GetTime() >= 11.0)
				{
					GLFW.CloseWindow();
				}
				else if (GLFW.GetTime() > 1.0) // skip the first second, to avoid the initial lag
				{
					Milliseconds.Add(deltaTime * 1000); // in milliseconds
				}
			}

			GLFW.PollEvents();
		} while(GLFW.GetWindowParam(GLFW.ACTIVE) == 1);
	}

	private static void Display()
	{
		// Clear the screen
		GL.Clear(GL.COLOR_BUFFER_BIT);

		RenderSelf();

		// GLFW: Swap Buffers and Poll IO Events (keys pressed/released, mouse moved etc.)
		GLFW.SwapBuffers();
	}

	private static void RenderSelf()
	{
		GL.BindTexture(GL.TEXTURE_2D, Texture[0]);
		GL.Color4ub(Colour[0], Colour[1], Colour[2], Colour[3]);
		DrawQuad();
		GL.Color4ub(255, 255, 255, 255);
		GL.BindTexture(GL.TEXTURE_2D, 0); // unbind texture
	}

	private static void DrawQuad()
	{
		GL.EnableClientState(GL.TEXTURE_COORD_ARRAY);
		GL.EnableClientState(GL.VERTEX_ARRAY);
		GL.TexCoordPointer(2, GL.FLOAT, 0, UVs);
		GL.VertexPointer(2, GL.FLOAT, 0, Vertices);
		GL.DrawArrays(GL.QUADS, 0, 4);
		GL.DisableClientState(GL.VERTEX_ARRAY);
		GL.DisableClientState(GL.TEXTURE_COORD_ARRAY);
	}

	private static void Close()
	{
		GLFW.Terminate();
	}
}
