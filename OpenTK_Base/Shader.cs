using OpenTK.Graphics.OpenGL4;

namespace Program
{
    public class Shader
    {
        public int Handle;

        public Shader(string vertex_path, string fragment_path)
        {
            // Read the shader files
            string vertexShaderSource = File.ReadAllText(vertex_path);
            string fragmentShaderSource = File.ReadAllText(fragment_path);

            // Create the shader objects and assign the read data to them
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);   
            GL.ShaderSource(fragmentShader, fragmentShaderSource);

            // Compile the shaders
            int shadersBuilt = 0;

            shadersBuilt += compile(vertexShader);
            shadersBuilt += compile(fragmentShader);

            if (shadersBuilt != 2)
            {
                return;
            }

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);

            GL.LinkProgram(Handle);
            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(Handle);
                Console.WriteLine(infoLog);
            }

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public int compile(int shaderObject)
        {
            GL.CompileShader(shaderObject);
            GL.GetShader(shaderObject, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shaderObject);
                Console.WriteLine(infoLog);

                return 0;
            }
            return 1;
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }



        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);

                disposedValue = true;
            }
        }

        ~Shader()
        {
            if (disposedValue == false)
            {
                Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}