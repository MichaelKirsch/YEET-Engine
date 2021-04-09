using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SimplexNoise;


namespace YEET
{
    public class PointCloudTest : Entity
    {
        private List<Vector3> FinalVertices;
        public float Scale;
        public ShaderLoader Loader;
        private int VAO,VBO;
        private float lastscale,lastsurface = 0.1f;
        public int Dimension=16;
        public float SurfaceLevel = 1.0f;
        public PointCloudTest()
        {
            FinalVertices = new List<Vector3>();
            Loader = new ShaderLoader("Points", "LinesVert", "LinesFrag", true);
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();

        }

        public void Generate()
        {
            if (lastscale != Scale || lastsurface != SurfaceLevel)
            {
                lastscale = Scale;
                lastsurface = SurfaceLevel;
                Util.StopWatchMilliseconds watch = new Util.StopWatchMilliseconds();
                FinalVertices.Clear();
                var Rand = new Random();
                Noise.Seed = Rand.Next();

                Parallel.For(0, Dimension, x =>
                {
                    Parallel.For(0, Dimension, y =>
                    {
                        Parallel.For(0, Dimension, z =>
                        {
                            var value = Noise.CalcPixel3D(x, y, z, Scale)/255.0f;
                            var v1 = new Vector3(x, y, z);
                            var v2 = new Vector3(value, value, value);
                            if (value < SurfaceLevel)
                            {
                                lock (FinalVertices)
                                {
                                    FinalVertices.Add(v1);
                                    FinalVertices.Add(v2);
                                }
                            }
                        });
                    });
                });
                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer,VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, FinalVertices.Count * 3 * sizeof(float), FinalVertices.ToArray(),
                    BufferUsageHint.DynamicDraw);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);
                GL.BindVertexArray(0);
            
                Console.WriteLine("Generated Pointcloud. Dimensions:" + Dimension +"x"+Dimension+"x"+Dimension+" Took:" + watch.Result() + "ms" );
            }
        }
        
        

        public override void OnDraw()
        {
            Generate();
            base.OnDraw();
            Loader.UseShader();
            GL.BindVertexArray(VAO);
            GL.PointSize(5.0f);
            GL.DrawArrays(PrimitiveType.Points, 0, FinalVertices.Count * 3);
            Loader.SetUniformMatrix4F("view", ref Camera.View);
            Loader.SetUniformMatrix4F("projection", ref Camera.Projection);
            GL.BindVertexArray(0);
        }
    }
}