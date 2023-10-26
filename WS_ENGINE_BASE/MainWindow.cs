using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Input;
using OpenTK;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace WS_ENGINE_BASE
{
    public class MainWindow : GameWindow
    {
        
        Engine engine;

        public MainWindow(int width, int height, string title) : base(
            GameWindowSettings.Default,
            new NativeWindowSettings() { Size = (width, height), Title = title }
            )
        {
            string frag = @"C:\Users\wes\github-repos\dotnet_opentk_base\WS_ENGINE_BASE\shader.frag";
            string vert = @"C:\Users\wes\github-repos\dotnet_opentk_base\WS_ENGINE_BASE\shader.vert";

            engine = new Engine(width, height, Size, vert, frag);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            engine.RenderFrame(e);
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            int status = -2;
            engine.UpdateFrame(e, KeyboardState, MouseState, IsFocused, Size, ref status);

            if (status == -1)
            {
                Close();
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            engine.MouseWheel(e, KeyboardState);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            engine.Resize(e);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            engine.Load();
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            engine.Unload();
        }
    }

    
}