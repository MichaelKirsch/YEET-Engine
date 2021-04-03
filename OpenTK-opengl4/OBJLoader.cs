using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


namespace OpenTK_opengl4
{
    public class OBJLoader
    {
        public List<Vector3> Vertices;
        public List<Vector2> Normals;
        public List<Vector2> TextureCoordinates;
        public List<uint> Indices;
        public int VAO, VBO,EBO;
        private ShaderLoader _loader;
        /// <summary>
        /// </summary>
        /// <param name="path">Path from the Models Directory</param>
        public OBJLoader(string path, ShaderLoader loader)
        {
            Vertices = new List<Vector3>();
            Normals = new List<Vector2>();
            TextureCoordinates = new List<Vector2>();
            Indices = new List<uint>();
            
            _loader = loader;

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();
            
            string line;
            StreamReader reader = new StreamReader("Models/" + path + ".obj", Encoding.UTF8);
            while ((line = reader.ReadLine())!=null)
            {
                string[] substrings = line.Split(" ");
                switch (substrings[0])
                {
                    case "v":
                        Vertices.Add((Convert.ToSingle(substrings[1],CultureInfo.InvariantCulture),
                            Convert.ToSingle(substrings[2],CultureInfo.InvariantCulture),
                            Convert.ToSingle(substrings[3],CultureInfo.InvariantCulture)));
                        break;
                    case "vt":
                        TextureCoordinates.Add((Convert.ToSingle(substrings[1],CultureInfo.InvariantCulture),
                            Convert.ToSingle(substrings[2],CultureInfo.InvariantCulture)));
                        break;
                    case "vn":
                        Normals.Add((Convert.ToSingle(substrings[1],CultureInfo.InvariantCulture),
                            Convert.ToSingle(substrings[2],CultureInfo.InvariantCulture)));
                        break;
                    case "f":
                        SplitFLine(substrings);
                        break;
                }
            }
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Count * sizeof(UInt32), Indices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer,VBO);
            GL.BufferData(BufferTarget.ArrayBuffer,Vertices.Count*sizeof(float)*3,Vertices.ToArray(),BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            Console.WriteLine("Parsed " +path +": " + Vertices.Count + " Vertices | " + Indices.Count + " Indices" );
        }
        
        public void Draw()
        {
            _loader.UseShader();
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Count, DrawElementsType.UnsignedInt,0);
        }

        private void SplitFLine(string[] line)
        {
            for (int i = 1; i < line.Length; i++)
            {
                string[] vertex = line[i].Split("/");
                Indices.Add(Convert.ToUInt32(vertex[0]) - 1);
            }
        }


    }
}