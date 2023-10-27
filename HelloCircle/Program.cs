
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using Logic;

namespace HelloTriangle
{
    public class Game : GameWindow
    {

        

        // float[] vertices = 
        // {
        //     -0.5f, 0.5f, 0.0f,   // Top Left
        //     -0.5f, -0.5f, 0.0f, // Bottom Left
        //     0.5f, -0.5f, 0.0f,   // Bottom Right
        //     0.5f, 0.5f, 0.0f     // Top Right
        // };

        


        

        public Game(int width, int height, string title) : base(
            GameWindowSettings.Default,
            new NativeWindowSettings()
            {
                Size = (width,height),
                Title = title
            })
        {
            this.CenterWindow();
        }


        

        protected override void OnLoad()
        {
            base.OnLoad();

            Logic.Logic.Setup();
            Logic.Logic.SetupShader();
        }

        

        

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            Logic.Logic.Render();

            

            // Do this last to display the changes
            Context.SwapBuffers();
            // while(1==1)
            // {
            // }
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            KeyboardState key = KeyboardState;

            if (key.IsKeyDown(Keys.Escape))
            {
                Close();
                Environment.Exit(0);
            }

            if (key.IsKeyPressed(Keys.S))
            {
                Logic.Logic.Subdivide();
            }
        }



        

        protected override void OnUnload()
        {
            base.OnUnload();

            GL.DeleteProgram(Logic.Logic.ShaderHandle);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            // Resize the gl viewport when the window is resized
            GL.Viewport(0, 0, e.Width, e.Height);
        }

        

    }


    public static class Program
    {
        public static void Main(string[] args)
        {
            using (Game game = new Game(1280, 1280, "HelloTriangle"))
            {
                game.Run();
            }
        }
    }

    
}