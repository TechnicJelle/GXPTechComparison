using System;
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
