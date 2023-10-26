using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;



namespace WS_2D_PIXEL
{
    public class Square
    {
        const int MIN = 0;
        const int MAX = 4;

        public Vector2 Position;
        public Vector2 Size;

        public float R = 1.0f;
        public float G = 0.0f;
        public float B = 0.0f;
        public float A = 1.0f;

        public int VertexDataBufferObject;
        public int ElementBufferObject;
        public int VertexArrayObject;

        public bool Self = true;
        public bool Drawn = false;

        public Square[] SubSquare = new Square[4];

        public static int CubeCounter = 0;

        public Square(float[] size)
        {
            this.Position.X = size[0];
            this.Position.Y = size[1];

            this.Size.X = size[2];
            this.Size.Y = size[3];
        }

        public void SetColor(float _r, float _g, float _b, float _a)
        {
            this.R = _r;
            this.G = _g;
            this.B = _b;
            this.A = _a;
        }

        

        public static void Draw(Square _root)
        {
            if (_root.Self)
            {
                float[] vert =
                {
                    _root.Position.X + _root.Size.X, _root.Position.Y, 0.0f,  // top right
                    _root.Position.X + _root.Size.X, _root.Position.Y - _root.Size.Y, 0.0f,  // bottom right
                    _root.Position.X, _root.Position.Y - _root.Size.Y, 0.0f,  // bottom left
                    _root.Position.X, _root.Position.Y, 0.0f   // top left
                };

                uint[] indices =
                {
                    0, 1, 3,
                    1, 2, 3
                };

                _root.VertexArrayObject = GL.GenVertexArray();
                GL.BindVertexArray(_root.VertexArrayObject);

                // Create and bind buffer for vertex data
                _root.VertexDataBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _root.VertexDataBufferObject);

                // Now that it's bound, load with data
                GL.BufferData(
                    BufferTarget.ArrayBuffer,
                    vert.Length * sizeof(float),
                    vert,
                    BufferUsageHint.DynamicDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                _root.ElementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _root.ElementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);

                _root.Drawn = true;
            }
            else
            {
        
                for (int i = MIN; i < MAX; i++)
                {
                    Square.Draw(_root.SubSquare[i]);
                }
            }
        }

        public static void Render(Square _root, int _vertexColorLocation)
        {
            if (_root.Self)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.Uniform4(_vertexColorLocation, _root.R, _root.G, _root.B, _root.A);
                GL.BindVertexArray(_root.VertexArrayObject);
                GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            
            }
            else
            {
    
                for (int i = MIN; i < MAX; i++)
                {
                    if (!_root.SubSquare[i].Drawn)
                    {
                        Square.Draw(_root.SubSquare[i]);
                    }
        
                    Square.Render(_root.SubSquare[i], _vertexColorLocation);
                }
            }
        }

        public static void Subdivide(Square _root)
        {
            if (_root.Self == true)
            {
                _root.Self = false;

                float new_width = _root.Size.X / 2.0f;
                float new_height = _root.Size.Y / 2.0f;

                float[] top_left =  { _root.Position.X, _root.Position.Y, new_width, new_height };
                float[] top_right = { _root.Position.X + new_width, _root.Position.Y, new_width, new_height };
                float[] bottom_right = { _root.Position.X + new_width, _root.Position.Y - new_height, new_width, new_height };
                float[] bottom_left = { _root.Position.X, _root.Position.Y - new_height, new_width, new_height };


                _root.SubSquare[0] = new Square(top_left);
                _root.SubSquare[0].SetColor(_root.R, _root.G, _root.B, _root.A);

                _root.SubSquare[1] = new Square(top_right);
                _root.SubSquare[1].SetColor(_root.R - 0.3f, _root.G, _root.B, _root.A);

                _root.SubSquare[2] = new Square(bottom_right);
                _root.SubSquare[2].SetColor(_root.R, _root.G + 0.3f, _root.B, _root.A);

                _root.SubSquare[3] = new Square(bottom_left);
                _root.SubSquare[3].SetColor(_root.R, _root.G, _root.B + 0.3f, _root.A);

                CubeCounter += 4;
            }
            else
            {
                for (int i = MIN; i < MAX; i++)
                {
                    Subdivide(_root.SubSquare[i]);
                }
            }

           
            
        }

        
    }
}