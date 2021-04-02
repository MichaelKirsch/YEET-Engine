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
        
        public List<float> Vertices;
        public List<uint> Indices;
        public int VAO, VBO,EBO;
        private ShaderLoader _loader;
        /// <summary>
        /// </summary>
        /// <param name="path">Path from the Models Directory</param>
        public OBJLoader(string path, ShaderLoader loader)
        {
            Vertices = new List<float>();
            Indices = new List<uint>();
            _loader = loader;

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();
            
            
            string line;
            StreamReader reader = new StreamReader("Models/" + path + ".obj", Encoding.UTF8);
            while ((line = reader.ReadLine())!=null)
            {
                if (line.IndexOf("v ", 0)!=-1)
                {
                    string[] subs = line.Split(' ');
                    Vertices.Add(Convert.ToSingle(subs[1],CultureInfo.InvariantCulture));
                    Vertices.Add(Convert.ToSingle(subs[2],CultureInfo.InvariantCulture));
                    Vertices.Add(Convert.ToSingle(subs[3],CultureInfo.InvariantCulture));
                    continue;
                }
                if (line.IndexOf("f ", 0)!=-1)
                {
                    var substrings  =line.Replace("/"," ").Split(" ");
                    
                    for (int x = 1; x < substrings.Length; x++)
                    {
                        uint item = Convert.ToUInt32(substrings[x]);
                        Indices.Add(item);
                    }
                    continue; 
                }
                
            }
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Count * sizeof(UInt32), Indices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer,VBO);
            GL.BufferData(BufferTarget.ArrayBuffer,Vertices.Count*sizeof(float),Vertices.ToArray(),BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            Console.WriteLine("Parsed " +path +": " + Vertices.Count + " Vertices | " + Indices.Count + " Indices" );
        }


        public void Draw()
        {
            _loader.UseShader();
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Count, DrawElementsType.UnsignedInt, 0);
        }
        
    }
}