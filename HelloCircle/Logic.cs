using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Logic
{
    

    public class Logic
    {
        public static int VertexBuffer;
        public static int VertexArray;
        public static int ShaderHandle;

        public static int ElementBuffer;

        public static int DataLength;

        public static int Segments = 8;

        public static bool Filled = false;

        public static float CircleX = 0.0f;
        public static float CircleY = 0.0f;
        public static float CircleRadius = 0.5f;

        private static void CreateCircle(float radius, int division, out float[] vertices, out uint[] indices)
        {
            double angle = 2 * Math.PI / division;

            vertices = new float[12 + (3 * division)];
            vertices[0] = CircleX;
            vertices[1] = CircleY;
            vertices[2] = 0.0f;

            indices = new uint[3 + (3 * division)];
            // indices[indices.Length - 1] = (uint)(division + 2);
            // indices[indices.Length - 2] = (uint)(division + 1);
            // indices[indices.Length - 3] = 0;

            

            for (int i = 0; i <= division; i++)
            {
                vertices[3 * (i + 1)] =     CircleX + (radius * (float)Math.Cos(angle * i));//x);
                vertices[3 * (i + 1) + 1] = CircleY + (radius * (float)Math.Sin(angle * i)); //y;
                vertices[3 * (i + 1) + 2] = 0.0f;                       //z;

                indices[3 * i] =      0;
                indices[3 * i + 1] =  (uint)(i + 1);
                indices[3 * i + 2] =  (uint)(i + 2);

            }
            


            // for (int i = 0; i < indices.Length; i++)
            // {
            //     Console.Write("{0}    ,", indices[i]);

            //     if ((i+1) % 3 == 0)
            //     {
            //         Console.WriteLine();
            //     }
            // }
       

            
        }

        public static void Setup()
        {
            // Set the clear color for refreshing
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            CreateCircle(CircleRadius, Segments, out float[] vertices, out uint[] indices);
            DataLength = indices.Length;

            


            //-----------------------CREATE THE VERTICES----------------------//
            // Create the array buffer to fill with vertices
            VertexBuffer = GL.GenBuffer();

            VertexArray = GL.GenVertexArray();
            GL.BindVertexArray(VertexArray);

            // Bind the buffer with the target being array
            // Then the buffer can be filled
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);

            // Draw the vertices
            int data_bytes = vertices.Length * sizeof(float);

            GL.BufferData
            (
                BufferTarget.ArrayBuffer,
                data_bytes,
                vertices,
                BufferUsageHint.StaticDraw
            );


            ElementBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBuffer);

        

            GL.BufferData
            (
                BufferTarget.ElementArrayBuffer,
                indices.Length * sizeof(float),
                indices,
                BufferUsageHint.StaticDraw
            );

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3*sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            // GL.DeleteBuffer(this.VertexBuffer);


            

            

            //-----------------------LOAD THE SHADER----------------------//
            // Console.WriteLine("LOAD THE SHADER");

            // Create the shaders
            
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

        public static void Render()
        {
            // Clear the screen
            // GL.Clear(ClearBufferMask.ColorBufferBit);

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

            // Bind the vertex data
            GL.BindVertexArray(VertexArray);
            
            // Draw the triangle
            // GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            GL.DrawElements(PrimitiveType.Triangles, DataLength - 3, DrawElementsType.UnsignedInt, 0);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
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