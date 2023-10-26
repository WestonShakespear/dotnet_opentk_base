using System.Numerics;
using OpenTK.Graphics.OpenGL4;

namespace Drawing
{
    public class Square
    {
        float[] vertices = new float[12];
        uint[] indices  = {
            0, 1, 2,
            0, 2, 3
        };

        int VertexArray;

        public Vector2 Center;
        public float Side;


        public Square[] SubSquare = new Square[4];
        public bool[] Subs = { false, false, false, false };

        bool Drawn = false;
        bool Off = false;

        public Square()
        {

        }

        public void Create(float side, Vector2 location)
        {
            Center.X = location.X;
            Center.Y = location.Y;
            Side = side;

            float side_half = side / 2.0f;

            float[] vert = {
                location.X - side_half, location.Y + side_half, 0.0f, //top left
                location.X + side_half, location.Y + side_half, 0.0f, //top right
                location.X + side_half, location.Y - side_half, 0.0f, //bottom right
                location.X - side_half, location.Y - side_half, 0.0f //bottom left
            };

            for (int i = 0; i < vert.Length; i++)
            {
                vertices[i] = vert[i];
            }
              
        }
        public static void Draw(Square square)
        {
            if (IsRoot(square))
            {
                if (square.Drawn)   return;
                //-----------------------CREATE THE VERTICES----------------------//
                // Create the array buffer to fill with vertices
                int vertex_buffer = GL.GenBuffer();

                square.VertexArray = GL.GenVertexArray();
                GL.BindVertexArray(square.VertexArray);

                // Bind the buffer with the target being array
                // Then the buffer can be filled
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer);

                // Draw the vertices
                int data_bytes = square.vertices.Length * sizeof(float);

                GL.BufferData
                (
                    BufferTarget.ArrayBuffer,
                    data_bytes,
                    square.vertices,
                    BufferUsageHint.StaticDraw
                );

                int element_buffer = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, element_buffer);

                GL.BufferData
                (
                    BufferTarget.ElementArrayBuffer,
                    square.indices.Length * sizeof(float),
                    square.indices,
                    BufferUsageHint.StaticDraw
                );

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3*sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                square.Drawn = true;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    Draw(square.SubSquare[i]);
                }
            }
        }

        public static void Render(Square square)
        {
            if (IsRoot(square))
            {
                if (square.Off)     return;

                // Bind the vertex data
                GL.BindVertexArray(square.VertexArray);
                
                // Draw the triangle
                // GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
                GL.DrawElements(PrimitiveType.Triangles, square.indices.Length, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    Render(square.SubSquare[i]);
                }
            }
        }

        public static bool IsRoot(Square square)
        {
            return !square.Subs[0] && !square.Subs[1] && !square.Subs[2] && !square.Subs[3];
        }

        public static void Subdivide(Square square)
        {
            float offset = square.Side / 4;

            Vector2[] positions = {
                new Vector2(square.Center.X - offset, square.Center.Y - offset),
                new Vector2(square.Center.X + offset, square.Center.Y - offset),
                new Vector2(square.Center.X - offset, square.Center.Y + offset),
                new Vector2(square.Center.X + offset, square.Center.Y + offset)
            };

            for (int i = 0; i < 4; i++)
            {
                square.Subs[i] = true;
                square.SubSquare[i] = new Square();
                square.SubSquare[i].Create(square.Side / 2, positions[i]);
            }
            
        }

        public static void SubdivideFromCircle(Circle circle, Square square, int max_level)
        {
            
            if (IsRoot(square))
            {
                bool collision = Circle.Collide(circle, square);

                // Console.WriteLine("This is the root square");

                if (collision)
                {
                    if (max_level == 0)
                    {
                        square.Off = true;
                        return;
                    }

                    // Console.WriteLine("Circle is inside, subdividing this root");
                    Subdivide(square);
                }
                
               
            }
            else
            {
                max_level--;
                for (int i = 0; i < 4; i++)
                {
                    SubdivideFromCircle(circle, square.SubSquare[i], max_level);
                }
            }
        }

        // public static bool Collide(Circle a, Circle b)
        // {
        //     float x_distance = a.Center.X - b.Center.X;
        //     float y_distance = a.Center.Y - b.Center.Y;

        //     float distance = (float)Math.Sqrt( (x_distance * x_distance) - (y_distance - y_distance));

        //     if (distance < a.Radius + b.Radius)
        //     {
        //         return true;
        //     }

        //     return false;
        // }
    }
}