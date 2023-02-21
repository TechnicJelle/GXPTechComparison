using static Arqan.GL;
using static Arqan.GLFW;

namespace ProjSkiaSharp;

class Program
{
	// Settings
	private const int SCR_WIDTH = 800;
	private const int SCR_HEIGHT = 600;

	private static int Main(string[] args)
	{
		// GLFW: Initialize and Configure
		glfwInit();
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);

		// GLFW: Window Creation
		nint window = glfwCreateWindow(SCR_WIDTH, SCR_HEIGHT, "ProjSkiaSharp"u8.ToArray(), 0, 0);
		if (window == 0)
		{
			Console.WriteLine("Failed to create GLFW window");
			glfwTerminate();
			return -1;
		}

		glfwMakeContextCurrent(window);
		glfwSetFramebufferSizeCallback(window, FramebufferSizeCallback);
		glfwSwapInterval(0); // Disable VSync

		Console.WriteLine("GLFW window created");

		// Render Loop
		double lastTime = glfwGetTime();
		while (glfwWindowShouldClose(window) != 1)
		{
			double deltaTime = glfwGetTime() - lastTime;
			lastTime = glfwGetTime();
			// Input
			ProcessInput(window);

			// Get Framerate
			Console.WriteLine("FPS: " +  1.0 / deltaTime);

			// Render
			glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
			glClear(GL_COLOR_BUFFER_BIT);

			// GLFW: Swap Buffers and Poll IO Events (keys pressed/released, mouse moved etc.)
			glfwSwapBuffers(window);
			glfwPollEvents();
		}

		glfwTerminate();
		return 0;
	}

	private static void ProcessInput(nint window)
	{
		if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
			glfwSetWindowShouldClose(window, 1);
	}

	private static void FramebufferSizeCallback(nint window, int width, int height)
	{
		glViewport(0, 0, width, height);
	}

}
