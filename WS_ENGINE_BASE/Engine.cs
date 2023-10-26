using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Input;
using OpenTK;
using OpenTK.Windowing.GraphicsLibraryFramework;

using WS_2D_PIXEL;

namespace WS_ENGINE_BASE
{
    public class Engine
    {
        public static float PLAYER_SPEED = 0.5f;
        public static float cameraSpeed = 1.5f;

        int WindowWidth;
        int WindowHeight;

        uint[] tri_indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        public static WS_2D_PIXEL.Square sq1;

        public static Camera? camera;
        Shader shader;

        public static Vector2 ModelRotation = new Vector2(0.0f, 0.0f);
        private bool _firstMove = true;
        public static Vector2 _lastPos;
        
        
        float RotationSensitivity = 0.6f;
        float StartZ = 2;

        bool MiddleMouse = false;

        
        public Engine(int _WindowWidth, int _WindowHeight, Vector2 Size, string _shader_vert, string _shader_frag)
        {
            this.shader = new Shader(_shader_vert, _shader_frag);
            this.WindowWidth = _WindowWidth;
            this.WindowHeight = _WindowHeight;

            camera = new Camera(Vector3.UnitZ * StartZ, Size.X / (float)Size.Y);

            float[] square_size = { -1.0f, 1.0f, 2.0f, 2.0f };

            sq1 = new WS_2D_PIXEL.Square(square_size);

            sq1.SetColor(1.0f, 0.0f, 0.0f, 1.0f);
        }

        public void RenderFrame(FrameEventArgs e)
        {
            if (camera != null)
            {

                GL.Clear(ClearBufferMask.ColorBufferBit);

                this.shader.Use();

                int vertexColorLocation = GL.GetUniformLocation(this.shader.Handle, "ourColor");
                GL.Uniform4(vertexColorLocation, 1.0f, 0.0f, 0.0f, 1.0f);


                float x_rotation = MathHelper.DegreesToRadians(ModelRotation.X);
                float y_rotation = MathHelper.DegreesToRadians(ModelRotation.Y);

                Matrix4 model = Matrix4.CreateRotationX(x_rotation);
                model *= Matrix4.CreateRotationY(y_rotation);


                shader.SetMatrix4("model", model);
                shader.SetMatrix4("view", camera.GetViewMatrix());
                shader.SetMatrix4("projection", camera.GetProjectionMatrix());


                Square.Render(sq1, vertexColorLocation);
            }
        }

        public void UpdateFrame(FrameEventArgs e, KeyboardState input, MouseState mouse, bool IsFocused, Vector2 Size, ref int status)
        {

            if (!IsFocused) // Check to see if the window is focused
            {
                return;
            }

            if (input.IsKeyDown(Keys.Escape))
            {
                status = -1;
                return;
            }

            if (input.IsKeyPressed(Keys.F))
            {
                camera = new Camera(Vector3.UnitZ * StartZ, Size.X / (float)Size.Y);
                ModelRotation = new Vector2(0.0f, 0.0f);
            }

            if (input.IsKeyPressed(Keys.S))
            {
                Square.Subdivide(sq1);
                Console.WriteLine("There are {0} cubes.", Square.CubeCounter);
            }


            bool rotate = mouse[MouseButton.Middle] && input.IsKeyDown(Keys.LeftControl);
            bool pan = mouse[MouseButton.Middle] && !input.IsKeyDown(Keys.LeftControl);

            if (pan && camera != null)
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

                    camera.Position += deltaY * (camera.Up * h);
                    camera.Position -= deltaX * (Vector3.Normalize(Vector3.Cross(camera.Front, camera.Up)) * w);
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

                    
                    ModelRotation.X += deltaY * RotationSensitivity; 
                    ModelRotation.Y += deltaX * RotationSensitivity;
                }
            }
            else
            {
                _firstMove = true;
            }

            return;
        }

        public void MouseWheel(MouseWheelEventArgs e, KeyboardState input)
        {
            if (camera != null)
            {

                if (!input.IsKeyDown(Keys.LeftShift))
                {
                    camera.Fov += e.Offset.Y * 5;
                }
                else
                {
                    camera.Position += (e.Offset.Y * camera.Front);
                }
            }
        }

        public void Resize(ResizeEventArgs e)
        {
            this.WindowWidth = e.Width;
            this.WindowHeight = e.Height;

            GL.Viewport(0, 0, this.WindowWidth, this.WindowHeight);
        }

        public void Load()
        {
            // Set the background color
            GL.ClearColor(0.0f, 0.0f, 0.1f, 1.0f);

            // Load the shader
            Square.Draw(sq1);
        }

        public void Unload()
        {
            this.shader.Dispose();
        }

    }
}