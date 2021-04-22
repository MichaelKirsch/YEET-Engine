using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
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

    public class LineBlob
    {
        public List<LineDrawer.Line> _Lines = new List<LineDrawer.Line>();

        public void Clear()
        {
            _Lines.Clear();
        }

        public void AddAxisAllignedCube(Vector3 zeroPoint, float Dimension, Vector3 color)
        {
            Vector3 p1 = zeroPoint;
            Vector3 p2 = new Vector3(p1.X, p1.Y, p1.Z+Dimension);
            Vector3 p3 = new Vector3(p1.X, p1.Y+Dimension, p1.Z+Dimension);
            Vector3 p4 = new Vector3(p1.X, p1.Y+Dimension, p1.Z);
                
            Vector3 p5 = new Vector3(p1.X+Dimension, p1.Y, p1.Z);
            Vector3 p6 = new Vector3(p1.X+Dimension, p1.Y, p1.Z+Dimension);
            Vector3 p7 = new Vector3(p1.X+Dimension, p1.Y+Dimension, p1.Z+Dimension);
            Vector3 p8 = new Vector3(p1.X+Dimension, p1.Y+Dimension, p1.Z);
            
            _Lines.Add(new LineDrawer.Line(p1,p2,color));
            _Lines.Add(new LineDrawer.Line(p1,p4,color));
            _Lines.Add(new LineDrawer.Line(p3,p4,color));
            _Lines.Add(new LineDrawer.Line(p2,p3,color));
            
            _Lines.Add(new LineDrawer.Line(p8,p7,color));
            _Lines.Add(new LineDrawer.Line(p7,p6,color));
            _Lines.Add(new LineDrawer.Line(p5,p6,color));
            _Lines.Add(new LineDrawer.Line(p5,p8,color));
            
            _Lines.Add(new LineDrawer.Line(p3,p7,color));
            _Lines.Add(new LineDrawer.Line(p2,p6,color));
            _Lines.Add(new LineDrawer.Line(p4,p8,color));
            _Lines.Add(new LineDrawer.Line(p1,p5,color));
        }
        
        
    }

    public static class LineDrawer
    {
        private static List<Vector3> _vertices;
        public static ShaderLoader Loader;
        private static int VAO, VBO;
        public static Vector3 Color;
        private static List<LineBlob> Blobs;
        private static Queue<LineBlob> ToAdd = new Queue<LineBlob>();
        private static Queue<LineBlob> ToRemove = new Queue<LineBlob>();
        
        public class Line
        {
            public Guid ID;

            public Line(Vector3 s, Vector3 e, Vector3 c )
            {
                Start = s;
                End = e;
                Color = c;
            }

            public Vector3 Start { get; set; }
            public Vector3 End { get; set; }
            public Vector3 Color { get; set; }
        }

        static LineDrawer()
        {
            Blobs = new List<LineBlob>();
            Loader = new ShaderLoader("Cube", "LinesVert", "LinesFrag", true);
            _vertices = new List<Vector3>();
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
        }

        public static void Draw()
        {
            UpdateVertices();
            Loader.UseShader();
            GL.BindVertexArray(VAO);
            GL.LineWidth(2.0f);
            GL.DrawArrays(PrimitiveType.Lines, 0, _vertices.Count * 3);
            Loader.SetUniformMatrix4F("view", ref Camera.View);
            Loader.SetUniformMatrix4F("projection", ref Camera.Projection);
            GL.BindVertexArray(0);
        }

        private static void UpdateVertices()
        {
            _vertices.Clear();
            foreach (var blob in Blobs)
            {
                foreach (var line in blob._Lines)
                {
                    _vertices.Add(line.Start);
                    _vertices.Add(line.Color);
                    _vertices.Add(line.End);
                    _vertices.Add(line.Color);
                }
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

        public static void OnUpdate()
        {
            if (ToRemove.Count == 0 && ToAdd.Count == 0)
                return;
            
            while (ToRemove.Count>0)
            {
                Blobs.Remove(ToRemove.Dequeue());
            }

            while (ToAdd.Count>0)
            {
                Blobs.Add(ToAdd.Dequeue());
            }
            UpdateVertices();
        }
        
        public static void AddBlob(LineBlob toadd)
        {
            if(Blobs.Contains(toadd))
                ToRemove.Enqueue(toadd); //delete old values and update them
            ToAdd.Enqueue(toadd);
        }

        public static void RemoveBlob(LineBlob toremove)
        {
            ToRemove.Enqueue(toremove);
        }
    }
}