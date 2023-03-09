using System;
using System.Drawing;
using System.Drawing.Imaging;
using ProjWinDrawing.bindings;

namespace ProjWinDrawing;

internal static class Program
{
	// Settings
	private const int SCR_WIDTH = 800;
	private const int SCR_HEIGHT = 600;

	private const string WINDOW_TITLE = "ProjSkiaSharp";

	public static int Main(string[] args)
	{
		// GLFW: Initialize and Configure
		GLFW.Init();

		// GLFW: Window Creation
		// GLFW.OpenWindowHint(GLFW.FSAA_SAMPLES, 8);
		GLFW.OpenWindow(SCR_WIDTH, SCR_HEIGHT, 8, 8, 8, 8, 24, 0, GLFW.WINDOWED);
		GLFW.SetWindowTitle(WINDOW_TITLE);

		GLFW.SetWindowSizeCallback(WindowSizeCallback);
		GLFW.SwapInterval(false); // VSync

		Console.WriteLine("GLFW window created");

		float[] vertices =
		{
			-0.5f, 0.5f, // left top
			0.5f, 0.5f, // right top
			0.5f, -0.5f, // right bottom
			-0.5f, -0.5f, // left bottom
		};

		float[] uvs =
		{
			0.0f, 0.0f, // left top
			1.0f, 0.0f, // right top
			1.0f, 1.0f, // right bottom
			0.0f, 1.0f, // left bottom
		};

		// load and create a texture
		int[] texture = new int[1];
		GL.GenTextures(1, texture);
		GL.BindTexture(GL.TEXTURE_2D, texture[0]); // all upcoming GL_TEXTURE_2D operations now have effect on this texture object
		// set the texture wrapping parameters
		GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, GL.CLAMP_TO_EDGE_EXT);
		GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, GL.CLAMP_TO_EDGE_EXT);
		// set texture filtering parameters
		GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR);
		GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, GL.LINEAR);
		// load and generate the texture
		Bitmap bitmap = new("container.jpg");
		BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
		GL.TexImage2D(GL.TEXTURE_2D, 0, GL.RGBA, bitmap.Width, bitmap.Height, 0, GL.RGBA, GL.UNSIGNED_BYTE, data.Scan0);
		bitmap.UnlockBits(data);

		// Render Loop
		double lastTime = GLFW.GetTime();
		double fpsTimer = GLFW.GetTime();
		while (GLFW.GetWindowParam(GLFW.ACTIVE) == 1)
		{
			double deltaTime = GLFW.GetTime() - lastTime;
			lastTime = GLFW.GetTime();
			// Input
			ProcessInput();

			// Change Window Title to show FPS, every second
			if (GLFW.GetTime() - fpsTimer >= 1.0)
			{
				GLFW.SetWindowTitle($"{WINDOW_TITLE} - FPS: " + Math.Round(1.0 / deltaTime));
				fpsTimer = GLFW.GetTime();
			}

			// Render
			// Clear the screen
			GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
			GL.Clear(GL.COLOR_BUFFER_BIT);

			// Draw our textured square
			// GL.BlendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
			GL.Color4ub(255, 0, 255, 255);
			GL.BindTexture(GL.TEXTURE_2D, texture[0]);
			GL.EnableClientState(GL.TEXTURE_COORD_ARRAY);
			GL.EnableClientState(GL.VERTEX_ARRAY);
			GL.TexCoordPointer(2, GL.FLOAT, 0, uvs);
			GL.VertexPointer(2, GL.FLOAT, 0, vertices);
			GL.DrawArrays(GL.QUADS, 0, 4);
			GL.DisableClientState(GL.VERTEX_ARRAY);
			GL.DisableClientState(GL.TEXTURE_COORD_ARRAY);

			// GLFW: Swap Buffers and Poll IO Events (keys pressed/released, mouse moved etc.)
			GLFW.SwapBuffers();
			GLFW.PollEvents();
		}

		GLFW.Terminate();
		return 0;
	}

	private static void ProcessInput()
	{
		if (GLFW.GetKey(Key.ESCAPE))
			GLFW.CloseWindow();
	}

	private static void WindowSizeCallback(int newWidth, int newHeight)
	{
		GL.Viewport(0, 0, newWidth, newHeight);
		// GL.Enable(GL.MULTISAMPLE);
		// GL.Enable(GL.TEXTURE_2D);
		// GL.Enable(GL.BLEND);
		// GL.BlendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
		// GL.Hint(GL.PERSPECTIVE_CORRECTION, GL.FASTEST);
		// //GL.Enable(GL.POLYGON_SMOOTH);
		// GL.ClearColor(0.0f, 1.0f, 0.0f, 0.0f);
	}
}
