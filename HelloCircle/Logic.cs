using System.Numerics;
using OpenTK.Graphics.OpenGL4;

using Drawing;

namespace Logic
{
    

    public class Logic
    {
        public static int VertexArrayCircle1;
        public static int VertexArrayCircle2;
        public static int ShaderHandle;

        public static int DataLengthCircle1;
        public static int DataLengthCircle2;

        public static int Segments = 8;

        public static bool Filled = false;

        // x y radius
        public static Vector3 CircleSize = new Vector3(-0.4f, 0.2f, 0.125f);
        public static Vector3 CircleSize2 = new Vector3(0.7f, 0.7f, 0.125f);

        public static Vector3 SquareSize = new Vector3(0.0f, 0.0f, 1.0f);

        public static bool RenderClearScreen = true;

        public static Circle circleA = new Circle();
        public static Circle circleB = new Circle();

        public static Square square1 = new Square();

        public static bool collideC = false;
        public static bool collideS = false;

        

        

        public static void Setup()
        {
            // Set the clear color for refreshing
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            circleA.Create(CircleSize.Z, new Vector2(CircleSize.X, CircleSize.Y),  Segments);
            circleA.Draw();

            circleB.Create(CircleSize2.Z, new Vector2(CircleSize2.X, CircleSize2.Y),  16);
            circleB.Draw();

            square1.Create(SquareSize.Z, new Vector2(SquareSize.X, SquareSize.Y));
            Square.Draw(square1);

            collideC = Circle.Collide(circleA, circleB);
            collideS = Circle.Collide(circleA, square1);    
        }

        public static void Subdivide()
        {
            if (!Circle.Collide(circleA, square1))
            {
                Console.WriteLine("The circle is not intersecting the square");
                return;
            }
            Console.WriteLine("The circle is intersecting the square...subdividing");
            
        }

        

        public static void Render()
        {
            // Clear the screen
            if (RenderClearScreen)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit);
            }

            // Set wireframe
            if (!Filled)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
            // Use the shader
            GL.UseProgram(ShaderHandle);

            if (circleA != null && circleB != null && square1 != null)
            {
                circleA.Render();
                circleB.Render();

                Square.SubdivideFromCircle(circleA, square1, 7);
                Square.Draw(square1);
                Square.Render(square1);
            }
            

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }



        public static void SetupShader()
        {
            int VertShader = GL.CreateShader(ShaderType.VertexShader);
            int FragShader = GL.CreateShader(ShaderType.FragmentShader);

            // Bind the code to them
            GL.ShaderSource(VertShader, vert);
            GL.ShaderSource(FragShader, frag);

            // Compile the shaders
            GL.CompileShader(VertShader);
            GL.CompileShader(FragShader);

            // Get the shader status
            GL.GetShader(VertShader, ShaderParameter.CompileStatus, out int vert_success);
            GL.GetShader(FragShader, ShaderParameter.CompileStatus, out int frag_success);

            Console.WriteLine("Vert succes: {0} Frag success: {1}", vert_success, frag_success);

            if (vert_success + frag_success != 2)
            {
                string info_vert = GL.GetShaderInfoLog(VertShader);
                string info_frag = GL.GetShaderInfoLog(FragShader);

                Console.WriteLine("Vertex Info: \r\n {0}", info_vert);
                Console.WriteLine();
                Console.WriteLine("Fragment Info: \r\n {0}", info_frag);
            }

            // Create the program
            ShaderHandle = GL.CreateProgram();

            // Attach shaders to program
            GL.AttachShader(ShaderHandle, VertShader);
            GL.AttachShader(ShaderHandle, FragShader);

            // Link the program and get the status
            GL.LinkProgram(ShaderHandle);

            GL.GetProgram(ShaderHandle, GetProgramParameterName.LinkStatus, out int success);

            if (success == 0)
            {
                Console.WriteLine("Shader Link: {0}", GL.GetProgramInfoLog(ShaderHandle));
            }

            // Now that it's stored we can remove
            GL.DetachShader(ShaderHandle, VertShader);
            GL.DetachShader(ShaderHandle, FragShader);
            GL.DeleteShader(VertShader);
            GL.DeleteShader(FragShader);
        }

        static string vert = 
        """
            #version 330 core
            layout (location = 0) in vec3 aPosition;

            void main()
            {
                gl_Position = vec4(aPosition.x, aPosition.y, aPosition.z, 1.0);
            }
        """;

        static string frag = 
        """
            #version 330 core
            out vec4 FragColor;

            void main()
            {
                FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
            }
        """;
    }

    
}