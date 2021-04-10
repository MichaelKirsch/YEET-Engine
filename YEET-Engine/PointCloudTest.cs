//all of this code is heavily based on this video: https://www.youtube.com/watch?v=M3iI2l0ltbE&t=83s

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
        private List<Vector3> FinalTriangleVertices;
        public float Scale = 0.05f;
        public ShaderLoader Loader;
        private int VAO, VBO;
        private float lastscale, lastsurface = 0.1f;
        public int Dimension = 32;
        public float SurfaceLevel = 0.319f;
        public Vector3i Offset;
        private float[,,] pointCloudValues;
        public bool NeedsUpdate = true;
        
        private Vector3[] cubeCornerOffsets =
        {
            new(0, 0, 0),
            new(1, 0, 0),
            new(1, 0, 1),
            new(0, 0, 1),
            new(0, 1, 0),
            new(1, 1, 0),
            new(1, 1, 1),
            new(0, 1, 1),
        };

        public PointCloudTest(int noise_seed, Vector3i offset, ShaderLoader loader)
        {
            FinalTriangleVertices = new List<Vector3>();
            Loader = loader;
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            
            Offset = offset;
            Noise.Seed = noise_seed;
        }

        public void Generate()
        {
            if (lastscale != Scale || lastsurface != SurfaceLevel|| NeedsUpdate)
            {
                NeedsUpdate = false;
                lastscale = Scale;
                lastsurface = SurfaceLevel;
                Util.StopWatchMilliseconds watch = new Util.StopWatchMilliseconds();
                FinalTriangleVertices.Clear();

                pointCloudValues = Noise.Calc3D(Dimension, Dimension, Dimension, Scale);
                

                Parallel.For(0, Dimension,
                    x =>
                    {
                        Parallel.For(0, Dimension, y => { Parallel.For(0, Dimension, z =>
                        {
                            March((x, y, z));
                        }); });
                    });

                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, FinalTriangleVertices.Count * 3 * sizeof(float),
                    FinalTriangleVertices.ToArray(),
                    BufferUsageHint.DynamicDraw);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float),
                    3 * sizeof(float));
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float),
                    6 * sizeof(float));
                GL.EnableVertexAttribArray(2);
                GL.BindVertexArray(0);

                Console.WriteLine("Generated Marching Cube.Offset:" + Offset + " Dimensions:" + Dimension + "x" + Dimension + "x" +
                                  Dimension + " Took:" + watch.Result() + "ms");
            }
        }


        public override void OnUpdate()
        {
            base.OnUpdate();
            Generate();
        }

        public void OnDraw(Vector3 lightpos)
        {
            base.OnDraw();
            Loader.UseShader();
            GL.BindVertexArray(VAO);
            Loader.SetUniformMatrix4F("view", ref Camera.View);
            Loader.SetUniformMatrix4F("projection", ref Camera.Projection);
            Loader.SetUniformVec3("LightPosition", lightpos);
            Loader.SetUniformVec3("offset", Offset);
            GL.DrawArrays(PrimitiveType.Triangles, 0, FinalTriangleVertices.Count * 3);
            
            GL.BindVertexArray(0);
        }

        float GenerateFloatPerPoint(Vector3i pos)
        {
            return Noise.CalcPixel3D((pos+Offset).X,(pos+Offset).Y,(pos+Offset).Z, Scale) / 255.0f;
        }


        private void March(Vector3i CubePosition)
        {
            float[] cubeCorners =
            {
                GenerateFloatPerPoint((CubePosition.X, CubePosition.Y, CubePosition.Z)),
                GenerateFloatPerPoint((CubePosition.X + 1, CubePosition.Y, CubePosition.Z)),
                GenerateFloatPerPoint((CubePosition.X + 1, CubePosition.Y, CubePosition.Z + 1)),
                GenerateFloatPerPoint((CubePosition.X, CubePosition.Y, CubePosition.Z + 1)),
                GenerateFloatPerPoint((CubePosition.X, CubePosition.Y + 1, CubePosition.Z)),
                GenerateFloatPerPoint((CubePosition.X + 1, CubePosition.Y + 1, CubePosition.Z)),
                GenerateFloatPerPoint((CubePosition.X + 1, CubePosition.Y + 1, CubePosition.Z + 1)),
                GenerateFloatPerPoint((CubePosition.X, CubePosition.Y + 1, CubePosition.Z + 1))
            };

            int cubeIndex = 0;
            if (cubeCorners[0] < SurfaceLevel) cubeIndex |= 1;
            if (cubeCorners[1] < SurfaceLevel) cubeIndex |= 2;
            if (cubeCorners[2] < SurfaceLevel) cubeIndex |= 4;
            if (cubeCorners[3] < SurfaceLevel) cubeIndex |= 8;
            if (cubeCorners[4] < SurfaceLevel) cubeIndex |= 16;
            if (cubeCorners[5] < SurfaceLevel) cubeIndex |= 32;
            if (cubeCorners[6] < SurfaceLevel) cubeIndex |= 64;
            if (cubeCorners[7] < SurfaceLevel) cubeIndex |= 128;


            for (int i = 0; MarchingCubesTables.triangulation[cubeIndex, i] != -1; i += 3)
            {
                // Get indices of corner points A and B for each of the three edges
                // of the cube that need to be joined to form the triangle.
                int a0 = MarchingCubesTables.cornerIndexAFromEdge[MarchingCubesTables.triangulation[cubeIndex, i]];
                int b0 = MarchingCubesTables.cornerIndexBFromEdge[MarchingCubesTables.triangulation[cubeIndex, i]];

                int a1 = MarchingCubesTables.cornerIndexAFromEdge[MarchingCubesTables.triangulation[cubeIndex, i + 1]];
                int b1 = MarchingCubesTables.cornerIndexBFromEdge[MarchingCubesTables.triangulation[cubeIndex, i + 1]];

                int a2 = MarchingCubesTables.cornerIndexAFromEdge[MarchingCubesTables.triangulation[cubeIndex, i + 2]];
                int b2 = MarchingCubesTables.cornerIndexBFromEdge[MarchingCubesTables.triangulation[cubeIndex, i + 2]];

                lock (FinalTriangleVertices)
                {
                    Vector3 Color = new Vector3((0.01f + CubePosition.X) / Dimension,
                        (0.01f + CubePosition.Y) / Dimension, (0.01f + CubePosition.Z) / Dimension);
                    Vector3 v1 = ((cubeCornerOffsets[a0] + CubePosition) + (cubeCornerOffsets[b0] + CubePosition)) / 2;
                    Vector3 v2 = ((cubeCornerOffsets[a1] + CubePosition) + (cubeCornerOffsets[b1] + CubePosition)) / 2;
                    Vector3 v3 = ((cubeCornerOffsets[a2] + CubePosition) + (cubeCornerOffsets[b2] + CubePosition)) / 2;
                    var dir = Vector3.Cross(v2 - v1, v3 - v1);
                    Vector3 norm = Vector3.Normalize(dir);
                    lock (FinalTriangleVertices)
                    {
                        FinalTriangleVertices.AddRange(new List<Vector3>()
                            {v1, Color, norm, v2, Color, norm, v3, Color, norm});
                    }
                }
            }
        }
    }
}