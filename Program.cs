using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using System;

class Program
{
    private static GameWindow gameWindow;
    private static int _vertexArray;
    private static int _vertexBuffer;
    private static int _shaderProgram;

    static void Main(string[] args)
    {
        var gameWindowSettings = GameWindowSettings.Default;
        var nativeWindowSettings = new NativeWindowSettings()
        {
            Title = "Grafico U",
            Size = new Vector2i(800, 600),
        };

        gameWindow = new GameWindow(gameWindowSettings, nativeWindowSettings);

        // Eventos de OpenGL
        gameWindow.Load += Load;
        gameWindow.RenderFrame += RenderFrame;
        gameWindow.Run();
    }

    static void Load()
    {
        GL.ClearColor(Color4.CornflowerBlue); // Fondo azul

        // Crear el shader
        _shaderProgram = CreateShaderProgram();

        // Coordenadas de los rectángulos que formarán la "U"
        float[] vertices = {
            // Lado izquierdo (rectángulo vertical)
            -0.6f,  0.5f, 0.0f,  
            -0.5f,  0.5f, 0.0f,  
            -0.6f, -0.5f, 0.0f,  
            -0.6f, -0.5f, 0.0f,  
            -0.5f,  0.5f, 0.0f,  
            -0.5f, -0.5f, 0.0f,  

            // Lado derecho (rectángulo vertical)
            0.5f,  0.5f, 0.0f,  
            0.6f,  0.5f, 0.0f,  
            0.5f, -0.5f, 0.0f,  
            0.5f, -0.5f, 0.0f,  
            0.6f,  0.5f, 0.0f,  
            0.6f, -0.5f, 0.0f,  

            // Base de la "U" (rectángulo horizontal)
            -0.6f, -0.5f, 0.0f,  
            0.6f, -0.5f, 0.0f,  
            -0.6f, -0.6f, 0.0f,  
            -0.6f, -0.6f, 0.0f,  
            0.6f, -0.5f, 0.0f,  
            0.6f, -0.6f, 0.0f   
        };

        // Generar el VAO y el VBO
        _vertexArray = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArray);

        _vertexBuffer = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        // Configurar el buffer de vértices
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    static void RenderFrame(FrameEventArgs args)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);

        GL.UseProgram(_shaderProgram);
        GL.BindVertexArray(_vertexArray);

        // Dibujar los rectángulos como triángulos
        GL.DrawArrays(PrimitiveType.Triangles, 0, 18);

        GL.BindVertexArray(0);
        gameWindow.SwapBuffers();
    }

    static int CreateShaderProgram()
    {
        string vertexShaderSource = @"
        #version 330 core
        layout(location = 0) in vec3 aPos;
        void main()
        {
            gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
        }";

        string fragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(0.0, 0.0, 0.0, 1.0); // Color negro
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

        GL.GetShaderInfoLog(shader, out string log);
        if (!string.IsNullOrEmpty(log))
        {
            Console.WriteLine($"Error compiling shader: {log}");
        }

        return shader;
    }
}
