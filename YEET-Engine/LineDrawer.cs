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
    
    
    public class LineDrawer
    {
        private List<Vector3> _vertices;
        public ShaderLoader Loader;
        private int VAO, VBO;
        public Vector3 Color;
        private Dictionary<string, Line> Lines;
        
        
        public struct Line
        {
            public Line(Vector3 s, Vector3 e,Vector3 c)
            {
                Start = s;
                End = e;
                Color = c;
            }
            public Vector3 Start;
            public Vector3 End;
            public Vector3 Color;
        }
        
        public LineDrawer(Vector3 color)
        {
            Lines = new Dictionary<string, Line>();
            Loader = new ShaderLoader("Cube","LinesVert","LinesFrag",true);
            _vertices = new List<Vector3>();
            Color = color;
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
        }

        public void Draw(Matrix4 view, Matrix4 projection)
        {
            Loader.UseShader();
            GL.BindVertexArray(VAO);
            GL.LineWidth(2.0f);
            GL.DrawArrays(PrimitiveType.Lines,0,_vertices.Count*3);
            Loader.SetUniformMatrix4F("view", ref view);
            Loader.SetUniformMatrix4F("projection", ref projection);
            GL.BindVertexArray(0);
        }

        private void UpdateVertices()
        {
            _vertices.Clear();
            foreach (var line in Lines)
            {
                _vertices.Add(line.Value.Start);
                _vertices.Add(line.Value.Color);
                _vertices.Add(line.Value.End);
                _vertices.Add(line.Value.Color);
            }
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer,VBO);
            GL.BufferData(BufferTarget.ArrayBuffer,_vertices.Count*3*sizeof(float),_vertices.ToArray(),BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(0, 3,VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3,VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.BindVertexArray(0);
        }

        public void AddLine(string name,Vector3 startpoint, Vector3 endpoint,Vector3 color)
        {
            Lines.Add(name,new Line(startpoint,endpoint,color));
            UpdateVertices();
        }

        public void ChangeLine(string name, Vector3 startpoint, Vector3 endpoint,Vector3 color)
        {
            Lines[name] = new Line(startpoint, endpoint,color);
            UpdateVertices();
        }

        public void DeleteLine(string name)
        {
            Lines.Remove(name);
            UpdateVertices();
        }
    }
}