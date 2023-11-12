using System.Numerics;
using OpenTK.Graphics.OpenGL4;

namespace Drawing
{
    public class Circle
    {
        float[] vertices = {};
        uint[] indices = {};

        int VertexArray;

        public Vector2 Center;
        public float Radius;

        public Circle()
        {

        }

        public void Create(float radius, Vector2 location, int division)
        {
            Center.X = location.X;
            Center.Y = location.Y;
            Radius = radius;

            double angle = 2 * Math.PI / division;

            vertices = new float[12 + (3 * division)];
            vertices[0] = location.X;
            vertices[1] = location.Y;
            vertices[2] = 0.0f;

            indices = new uint[3 + (3 * division)];

            for (int i = 0; i <= division; i++)
            {
                vertices[3 * (i + 1)] =     location.X + (radius * (float)Math.Cos(angle * i));//x);
                vertices[3 * (i + 1) + 1] = location.Y + (radius * (float)Math.Sin(angle * i)); //y;
                vertices[3 * (i + 1) + 2] = 0.0f;                       //z;

                indices[3 * i] =      0;
                indices[3 * i + 1] =  (uint)(i + 1);
                indices[3 * i + 2] =  (uint)(i + 2);

            }    
        }
        public void Draw()
        {
             //-----------------------CREATE THE VERTICES----------------------//
            // Create the array buffer to fill with vertices
            int vertex_buffer = GL.GenBuffer();

            VertexArray = GL.GenVertexArray();
            GL.BindVertexArray(VertexArray);

            // Bind the buffer with the target being array
            // Then the buffer can be filled
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer);

            // Draw the vertices
            int data_bytes = vertices.Length * sizeof(float);

            GL.BufferData
            (
                BufferTarget.ArrayBuffer,
                data_bytes,
                vertices,
                BufferUsageHint.StaticDraw
            );

            int element_buffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, element_buffer);

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
        }

        public void Render()
        {
            // Bind the vertex data
            GL.BindVertexArray(VertexArray);
            
            // Draw the triangle
            // GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length - 3, DrawElementsType.UnsignedInt, 0);
        }

        public static bool Collide(Circle a, Circle b)
        {
            float x_distance = a.Center.X - b.Center.X;
            float y_distance = a.Center.Y - b.Center.Y;

            float distance = (float)Math.Sqrt( (x_distance * x_distance) + (y_distance * y_distance));

            if (distance < a.Radius + b.Radius)
            {
                return true;
            }

            return false;
        }
        public static bool Collide(Circle a, Square b)
        {
            float circle_x = a.Center.X;
            float circle_y = a.Center.Y;

            float square_half = b.Side / 2.0f;

            float square_x_lower = b.Center.X - square_half - a.Radius;
            float square_x_upper = b.Center.X + square_half + a.Radius;

            float square_y_lower = b.Center.Y - square_half - a.Radius;
            float square_y_upper = b.Center.Y + square_half + a.Radius;

            bool in_x = square_x_lower <= circle_x && circle_x <= square_x_upper;
            bool in_y = square_y_lower <= circle_y && circle_y <= square_y_upper;

            return in_x && in_y;
        }

        public static bool Collide(Square s, Circle c)
        {
            float offset = s.Side / 2.0f;

            Vector2[] positions = {
                new Vector2(s.Center.X - offset, s.Center.Y - offset),
                new Vector2(s.Center.X + offset, s.Center.Y - offset),
                new Vector2(s.Center.X - offset, s.Center.Y + offset),
                new Vector2(s.Center.X + offset, s.Center.Y + offset)
            };

            for (int i = 0; i < 4; i++)
            {
                float x_distance = positions[i].X - c.Center.X;
                float y_distance = positions[i].Y - c.Center.Y;

                float distance = (float)Math.Sqrt( (x_distance * x_distance) + (y_distance * y_distance));

                if (distance < c.Radius)
                {
                    return true;
                }
            }


            

            return false;
        }
    }
}