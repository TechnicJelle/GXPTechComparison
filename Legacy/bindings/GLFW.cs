using System.Runtime.InteropServices;
using System.Security;
// ReSharper disable UnusedMember.Global

namespace Legacy.bindings;

public static class GLFW
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[SuppressUnmanagedCodeSecurity]
	public delegate void KeyCallback(int key, int action);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[SuppressUnmanagedCodeSecurity]
	public delegate void MouseButtonCallback(int button, int action);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[SuppressUnmanagedCodeSecurity]
	public delegate void WindowSizeCallback(int width, int height);

	public const int OPENED			= 0x00020001;
	public const int WINDOWED		= 0x00010001;
	public const int FULLSCREEN		= 0x00010002;
	public const int ACTIVE			= 0x00020001;
	public const int FSAA_SAMPLES	= 0x0002100E;
	public const int MOUSE_CURSOR	= 0x00030001;

	// Uses the GLFW.dll from the lib folder

	[DllImport("lib/glfw.dll", EntryPoint = "glfwSetTime")]
	public static extern void SetTime(double time);

	[DllImport("lib/glfw.dll", EntryPoint = "glfwGetTime")]
	public static extern double GetTime();

	[DllImport("lib/glfw.dll", EntryPoint = "glfwPollEvents")]
	public static extern void PollEvents();

	[DllImport("lib/glfw.dll", EntryPoint = "glfwGetWindowParam")]
	public static extern int GetWindowParam(int param);

	[DllImport("lib/glfw.dll", EntryPoint = "glfwInit")]
	public static extern void Init();

	[DllImport("lib/glfw.dll", EntryPoint = "glfwOpenWindow")]
	public static extern void OpenWindow(int width, int height, int r, int g, int b, int a, int depth, int stencil, int mode);

	[DllImport("lib/glfw.dll", EntryPoint = "glfwSetWindowTitle")]
	public static extern void SetWindowTitle(string title);

	[DllImport("lib/glfw.dll", EntryPoint = "glfwSwapInterval")]
	public static extern void SwapInterval(bool mode);

	[DllImport("lib/glfw.dll", EntryPoint = "glfwSetWindowSizeCallback")]
	public static extern void SetWindowSizeCallback(WindowSizeCallback callback);

	[DllImport("lib/glfw.dll", EntryPoint = "glfwCloseWindow")]
	public static extern void CloseWindow();

	[DllImport("lib/glfw.dll", EntryPoint = "glfwTerminate")]
	public static extern void Terminate();

	[DllImport("lib/glfw.dll", EntryPoint = "glfwSwapBuffers")]
	public static extern void SwapBuffers();

	[DllImport("lib/glfw.dll", EntryPoint = "glfwGetKey")]
	public static extern bool GetKey(int key);

	[DllImport("lib/glfw.dll", EntryPoint = "glfwSetKeyCallback")]
	public static extern void SetKeyCallback(KeyCallback callback);

	[DllImport("lib/glfw.dll", EntryPoint = "glfwOpenWindowHint")]
	public static extern void OpenWindowHint(int name, int value);

	[DllImport("lib/glfw.dll", EntryPoint = "glfwGetMousePos")]
	public static extern bool GetMousePos(out int x, out int y);

	[DllImport("lib/glfw.dll", EntryPoint = "glfwSetMouseButtonCallback")]
	public static extern void SetMouseButtonCallback(MouseButtonCallback callback);

	[DllImport("lib/glfw.dll", EntryPoint = "glfwEnable")]
	public static extern void Enable(int property);

	[DllImport("lib/glfw.dll", EntryPoint = "glfwDisable")]
	public static extern void Disable(int property);
}
