using System.Text;
using SkiaSharp;
using static Arqan.GL;
using static Arqan.GLFW;

namespace ProjSkiaSharp;

static internal class Program
{
	// Settings
	private const int SCR_WIDTH = 800;
	private const int SCR_HEIGHT = 600;

	private const string WINDOW_TITLE = "ProjSkiaSharp";

	private const int GL_INFO_LOG_SIZE = 512;

	private static nint _window;
	private static uint _shaderProgram;

	private static readonly float[] Vertices =
	{
		// positions      // colors        // texture coords
		0.5f, 0.5f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, // right top
		0.5f, -0.5f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, // right bottom
		-0.5f, -0.5f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, // left bottom
		-0.5f, 0.5f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f, 1.0f, // left top
	};

	private static readonly uint[] Indices =
	{
		0, 1, 3, // first triangle
		1, 2, 3, // second triangle
	};

	// Vertex Buffer Object
	private static readonly uint[] VBO = new uint[1];
	private static readonly uint[] VAO = new uint[1];
	private static readonly uint[] EBO = new uint[1];

	private static readonly uint[] Texture = new uint[1];

	private static int Main(string[] args)
	{
		if (!CreateWindow()) return -1;

		SetupShaders();

		SetupVertices();

		CreateGLTexture();

		Run();

		Close();

		return 0;
	}

	private static bool CreateWindow()
	{
		// Initialisation
		int result = glfwInit();
		if (result == 0)
		{
			Console.WriteLine("Failed to initialize GLFW");
			return false;
		}

		// Configuration
		// > OpenGL 3.3 Core
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		// > Multisampling
		glfwWindowHint(GLFW_SAMPLES, 4);

		// Window Creation
		_window = glfwCreateWindow(SCR_WIDTH, SCR_HEIGHT, Encoding.UTF8.GetBytes(WINDOW_TITLE), 0, 0);
		if (_window == 0)
		{
			Console.WriteLine("Failed to create GLFW window");
			glfwTerminate();
			return false;
		}
		glfwMakeContextCurrent(_window);

		// VSync
		glfwSwapInterval(0);

		// Inputs
		glfwSetKeyCallback(_window, (IntPtr window, int key, int _, int action, int _) =>
		{
			if (action == GLFW_PRESS && key == GLFW_KEY_ESCAPE)
				glfwSetWindowShouldClose(window, 1);
		});

		glfwSetWindowSizeCallback(_window, WindowSizeCallback);
		void WindowSizeCallback(IntPtr _, int newWidth, int newHeight)
		{
			Console.WriteLine($"Window resized to {newWidth}x{newHeight}");
			glViewport(0, 0, newWidth, newHeight);
			glEnable(GL_MULTISAMPLE);
			glEnable(GL_TEXTURE_2D);
			glEnable(GL_BLEND);
			glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
			glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
		}
		//Apparently this isn't called by default anymore, so we need to call it manually
		WindowSizeCallback(_window, SCR_WIDTH, SCR_HEIGHT);

		Console.WriteLine("GLFW window created");
		return true;
	}

	private static void SetupShaders()
	{
		// build and compile our shader program
		_shaderProgram = CreateShaderProgram("shader.vert", "shader.frag");
	}

	private static uint CreateShaderProgram(string vertex, string fragment)
	{
		// Build and Compile our Shader Program
		const string prepend = "assets/shaders/";
		string vertexShaderSource = File.ReadAllText(prepend + vertex);
		string fragmentShaderSource = File.ReadAllText(prepend + fragment);

		// vertex shader
		uint vertexShader = glCreateShader(GL_VERTEX_SHADER);
		glShaderSource(vertexShader, 1, new[] {vertexShaderSource}, null);
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
		glShaderSource(fragmentShader, 1, new[] {fragmentShaderSource}, null);
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

	private static void SetupVertices()
	{
		// Set up Vertex Data (and Buffer(s)) and Configure Vertex Attributes
		glGenVertexArrays(1, VAO);
		glGenBuffers(1, VBO);
		glGenBuffers(1, EBO);

		// bind the Vertex Array Object first, then bind and set vertex buffer(s), and then configure vertex attributes(s).
		glBindVertexArray(VAO[0]);

		glBindBuffer(GL_ARRAY_BUFFER, VBO[0]);
		glBufferData(GL_ARRAY_BUFFER, Vertices.Length * sizeof(float), Vertices, GL_STATIC_DRAW);

		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO[0]);
		glBufferData(GL_ELEMENT_ARRAY_BUFFER, Indices.Length * sizeof(uint), Indices, GL_STATIC_DRAW);

		// position attribute
		glVertexAttribPointer(0, 3, GL_FLOAT, false, 8 * sizeof(float), 0);
		glEnableVertexAttribArray(0);
		// color attribute
		glVertexAttribPointer(1, 3, GL_FLOAT, false, 8 * sizeof(float), 3 * sizeof(float));
		glEnableVertexAttribArray(1);
		// texture coord attribute
		glVertexAttribPointer(2, 2, GL_FLOAT, false, 8 * sizeof(float), 6 * sizeof(float));
		glEnableVertexAttribArray(2);
	}

	private static void CreateGLTexture()
	{
		glGenTextures(1, Texture);

		glBindTexture(GL_TEXTURE_2D, Texture[0]); // all upcoming GL_TEXTURE_2D operations now have effect on this texture object

		// set texture filtering parameters (not PixelArt)
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

		// set the texture wrapping parameters
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);

		UpdateGLTexture();

		glBindTexture(GL_TEXTURE_2D, 0); //unbind texture
	}

	private static void UpdateGLTexture()
	{
		SKBitmap bitmap = BitmapFlipped(SKBitmap.Decode("assets/textures/container.jpg"));

		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, bitmap.Width, bitmap.Height, 0, GL_BGRA, GL_UNSIGNED_BYTE, bitmap.GetPixels());
	}

	private static SKBitmap BitmapFlipped(SKBitmap bitmap)
	{
		SKBitmap flipped = new(bitmap.Height, bitmap.Width);
		using SKCanvas canvas = new(flipped);
		canvas.Scale(1, -1, 0, bitmap.Height / 2.0f);
		canvas.DrawBitmap(bitmap, 0, 0);
		return flipped;
	}

	private static void Run()
	{
		glfwSetTime(0.0);

		double lastTime = glfwGetTime();
		double fpsTimer = glfwGetTime();
		do
		{
			double deltaTime = glfwGetTime() - lastTime;
			lastTime = glfwGetTime();

			// Change Window Title to show FPS, every second
			if (glfwGetTime() - fpsTimer >= 1.0)
			{
				glfwSetWindowTitle(_window, $"{WINDOW_TITLE} - FPS: " + Math.Round(1.0 / deltaTime));
				fpsTimer = glfwGetTime();
			}

			Display();

			glfwPollEvents();
		} while(glfwWindowShouldClose(_window) != 1);
	}

	private static void Display()
	{
		// Clear the screen
		glClear(GL_COLOR_BUFFER_BIT);

		RenderSelf();

		// GLFW: Swap Buffers and Poll IO Events (keys pressed/released, mouse moved etc.)
		glfwSwapBuffers(_window);
	}

	private static void RenderSelf()
	{
		glBindTexture(GL_TEXTURE_2D, Texture[0]);
		DrawQuad();
		glBindTexture(GL_TEXTURE_2D, 0);
	}

	private static void DrawQuad()
	{
		// Draw our textured square
		glUseProgram(_shaderProgram);
		glBindVertexArray(VAO[0]);
		glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);
		glBindVertexArray(0);
	}

	private static void Close()
	{
		// Deallocate
		glDeleteVertexArrays(1, VAO);
		glDeleteBuffers(1, VBO);
		glDeleteProgram(_shaderProgram);
		glDeleteTextures(1, Texture);

		glfwTerminate();
	}
}
