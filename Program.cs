using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using System;

class Program
{
    private static GameWindow gameWindow;
    private static int _vertexArray;
    private static int _vertexBuffer;
    private static int _shaderProgram;
    private static int _axisVBO, _axisVAO;

    //punto del centro relativo (X,Y,Z)
    private static Vector3 _center = new Vector3(-0.5f, 0.5f, 0.0f);
    

    private static Vector2 _lastMousePos;
    private static bool _mousePressed;
    private static Matrix4 _model = Matrix4.Identity;

    static void Main(string[] args)
    {
        var gameWindowSettings = GameWindowSettings.Default;
        var nativeWindowSettings = new NativeWindowSettings()
        {
            Title = "Grafico U",
            Size = new Vector2i(1600, 800),
        };

        gameWindow = new GameWindow(gameWindowSettings, nativeWindowSettings);

        gameWindow.Load += Load;
        gameWindow.RenderFrame += RenderFrame;
        gameWindow.MouseMove += MouseMove;
        gameWindow.MouseDown += MouseDown;
        gameWindow.MouseUp += MouseUp;
        gameWindow.Run();
    }

    static void Load()
    {
        GL.ClearColor(Color4.White);
        _shaderProgram = CreateShaderProgram();

        float[] vertices = GenerateUVertices(_center);
        _vertexArray = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArray);
        _vertexBuffer = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        float[] axisVertices =
        {
            -1.0f, 0.0f, 0.0f,  1.0f, 0.0f, 0.0f,  // X-axis
            0.0f, -1.0f, 0.0f,  0.0f, 1.0f, 0.0f,  // Y-axis
            0.0f, 0.0f, -1.0f,  0.0f, 0.0f, 1.0f   // Z-axis
        };

        _axisVAO = GL.GenVertexArray();
        GL.BindVertexArray(_axisVAO);
        _axisVBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _axisVBO);
        GL.BufferData(BufferTarget.ArrayBuffer, axisVertices.Length * sizeof(float), axisVertices, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    static void RenderFrame(FrameEventArgs args)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.UseProgram(_shaderProgram);
        int modelLoc = GL.GetUniformLocation(_shaderProgram, "model");
        GL.UniformMatrix4(modelLoc, false, ref _model);

        GL.BindVertexArray(_vertexArray);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 18);

        GL.BindVertexArray(_axisVAO);
        GL.DrawArrays(PrimitiveType.Lines, 0, 6);

        GL.BindVertexArray(0);
        gameWindow.SwapBuffers();
    }

    static void MouseDown(MouseButtonEventArgs e)
    {
        if (e.Button == MouseButton.Left)
        {
            _mousePressed = true;
            _lastMousePos = gameWindow.MousePosition;
        }
    }

    static void MouseUp(MouseButtonEventArgs e)
    {
        if (e.Button == MouseButton.Left)
        {
            _mousePressed = false;
        }
    }

    static void MouseMove(MouseMoveEventArgs e)
    {
        if (_mousePressed)
        {
            var delta = e.Position - _lastMousePos;
            _lastMousePos = e.Position;
            float sensitivity = 0.005f;
            _model *= Matrix4.CreateRotationY(delta.X * sensitivity);
            _model *= Matrix4.CreateRotationX(delta.Y * sensitivity);
        }
    }

    static int CreateShaderProgram()
    {
        string vertexShaderSource = @"#version 330 core
        layout(location = 0) in vec3 aPos;
        uniform mat4 model;
        void main()
        {
            gl_Position = model * vec4(aPos, 1.0);
        }";

        string fragmentShaderSource = @"#version 330 core
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(0.0, 0.0, 0.0, 1.0);
        }";

        int vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);
        int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);
        int shaderProgram = GL.CreateProgram();
        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);
        GL.LinkProgram(shaderProgram);
        GL.ValidateProgram(shaderProgram);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
        return shaderProgram;
    }

    static int CompileShader(ShaderType type, string source)
    {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);
        return shader;
    }

    static float[] GenerateUVertices(Vector3 center)
    {
        return new float[]
        {
            center.X - 0.3f, center.Y + 0.3f, center.Z,  
            center.X - 0.2f, center.Y + 0.3f, center.Z,  
            center.X - 0.3f, center.Y - 0.3f, center.Z,  
            center.X - 0.3f, center.Y - 0.3f, center.Z,  
            center.X - 0.2f, center.Y + 0.3f, center.Z,  
            center.X - 0.2f, center.Y - 0.3f, center.Z,  

            center.X + 0.2f, center.Y + 0.3f, center.Z,  
            center.X + 0.3f, center.Y + 0.3f, center.Z,  
            center.X + 0.2f, center.Y - 0.3f, center.Z,  
            center.X + 0.2f, center.Y - 0.3f, center.Z,  
            center.X + 0.3f, center.Y + 0.3f, center.Z,  
            center.X + 0.3f, center.Y - 0.3f, center.Z,  

            center.X - 0.3f, center.Y - 0.3f, center.Z,  
            center.X + 0.3f, center.Y - 0.3f, center.Z,  
            center.X - 0.3f, center.Y - 0.4f, center.Z,  
            center.X - 0.3f, center.Y - 0.4f, center.Z,  
            center.X + 0.3f, center.Y - 0.3f, center.Z,  
            center.X + 0.3f, center.Y - 0.4f, center.Z 
        };
    }
}
