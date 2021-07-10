//all of this code is heavily based on this video: https://www.youtube.com/watch?v=M3iI2l0ltbE&t=83s

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SimplexNoise;


namespace YEET
{
    public class MarchingCubeTerrain : Entity
    {
        private List<MarchingCubeChunk> _chunks;
        public float Scale = 0.019f;
        public int Divider = 7;
        public float SurfaceLevel = 0.319f;
        public Vector3 Dimensions;
        public int Seed;
        private ShaderLoader _loader;

        private float lastsurface = 0, lastscale = 0;
        private int lastSeed = 0, lastDivider = 0;

        Guid mesh;

        public MarchingCubeTerrain(Vector3 _Dimensions)
        {
            _chunks = new List<MarchingCubeChunk>();
            Dimensions = _Dimensions;
            var rand = new Random();
            Seed = rand.Next();
            _loader = new ShaderLoader("MarchingCubes");
            mesh = AddComponent(new Mesh(this,_loader));
            _chunks.Clear();
            for (int x = 0; x < Dimensions.X; x++)
            {
                for (int y = 0; y < Dimensions.Y; y++)
                {
                    for (int z = 0; z < Dimensions.Z; z++)
                    {
                        _chunks.Add(new MarchingCubeChunk(Seed, new Vector3i(x * 32, y * 32, z * 32), _loader));
                    }
                }
            }

            Generate();
        }


        public override void OnUpdate()
        {
            base.OnUpdate();
            Generate();
        }

        public override void OnRender()
        {
            base.OnRender();
            foreach (var chunk in _chunks)
            {
                chunk.OnDraw();
            }
        }


        public override void OnGui()
        {
            if (ShowGUI)
            {
                ImGui.Begin("Terrain (MC)");
                ImGui.SetWindowFontScale(1.5f);
                base.OnGui();
                ImGui.Checkbox("Active", ref Active);
                ImGui.SliderFloat("Scale", ref Scale, 0.001f, 0.1f);
                ImGui.SliderFloat("Surface Level", ref SurfaceLevel, 0.01f, 1.0f);
                ImGui.SliderInt("Divider", ref Divider, 1, 30);
                ImGui.End();
            }

            ImGui.Checkbox("Terrain", ref ShowGUI);
            
        }

        void Generate()
        {
            if (lastscale != Scale || lastsurface != SurfaceLevel || lastDivider != Divider || lastSeed != Seed)
            {
                var watch = new Util.StopWatchMilliseconds();
                lastscale = Scale;
                lastsurface = SurfaceLevel;
                lastDivider = Divider;
                lastSeed = Seed;
                foreach (var chunk in _chunks)
                {
                    chunk.Scale = Scale;
                    chunk.Divider = Divider;
                    chunk.SurfaceLevel = SurfaceLevel;
                    chunk.Generate();
                }

                Console.WriteLine("Generated " + _chunks.Count + " Chunks.Took: " + watch.Result() + "ms");
            }
        }
    }


    class MarchingCubeChunk
    {
        private List<Vector3> FinalTriangleVertices;
        private List<Vector3> PointsVisualizeCoordinates;
        public float Scale = 0.019f;
        public ShaderLoader Loader;
        private int VAO, VBO;
        private float lastscale, lastsurface = 0.1f;
        public int Dimension = 32;
        public float SurfaceLevel = 0.319f;
        public Vector3i Offset;
        public bool NeedsUpdate = true;
        public float Divider = 7;
        private float last_divider = 100;

        

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

        public MarchingCubeChunk(int noise_seed, Vector3i offset, ShaderLoader loader)
        {
            FinalTriangleVertices = new List<Vector3>();
            PointsVisualizeCoordinates = new List<Vector3>();
            Loader = loader;
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();

            Offset = offset;
            Noise.Seed = noise_seed;
        }

        public void Generate()
        {
            if (lastscale != Scale || lastsurface != SurfaceLevel || NeedsUpdate || last_divider != Divider)
            {
                NeedsUpdate = false;
                last_divider = Divider;
                lastscale = Scale;
                lastsurface = SurfaceLevel;
                Util.StopWatchMilliseconds watch = new Util.StopWatchMilliseconds();
                FinalTriangleVertices.Clear();
                
                Parallel.For(0, Dimension,
                    x =>
                    {
                        Parallel.For(0, Dimension, y =>
                        {
                            Parallel.For(0, Dimension, z =>
                            {
                                March((x, y, z));
                            });
                        });
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

                
            
                Console.WriteLine("Generated Marching Cube.Offset:" + Offset + " Dimensions:" + Dimension + "x" +
                                  Dimension + "x" +
                                  Dimension + " Took:" + watch.Result() + "ms");
            }
        }
        

        public void OnDraw()
        {
            Loader.UseShader();
            GL.BindVertexArray(VAO);
            Loader.SetUniformMatrix4F("view", Camera.View);
            Loader.SetUniformMatrix4F("projection", Camera.Projection);
            //Loader.SetUniformVec3("offset", Offset);
            GL.DrawArrays(PrimitiveType.Triangles, 0, FinalTriangleVertices.Count / 3);

            GL.BindVertexArray(0);
        }

        public float GenerateFloatPerPoint(Vector3i pos)
        {
            pos = pos + Offset;
            return -pos.Y + (Noise.CalcPixel3D(pos.X, pos.Y, pos.Z, Scale) / Divider);
        }

        public float GenerateFloatPerPointF(Vector3 pos)
        {
            pos = pos + Offset;
            return -pos.Y + (Noise.CalcPixel3D(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), Convert.ToInt32(pos.Z), Scale) / Divider);
        }
        
        public int GetSurface(Vector2i pos)
        {
            for (int x = 0; x < Dimension; x++)
            {
                if (GenerateFloatPerPoint(new Vector3i(pos.X, x, pos.Y)) < SurfaceLevel)
                    return x;
            }

            return -1;
        }

        
        Vector3 interpolateVerts(Vector3 v1, Vector3 v2) {
            float t = (SurfaceLevel - GenerateFloatPerPointF(v1)) / (GenerateFloatPerPointF(v2) - GenerateFloatPerPointF(v1));
            return v1 + t * (v2-v1);
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
                    Vector3 Color = (0, 0.6f, 0.098f);
                    //Vector3 Color = new Vector3(Noise.CalcPixel1D(CubePosition.X,Scale)/255f,Noise.CalcPixel1D(CubePosition.Y,Scale)/255f,
                    //    Noise.CalcPixel1D(CubePosition.Z,Scale)/255f);
                    
                    Vector3 v1 = interpolateVerts((cubeCornerOffsets[a0] + CubePosition), (cubeCornerOffsets[b0] + CubePosition));
                    Vector3 v2 = interpolateVerts((cubeCornerOffsets[a1] + CubePosition), (cubeCornerOffsets[b1] + CubePosition));
                    Vector3 v3 = interpolateVerts((cubeCornerOffsets[a2] + CubePosition), (cubeCornerOffsets[b2] + CubePosition));
                    var dir = Vector3.Cross(v2 - v1, v3 - v1);
                    Vector3 norm = -Vector3.Normalize(dir);
                    lock (FinalTriangleVertices)
                    {
                        FinalTriangleVertices.AddRange(new List<Vector3>()
                            {v1+Offset, Color, norm, v2+Offset, Color, norm, v3+Offset, Color, norm});
                    }
                }
            }
        }
    }
}