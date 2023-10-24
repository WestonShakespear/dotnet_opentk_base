using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Input;
using OpenTK;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Program
{
    public class Game : GameWindow
    {

        static Vector3 WorldUp = Vector3.UnitY;

        // static Vector3 CameraPosition = new Vector3(0.0f, 0.0f, 3.0f);
        // static Vector3 CameraTarget = Vector3.Zero;
        // static Vector3 CameraDirection = Vector3.Normalize(CameraDirection - CameraTarget);
        // static Vector3 CameraRight = Vector3.Normalize(Vector3.Cross(WorldUp, CameraDirection));
        // static Vector3 CameraUp = Vector3.Cross(CameraDirection, CameraRight);

        // static Vector3 PlayerPosition = new Vector3(0.0f, 0.0f, 3.0f);
        // static Vector3 PlayerFront = new Vector3(0.0f, 0.0f, -1.0f);
        // static Vector3 PlayerUp = new Vector3(0.0f, 1.0f, 0.0f);

        Camera camera;

        const float PLAYER_SPEED = 0.5f;

        int WindowWidth;
        int WindowHeight;

        uint[] tri_indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        const int FPS = 1000;

        int VertexDataBufferObject;
        int ElementBufferObject;
        int VertexArrayObject;

        int VertexDataBufferObject2;
        int ElementBufferObject2;
        int VertexArrayObject2;
        
        Shader shader;


        public Game(int width, int height, string title) : base(
            GameWindowSettings.Default,
            new NativeWindowSettings() { Size = (width, height), Title = title }
            )
        {
            shader = new Shader("shader.vert", "shader.frag");

            this.UpdateFrequency = FPS;
            this.WindowWidth = width;
            this.WindowHeight = height;
            this.camera = new Camera(Vector3.UnitZ * 1, Size.X / (float)Size.Y);

        }

        

        protected override void OnLoad()
        {
            base.OnLoad();

            // Set the background color
            GL.ClearColor(0.0f, 0.0f, 0.1f, 1.0f);

            float[] square_size = { -0.5f, 0.5f, 1.0f, 1.0f };
            float[] square_size2 = { -1.0f, 1.0f, 0.5f, 0.5f };

            // Load the shader
            MakeSquare(square_size, ref VertexArrayObject, ref VertexDataBufferObject, ref ElementBufferObject);
            MakeSquare(square_size2, ref VertexArrayObject2, ref VertexDataBufferObject2, ref ElementBufferObject2);
            
           
        }



        private Vector2 ModelRotation = new Vector2(0.0f, 0.0f);
        

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

 
            shader.Use();

            int vertexColorLocation = GL.GetUniformLocation(shader.Handle, "ourColor");
            GL.Uniform4(vertexColorLocation, 0.992f, 0.974f, 0.89f, 1.0f);


            float x_rotation = MathHelper.DegreesToRadians(this.ModelRotation.X);
            float y_rotation = MathHelper.DegreesToRadians(this.ModelRotation.Y);

            Matrix4 model = Matrix4.CreateRotationX(x_rotation);
            model *= Matrix4.CreateRotationY(y_rotation);


            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", this.camera.GetViewMatrix());
            shader.SetMatrix4("projection", this.camera.GetProjectionMatrix());


            DrawSquare(ref VertexArrayObject);
            DrawSquare(ref VertexArrayObject2);

            // GL.DrawArrays(PrimitiveType.Triangles, 0, 3);


            // Last
            SwapBuffers();
        }

        private bool _firstMove = true;
        private Vector2 _lastPos;
        const float cameraSpeed = 1.5f;
        
        float RotationSensitivity = 0.6f;

        bool MiddleMouse = false;


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            KeyboardState input = KeyboardState;
            MouseState mouse = MouseState;

            if (!IsFocused) // Check to see if the window is focused
            {
                return;
            }

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if (input.IsKeyPressed(Keys.F))
            {
                this.camera = new Camera(Vector3.UnitZ * 1, Size.X / (float)Size.Y);
                this.ModelRotation = new Vector2(0.0f, 0.0f);
            }


            bool rotate = mouse[MouseButton.Middle] && input.IsKeyDown(Keys.LeftControl);
            bool pan = mouse[MouseButton.Middle] && !input.IsKeyDown(Keys.LeftControl);

            

            if (pan)
            {
                if (!this.MiddleMouse)
                {
                    this.MiddleMouse = true;
                    _lastPos = new Vector2(mouse.X, mouse.Y);
                }
                else
                {
                    float deltaX = mouse.X - _lastPos.X;
                    float deltaY = mouse.Y - _lastPos.Y;
                    _lastPos = new Vector2(mouse.X, mouse.Y);

                    float h = 2.0f / WindowHeight;
                    float w = 2.0f / WindowWidth;

                    this.camera.Position += deltaY * (this.camera.Up * h);
                    this.camera.Position -= deltaX * (Vector3.Normalize(Vector3.Cross(this.camera.Front, this.camera.Up)) * w);
                }

            }
            else
            {
                this.MiddleMouse = false;
            }


            if (rotate)
            {
                if (_firstMove) // this bool variable is initially set to true
                {
                    _lastPos = new Vector2(mouse.X, mouse.Y);
                    _firstMove = false;
                }
                else
                {
                    var deltaX = mouse.X - _lastPos.X;
                    var deltaY = mouse.Y - _lastPos.Y;
                    _lastPos = new Vector2(mouse.X, mouse.Y);

                    
                    this.ModelRotation.X += deltaY * RotationSensitivity; 
                    this.ModelRotation.Y += deltaX * RotationSensitivity;
                }
            }
            else
            {
                _firstMove = true;
            }
        }


        

        // In the mouse wheel function we manage all the zooming of the camera
        // this is simply done by changing the FOV of the camera
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            KeyboardState input = KeyboardState;

            if (input.IsKeyDown(Keys.LeftShift))
            {
                camera.Fov -= e.Offset.Y * 5;
            }
            else
            {
                this.camera.Position += (e.Offset.Y * this.camera.Front);
            }
            
            
            // Console.WriteLine("{0}    {1}", e.Offset.X, e.Offset.Y);
            base.OnMouseWheel(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            this.WindowWidth = e.Width;
            this.WindowHeight = e.Height;

            GL.Viewport(0, 0, this.WindowWidth, this.WindowHeight);
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