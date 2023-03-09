using System.Reflection;
using System.Text;
using SkiaSharp;
using static Arqan.GL;
using static Arqan.GLFW;

namespace ProjSkiaSharp;

internal static class Program
{
	// Settings
	private const int SCR_WIDTH = 800;
	private const int SCR_HEIGHT = 600;

	private const string WINDOW_TITLE = "ProjSkiaSharp";

	private const int GL_INFO_LOG_SIZE = 512;

	private static int Main(string[] args)
	{
		// GLFW: Initialize and Configure
		glfwInit();
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);

		// GLFW: Window Creation
		nint window = glfwCreateWindow(SCR_WIDTH, SCR_HEIGHT, Encoding.UTF8.GetBytes(WINDOW_TITLE), 0, 0);
		if (window == 0)
		{
			Console.WriteLine("Failed to create GLFW window");
			glfwTerminate();
			return -1;
		}

		glfwMakeContextCurrent(window);
		glfwSetFramebufferSizeCallback(window, FramebufferSizeCallback);
		glfwSwapInterval(0); // VSync

		Console.WriteLine("GLFW window created");

		// build and compile our shader program
		uint shaderProgram = ShaderProgram("shader.vert", "shader.frag");

		// Set up Vertex Data (and Buffer(s)) and Configure Vertex Attributes
		float[] vertices =
		{
			// positions      // colors        // texture coords
			0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, // top right
			0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, // bottom right
			-0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, // bottom left
			-0.5f, 0.5f, 0.0f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f, // top left
		};

		uint[] indices =
		{
			0, 1, 3, // first triangle
			1, 2, 3, // second triangle
		};

		// Vertex Buffer Object
		uint[] vbo = new uint[1];
		uint[] vao = new uint[1];
		uint[] ebo = new uint[1];
		glGenVertexArrays(1, vao);
		glGenBuffers(1, vbo);
		glGenBuffers(1, ebo);

		// bind the Vertex Array Object first, then bind and set vertex buffer(s), and then configure vertex attributes(s).
		glBindVertexArray(vao[0]);

		glBindBuffer(GL_ARRAY_BUFFER, vbo[0]);
		glBufferData(GL_ARRAY_BUFFER, vertices.Length * sizeof(float), vertices, GL_STATIC_DRAW);

		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo[0]);
		glBufferData(GL_ELEMENT_ARRAY_BUFFER, indices.Length * sizeof(uint), indices, GL_STATIC_DRAW);

		// position attribute
		glVertexAttribPointer(0, 3, GL_FLOAT, false, 8 * sizeof(float), 0);
		glEnableVertexAttribArray(0);
		// color attribute
		glVertexAttribPointer(1, 3, GL_FLOAT, false, 8 * sizeof(float), 3 * sizeof(float));
		glEnableVertexAttribArray(1);
		// texture coord attribute
		glVertexAttribPointer(2, 2, GL_FLOAT, false, 8 * sizeof(float), 6 * sizeof(float));
		glEnableVertexAttribArray(2);

		// load and create a texture
		uint[] texture = new uint[1];
		glGenTextures(1, texture);
		glBindTexture(GL_TEXTURE_2D, texture[0]); // all upcoming GL_TEXTURE_2D operations now have effect on this texture object
		// set the texture wrapping parameters
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
		// set texture filtering parameters
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
		// load and generate the texture
		SKBitmap bitmap = ReadResourceImage("container.jpg");
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, bitmap.Width, bitmap.Height, 0, GL_RGBA, GL_UNSIGNED_BYTE, bitmap.GetPixels());
		glGenerateMipmap(GL_TEXTURE_2D);

		// Render Loop
		double lastTime = glfwGetTime();
		double fpsTimer = glfwGetTime();
		while (glfwWindowShouldClose(window) != 1)
		{
			double deltaTime = glfwGetTime() - lastTime;
			lastTime = glfwGetTime();
			// Input
			ProcessInput(window);

			// Change Window Title to show FPS, every second
			if (glfwGetTime() - fpsTimer >= 1.0)
			{
				glfwSetWindowTitle(window, $"{WINDOW_TITLE} - FPS: " + Math.Round(1.0 / deltaTime));
				fpsTimer = glfwGetTime();
			}

			// Render
			// Clear the screen
			glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
			glClear(GL_COLOR_BUFFER_BIT);

			// Draw our textured square
			glUseProgram(shaderProgram);
			glBindVertexArray(vao[0]);
			// glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
			glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);
			glBindVertexArray(0);

			// GLFW: Swap Buffers and Poll IO Events (keys pressed/released, mouse moved etc.)
			glfwSwapBuffers(window);
			glfwPollEvents();
		}

		// Deallocate
		glDeleteVertexArrays(1, vao);
		glDeleteBuffers(1, vbo);
		glDeleteProgram(shaderProgram);

		glfwTerminate();
		return 0;
	}

	private static void ProcessInput(nint window)
	{
		if (glfwGetKey(window, GLFW_KEY_ESCAPE) == GLFW_PRESS)
			glfwSetWindowShouldClose(window, 1);
	}

	private static void FramebufferSizeCallback(nint window, int newWidth, int newHeight)
	{
		glViewport(0, 0, newWidth, newHeight);
	}

	/// <exception cref="FileNotFoundException"></exception>
	private static string ReadResourceText(string name)
	{
		Stream stream = ReadResourceStream(name);
		using StreamReader reader = new(stream);
		return reader.ReadToEnd();
	}

	/// <exception cref="FileNotFoundException"></exception>
	private static SKBitmap ReadResourceImage(string name)
	{
		Stream stream = ReadResourceStream(name);
		return SKBitmap.Decode(stream);
	}

	/// <exception cref="FileNotFoundException"></exception>
	private static Stream ReadResourceStream(string name)
	{
		Assembly assembly = Assembly.GetExecutingAssembly();
		string? resourcePath = assembly.GetManifestResourceNames()
			.SingleOrDefault(str => str.EndsWith(name));
		if (resourcePath == null) throw new FileNotFoundException(name);
		return assembly.GetManifestResourceStream(resourcePath) ?? throw new FileNotFoundException(resourcePath);
	}

	private static uint ShaderProgram(string vertex, string fragment)
	{
		// Build and Compile our Shader Program
		string vertexShaderSource = ReadResourceText(vertex);
		string fragmentShaderSource = ReadResourceText(fragment);
		// vertex shader
		uint vertexShader = glCreateShader(GL_VERTEX_SHADER);
		glShaderSource(vertexShader, 1, new[] {vertexShaderSource,}, null);
		glCompileShader(vertexShader);
		// check for shader compile errors
		int success = 0;
		glGetShaderiv(vertexShader, GL_COMPILE_STATUS, ref success);
		if (success == 0)
		{
			int logLength = -1;
			byte[] infoLog = new byte[GL_INFO_LOG_SIZE];
			glGetShaderInfoLog(vertexShader, GL_INFO_LOG_SIZE, ref logLength, infoLog);
			Console.WriteLine("Vertex Shader Compilation Failed:\n" + Encoding.UTF8.GetString(infoLog[..logLength]));
		}

		// fragment shader
		uint fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
		glShaderSource(fragmentShader, 1, new[] {fragmentShaderSource,}, null);
		glCompileShader(fragmentShader);
		// check for shader compile errors
		glGetShaderiv(fragmentShader, GL_COMPILE_STATUS, ref success);
		if (success == 0)
		{
			int logLength = -1;
			byte[] infoLog = new byte[GL_INFO_LOG_SIZE];
			glGetShaderInfoLog(fragmentShader, GL_INFO_LOG_SIZE, ref logLength, infoLog);
			Console.WriteLine("Fragment Shader Compilation Failed:\n" + Encoding.UTF8.GetString(infoLog[..logLength]));
		}

		// link shaders
		uint shaderProgram = glCreateProgram();
		glAttachShader(shaderProgram, vertexShader);
		glAttachShader(shaderProgram, fragmentShader);
		glLinkProgram(shaderProgram);
		// check for linking errors
		int[] success2 = new int[1];
		glGetProgramiv(shaderProgram, GL_LINK_STATUS, success2);
		if (success2[0] == 0)
		{
			int logLength = -1;
			byte[] infoLog = new byte[GL_INFO_LOG_SIZE];
			glGetProgramInfoLog(shaderProgram, GL_INFO_LOG_SIZE, ref logLength, infoLog);
			Console.WriteLine("Shader Program Linking Failed:\n" + Encoding.UTF8.GetString(infoLog[..logLength]));
		}

		glUseProgram(shaderProgram);

		glDeleteShader(vertexShader);
		glDeleteShader(fragmentShader);
		return shaderProgram;
	}
}
