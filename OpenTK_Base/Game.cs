using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Program
{
    public class Game : GameWindow
    {

        uint[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        const int FPS = 1000;

        int VertexDataBufferObject;
        int ElementBufferObject;
        int VertexArrayObject;
        
        Shader shader;


        public Game(int width, int height, string title) : base(
            GameWindowSettings.Default,
            new NativeWindowSettings() { Size = (width, height), Title = title }
            )
        {
            shader = new Shader("shader.vert", "shader.frag");

            this.UpdateFrequency = FPS;
        }

        

        protected override void OnLoad()
        {
            base.OnLoad();

            // Set the background color
            GL.ClearColor(0.0f, 0.0f, 0.1f, 1.0f);

            // Load the shader
            float[] vertices = MakeSquare(-0.5f, 0.5f, 1.0f, 1.0f);
            
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            // Create and bind buffer for vertex data
            VertexDataBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexDataBufferObject);

            // Now that it's bound, load with data
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                vertices.Length * sizeof(float),
                vertices,
                BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
        }

        

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            //vec4(0.992f, 0.964f, 0.89f, 1.0f)

            


            
            shader.Use();
            int vertexColorLocation = GL.GetUniformLocation(shader.Handle, "ourColor");
            


            GL.Uniform4(vertexColorLocation, 0.992f, 0.974f, 0.89f, 1.0f);

            GL.BindVertexArray(VertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            // GL.DrawArrays(PrimitiveType.Triangles, 0, 3);


            // Last
            SwapBuffers();
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            shader.Dispose();
            // shader2.Dispose();
        }



        public float[] MakeSquare(float origin_x, float origin_y, float x_len, float y_len)
        {
            float[] vert =
            {
                origin_x + x_len, origin_y, 0.0f,  // top right
                origin_x + x_len, origin_y - y_len, 0.0f,  // bottom right
                origin_x, origin_y - y_len, 0.0f,  // bottom left
                origin_x, origin_y, 0.0f   // top left
            };

            return vert;
        }
    }

    
}