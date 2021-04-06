using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using BufferTarget = OpenTK.Graphics.OpenGL4.BufferTarget;
using BufferUsageHint = OpenTK.Graphics.OpenGL4.BufferUsageHint;
using GL = OpenTK.Graphics.OpenGL4.GL;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;
using VertexAttribPointerType = OpenTK.Graphics.OpenGL4.VertexAttribPointerType;


namespace YEET
{
    public static class Colors
    {
        public static Vector3 Red{ get { return new Vector3(1,0,0);} }
        public static Vector3 Green{ get { return new Vector3(0,1,0);} }
        public static Vector3 Blue{ get { return new Vector3(0,0,1);} }
    }


    public class LineDrawer
    {
        private List<Vector3> _vertices;
        public ShaderLoader Loader;
        private int VAO, VBO;
        public Vector3 Color;
        private List<Line> Lines;

        
        
        public class Line
        {
            public Guid ID;

            public Line(Guid id,ref Vector3 s, ref Vector3 e, Vector3 c)
            {
                ID = id;
                Start = s;
                End = e;
                Color = c;
            }

            public Vector3 Start { get; set; }
            public Vector3 End { get; set; }
            public Vector3 Color { get; set; }
        }

        public LineDrawer(Vector3 color)
        {
            Lines = new List<Line>();
            Loader = new ShaderLoader("Cube", "LinesVert", "LinesFrag", true);
            _vertices = new List<Vector3>();
            Color = color;
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
        }

        public void Draw(Matrix4 view, Matrix4 projection)
        {
            UpdateVertices();
            Loader.UseShader();
            GL.BindVertexArray(VAO);
            GL.LineWidth(2.0f);
            GL.DrawArrays(PrimitiveType.Lines, 0, _vertices.Count * 3);
            Loader.SetUniformMatrix4F("view", ref view);
            Loader.SetUniformMatrix4F("projection", ref projection);
            GL.BindVertexArray(0);
        }

        private void UpdateVertices()
        {
            _vertices.Clear();
            foreach (var line in Lines)
            {
                _vertices.Add(line.Start);
                _vertices.Add(line.Color);
                _vertices.Add(line.End);
                _vertices.Add(line.Color);
            }

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * 3 * sizeof(float), _vertices.ToArray(),
                BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.BindVertexArray(0);
        }

        public Line AddLine(ref Vector3 startpoint,ref Vector3 endpoint, Vector3 color)
        {
            var x = new Line(Guid.NewGuid(), ref startpoint, ref endpoint, color);
            Lines.Add(x);
            UpdateVertices();
            return x;
        }

        public void DeleteLine(Guid name)
        {
            foreach (var line in Lines)
            {
                if (line.ID == name)
                {
                    Lines.Remove(line);
                    return;
                }
            }

            UpdateVertices();
        }
    }
}