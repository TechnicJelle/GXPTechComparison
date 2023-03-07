﻿using System.Text;
using static Arqan.GL;
using static Arqan.GLFW;

namespace ProjSkiaSharp;

internal static class Program
{
	// Settings
	private const int SCR_WIDTH = 800;
	private const int SCR_HEIGHT = 600;


	private const int INFO_LOG_SIZE = 512;


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
		glfwSwapInterval(1); // VSync

		Console.WriteLine("GLFW window created");

		// Build and Compile our Shader Program
		string vertexShaderSource = File.ReadAllText("shader.vert");
		string fragmentShaderSource = File.ReadAllText("shader.frag");
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
			byte[] infoLog = new byte[INFO_LOG_SIZE];
			glGetShaderInfoLog(vertexShader, INFO_LOG_SIZE, ref logLength, infoLog);
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
			byte[] infoLog = new byte[INFO_LOG_SIZE];
			glGetShaderInfoLog(fragmentShader, INFO_LOG_SIZE, ref logLength, infoLog);
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
			byte[] infoLog = new byte[INFO_LOG_SIZE];
			glGetProgramInfoLog(shaderProgram, INFO_LOG_SIZE, ref logLength, infoLog);
			Console.WriteLine("Shader Program Linking Failed:\n" + Encoding.UTF8.GetString(infoLog[..logLength]));
		}

		glUseProgram(shaderProgram);

		glDeleteShader(vertexShader);
		glDeleteShader(fragmentShader);

		// Set up Vertex Data (and Buffer(s)) and Configure Vertex Attributes
		float[] vertices =
		{
			0.5f, 0.5f, 0.0f, // top right
			0.5f, -0.5f, 0.0f, // bottom right
			-0.5f, -0.5f, 0.0f, // bottom left
			-0.5f, 0.5f, 0.0f, // top left
		};

		uint[] indices =
		{
			// note that we start from 0!
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

		glVertexAttribPointer(0, 3, GL_FLOAT, false, 3 * sizeof(float), 0);
		glEnableVertexAttribArray(0);

		// bind the EBO
		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo[0]);
		glBufferData(GL_ELEMENT_ARRAY_BUFFER, indices.Length * sizeof(uint), indices, GL_STATIC_DRAW);

		// note that this is allowed, the call to glVertexAttribPointer registered VBO as the vertex attribute's bound vertex buffer object so afterwards we can safely unbind
		glBindBuffer(GL_ARRAY_BUFFER, 0);

		// unbind VAO
		glBindVertexArray(0);

		// Render Loop
		double lastTime = glfwGetTime();
		while (glfwWindowShouldClose(window) != 1)
		{
			double deltaTime = glfwGetTime() - lastTime;
			lastTime = glfwGetTime();
			// Input
			ProcessInput(window);

			// Get Framerate
			// Console.WriteLine("FPS: " + 1.0 / deltaTime);

			// Render
			// Clear the screen
			glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
			glClear(GL_COLOR_BUFFER_BIT);

			// Draw our first triangle
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

	private static void FramebufferSizeCallback(nint window, int width, int height)
	{
		glViewport(0, 0, width, height);
	}
}
