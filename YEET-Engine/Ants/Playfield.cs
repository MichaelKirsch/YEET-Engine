using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using SimplexNoise;
using OpenTK.Graphics.OpenGL4;


namespace YEET.Ants
{
    public class Playfield
    {
        public int Width = 100;
        public int Height = 100;
        private float sizeTile = 0.5f;
        private List<Vector3> vertices = new List<Vector3>();
        public float HeightScaler = 200;
        public float MAXHEIGHT, MINHEIGHT;
        /// <summary>
        /// Scale of the used simplex noise
        /// </summary>
        public float NoiseScaleMayor = 0.01f;

        private int _VAO, _VBO;
        public ShaderLoader shaderLoader { get; }
        
        public Playfield()
        {
            shaderLoader = new ShaderLoader("Playfield", "TriangleVert", "TriangleFrag", true);
            _VBO = GL.GenBuffer();
            _VAO = GL.GenVertexArray();
        }
        
        
        public void Draw()
        {
            shaderLoader.UseShader();
            GL.BindVertexArray(_VAO);
            
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Count*3);
        }
        
        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        public void Generate()
        {
            SimplexNoise.Noise.Seed = new Random().Next();
            MAXHEIGHT = 0;
            MINHEIGHT = 1000;
            vertices.Clear();
            Parallel.For(0, Width, x =>
            {
                Parallel.For(0, Height, y =>
                {
                    var height = SimplexNoise.Noise.CalcPixel2D(x, y, NoiseScaleMayor)/HeightScaler;
                    if(height<MINHEIGHT)
                        MINHEIGHT = height;
                    if(height>MAXHEIGHT) 
                        MAXHEIGHT = height;

                    lock (vertices)
                    {
                        vertices.Add(new Vector3(-sizeTile+x,height ,-sizeTile+y));
                        vertices.Add(new Vector3(sizeTile+x,height,-sizeTile+y));
                        vertices.Add(new Vector3(-sizeTile+x,height,sizeTile+y));
                        //add triangle 2
                        vertices.Add(new Vector3(sizeTile+x,height,-sizeTile+y));
                        vertices.Add(new Vector3(sizeTile+x,height,sizeTile+y));
                        vertices.Add(new Vector3(-sizeTile+x,height,sizeTile+y));
                    }
                });
            });
            GL.BindVertexArray(_VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer,_VBO);
            GL.BufferData(BufferTarget.ArrayBuffer,sizeof(float)*3*vertices.Count,vertices.ToArray(),BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }
    }
}