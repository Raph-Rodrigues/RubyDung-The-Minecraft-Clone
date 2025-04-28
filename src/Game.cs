using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RubyDung;

internal class Game : GameWindow
{
    // set of vertices to draw the triangle with (x,y,z) for each vertex
    float[] vertices =
    {
        0f, 0.5f, 0f, // top vertex
        -0.5f, -0.5f, 0f, // bottom left
        0.5f, -0.5f, 0f // bottom right
    };

    // Render Pipeline vars
    int vao;
    int shaderProgram;
    int vbo;

    
    int width, height;
    
    public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
        this.width = width;
        this.height = height;
        
        CenterWindow(new Vector2i(width, height));
    }
    
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0,0, e.Width, e.Height);
        this.width = e.Width;
        this.height = e.Height;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        // generate the vbo
        vao = GL.GenVertexArray();

        // generate a buffer
        vbo = GL.GenBuffer();
        // bind the buffer as an array buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        // Store data in the vbo
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length*sizeof(float), vertices, BufferUsageHint.StaticDraw);

        // bind the vao
        GL.BindVertexArray(vao);
        // point slot (0) of the VAO to the currently bound VBO (vbo)
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
        // enable the slot
        GL.EnableVertexArrayAttrib(vao, 0);

        // unbind the vbo and vao respectively
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        
        shaderProgram = GL.CreateProgram();

        
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        
        GL.ShaderSource(vertexShader, LoadShaderSource("Default.vert")); 
       
        GL.CompileShader(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, LoadShaderSource("Default.frag"));
        GL.CompileShader(fragmentShader);

        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);

        // Link the program to OpenGL
        GL.LinkProgram(shaderProgram);
        
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

    }
   
    protected override void OnUnload()
    {
        base.OnUnload();
        
        GL.DeleteVertexArray(vao);
        GL.DeleteBuffer(vbo);
        GL.DeleteProgram(shaderProgram);
    }
    
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        GL.ClearColor(0.6f, 0.3f, 1f, 1f);
        // Fill the screen with the color
        GL.Clear(ClearBufferMask.ColorBufferBit);


        // draw our triangle
        GL.UseProgram(shaderProgram); // bind vao
        GL.BindVertexArray(vao); // use shader program
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3); // draw the triangle | args = Primitive type, first vertex, last vertex


        // swap the buffers
        Context.SwapBuffers();

        base.OnRenderFrame(args);
    }
    
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        if (KeyboardState.IsKeyPressed(Keys.Escape))
        {
            Close();
        }
    }

    // Function to load a text file and return its contents as a string
    public static string LoadShaderSource(string filePath)
    {
        string shaderSource = "";

        try
        {
            using (StreamReader reader = new StreamReader("../../../src/Shaders/" + filePath))
            {
                shaderSource = reader.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to load shader source file: " + e.Message);
        }

        return shaderSource;
    }
}