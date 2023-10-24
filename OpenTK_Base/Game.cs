using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Program
{
    public class Game : GameWindow
    {

        

        uint[] tri_indices =
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

            float[] square_size = { -0.5f, 0.5f, 1.0f, 1.0f };

            // Load the shader
            MakeSquare(square_size, ref VertexArrayObject, ref VertexDataBufferObject, ref ElementBufferObject);
            
           
        }

        public static float rotationX = 0.0f;
        public static float rotationY = 0.0f;
        

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

 
            shader.Use();

            int vertexColorLocation = GL.GetUniformLocation(shader.Handle, "ourColor");
            GL.Uniform4(vertexColorLocation, 0.992f, 0.974f, 0.89f, 1.0f);

            Matrix4 model = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotationX));
            model *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotationY));

            Matrix4 view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Size.X / (float) Size.Y, 0.1f, 100.0f);

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);


            DrawSquare(ref VertexArrayObject);

            // GL.DrawArrays(PrimitiveType.Triangles, 0, 3);


            // Last
            SwapBuffers();
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            KeyboardState input = KeyboardState;

            float amount = 0.2f;

            if (input.IsKeyDown(Keys.Right))
            {
                rotationY += amount;

                if (rotationY >= 360.0f) rotationY = 0.0f;
            }
            if (input.IsKeyDown(Keys.Left))
            {
                rotationY -= amount;

                if (rotationY <= 0.0f) rotationY = 360.0f;
            }

            if (input.IsKeyDown(Keys.Up))
            {
                rotationX += amount;

                if (rotationX >= 360.0f) rotationX = 0.0f;
            }
            if (input.IsKeyDown(Keys.Down))
            {
                rotationX -= amount;

                if (rotationX <= 0.0f) rotationX = 360.0f;
            }
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

        

        public void MakeSquare(float[] square_size, ref int vertex_array_object, ref int vertex_buffer_object, ref int element_buffer_object)
        {
            // square_size( origin_x origin_y x_len y_len

            float[] vert =
            {
                square_size[0] + square_size[2], square_size[1], 0.0f,  // top right
                square_size[0] + square_size[2], square_size[1] - square_size[3], 0.0f,  // bottom right
                square_size[0], square_size[1] - square_size[3], 0.0f,  // bottom left
                square_size[0], square_size[1], 0.0f   // top left
            };

            uint[] indices =
            {
                0, 1, 3,
                1, 2, 3
            };

            vertex_array_object = GL.GenVertexArray();
            GL.BindVertexArray(vertex_array_object);

            // Create and bind buffer for vertex data
            vertex_buffer_object = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer_object);

            // Now that it's bound, load with data
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                vert.Length * sizeof(float),
                vert,
                BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            element_buffer_object = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, element_buffer_object);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
        }

        public void DrawSquare(ref int vertex_array_object)
        {
            GL.BindVertexArray(vertex_array_object);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }
    }

    
}