using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


namespace OpenTK_opengl4
{
    public class OBJLoader
    {
        public List<Vector3> Vertices;
        public List<Vector3> Normals;
        public List<Vector2> TextureCoordinates;
        public List<Vector3> ColorPerIndex;
        public List<int> ColorIndices;
        public List<int> Indices, NormalIndices;
        public Dictionary<string, Vector3> Materials;
        public Dictionary<string,int> MaterialsIndices;
        public int VAO, VBO, EBO;
        public ShaderLoader Loader;
        private string current_material;
        private List<float> FinalVertexArray;
        /// <summary>
        /// </summary>
        /// <param name="path">Path from the Models Directory</param>
        public OBJLoader(string path)
        {
            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            TextureCoordinates = new List<Vector2>();
            ColorPerIndex = new List<Vector3>();
            NormalIndices = new List<int>();
            Indices = new List<int>();
            MaterialsIndices = new Dictionary<string, int>();
            Materials = new Dictionary<string, Vector3>();
            Loader = new ShaderLoader("Model", "FlatShadedModelVert", "FlatShadedModelFrag", true);

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();

            StreamReader materialreader = new StreamReader("Models/" + path + ".mtl", Encoding.UTF8);
            string current_material_name = "", material_line;
            while ((material_line = materialreader.ReadLine()) != null)
            {
                string[] substrings = material_line.Split(" ");
                switch (substrings[0])
                {
                    case "newmtl":
                        current_material_name = substrings[1];
                        break;
                    case "Kd":
                        var color = new Vector3(Convert.ToSingle(substrings[1], CultureInfo.InvariantCulture),
                            Convert.ToSingle(substrings[2], CultureInfo.InvariantCulture),
                            Convert.ToSingle(substrings[3], CultureInfo.InvariantCulture));
                        Materials.Add(current_material_name, color);
                        MaterialsIndices.Add(current_material_name,Materials.Count-1);
                        break;
                }
            }

            string line;

            StreamReader reader = new StreamReader("Models/" + path + ".obj", Encoding.UTF8);
            while ((line = reader.ReadLine()) != null)
            {
                string[] substrings = line.Split(" ");
                switch (substrings[0])
                {
                    case "v":
                        Vertices.Add((Convert.ToSingle(substrings[1], CultureInfo.InvariantCulture),
                            Convert.ToSingle(substrings[2], CultureInfo.InvariantCulture),
                            Convert.ToSingle(substrings[3], CultureInfo.InvariantCulture)));
                        break;
                    case "vt":
                        TextureCoordinates.Add((Convert.ToSingle(substrings[1], CultureInfo.InvariantCulture),
                            Convert.ToSingle(substrings[2], CultureInfo.InvariantCulture)));
                        break;
                    case "vn":
                        Normals.Add((Convert.ToSingle(substrings[1], CultureInfo.InvariantCulture),
                            Convert.ToSingle(substrings[2], CultureInfo.InvariantCulture),
                            Convert.ToSingle(substrings[3], CultureInfo.InvariantCulture)));
                        break;
                    case "f":
                        SplitFLine(substrings);
                        break;
                    case "usemtl":
                        current_material = substrings[1];
                        break;
                }
            }
            GenerateFinalVertexArray();
        }

        public void Draw()
        {
            Loader.UseShader();
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, FinalVertexArray.Count / 3);
            GL.BindVertexArray(0);
        }

        private void SplitFLine(string[] line)
        {
            for (int i = 1; i < line.Length; i++)
            {
                string[] vertex = line[i].Split("/");
                Indices.Add(Convert.ToInt32(vertex[0]) - 1);
                NormalIndices.Add(Convert.ToInt32(vertex[2]) - 1);
                ColorPerIndex.Add(Materials[current_material]);
            }
        }

        private void GenerateFinalVertexArray()
        {
            FinalVertexArray = new List<float>();
            for (int x = 0; x < Indices.Count; x++)
            {
                //vec3 position,vec3 normals, uint index_color
                FinalVertexArray.Add(Vertices[Indices[x]].X);
                FinalVertexArray.Add(Vertices[Indices[x]].Y);
                FinalVertexArray.Add(Vertices[Indices[x]].Z);
                FinalVertexArray.Add(Normals[NormalIndices[x]].X);
                FinalVertexArray.Add(Normals[NormalIndices[x]].Y);
                FinalVertexArray.Add(Normals[NormalIndices[x]].Z);
                FinalVertexArray.Add(ColorPerIndex[x].X);
                FinalVertexArray.Add(ColorPerIndex[x].Y);
                FinalVertexArray.Add(ColorPerIndex[x].Z);
            }
            
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer,VBO);
            GL.BufferData(BufferTarget.ArrayBuffer,FinalVertexArray.Count*sizeof(float),FinalVertexArray.ToArray(),BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 3*sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 6*sizeof(float));
            GL.EnableVertexAttribArray(2);
            GL.BindVertexArray(0);
        }
    }
}