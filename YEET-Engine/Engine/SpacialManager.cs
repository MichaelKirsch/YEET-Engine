using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using OpenTK.Mathematics;


namespace YEET
{
    public static class SpatialManager
    {
        public static List<Chunk> VisibleChunksAccordFrustum = new List<Chunk>();
        public static List<Chunk> _Chunks = new List<Chunk>();
        private static List<Vector3i> _ZeroedChunkPositions = new List<Vector3i>();
        private static Vector3i LastChunkPositionOfCamera = new Vector3i();
        private static LineBlob lines = new LineBlob();
        private static float LastViewDistance;
        private static bool _showoutline,_showfrustrum;
        public static bool ShowChunkOutline = false;

        public static bool ShowChunksInFrustrum = false;

        public static int ChunkSize
        {
            get { return 32; }
        }

        static SpatialManager()
        {
            GenerateZeroGrid();
            LastViewDistance = Camera.RenderingDistance;
        }


        public static Vector3i ConvertWorldToChunkCoordinates(Vector3 input)
        {
            return new Vector3i(Convert.ToInt32(input.X / ChunkSize),
                Convert.ToInt32(input.Y / ChunkSize),
                Convert.ToInt32(input.Z / ChunkSize));
        }

        public static Vector3 ConvertChunkToWorldCoordinates(Vector3i input)
        {
            return (new Vector3(input) * ChunkSize);
        }

        public static Vector3i GetCurrentChunkOfCamera()
        {
            return ConvertWorldToChunkCoordinates(Camera.Position);
        }


        private static void GenerateZeroGrid()
        {
            _ZeroedChunkPositions.Clear();
            int ViewDistanceInChunks = Convert.ToInt32(Camera.RenderingDistance / ChunkSize);
            for (int i = -ViewDistanceInChunks; i < ViewDistanceInChunks; i++)
            {
                for (int j = -ViewDistanceInChunks; j < ViewDistanceInChunks; j++)
                {
                    for (int k = -ViewDistanceInChunks; k < ViewDistanceInChunks; k++)
                    {
                        if (new Vector3(i * ChunkSize, j * ChunkSize, k * ChunkSize).Length < Camera.RenderingDistance)
                        {
                            //Add new Chunk
                            _ZeroedChunkPositions.Add(new Vector3i(i, j, k));
                        }
                    }
                }
            }
        }


        private static void GenerateOutlines()
        {
            if (ShowChunksInFrustrum)
            {
                foreach (var chunk in VisibleChunksAccordFrustum)
                {
                    lines.AddAxisAllignedCube(chunk.Position * 32, ChunkSize, Colors.Red);
                }
                    
            }
            else
            {
                foreach (var chunk in _Chunks)
                {
                    lines.AddAxisAllignedCube(chunk.Position * 32, ChunkSize, Colors.Green);
                }
                    
            }
            LineDrawer.AddBlob(lines);
        }

        public static bool FrustumCheckChunk(Vector3i pos_to_check)
        {
            return (MathHelper.RadiansToDegrees(MathHelper.Acos(Vector3.Dot(Camera.Front.Normalized(),
            (Camera.Position-ConvertChunkToWorldCoordinates(pos_to_check)).Normalized()))) > Camera.Frustrum / 2f);
        }

        private static void UpdateFrustrum()
        {
            VisibleChunksAccordFrustum = _Chunks.FindAll(chunk => FrustumCheckChunk(chunk.Position));
        }

        public static void OnUpdate()
        {
            UpdateFrustrum();
            if (GetCurrentChunkOfCamera() == LastChunkPositionOfCamera && Convert.ToInt32(LastViewDistance) == Convert.ToInt32(Camera.RenderingDistance) && _showoutline == ShowChunkOutline)
                return;

            if (Convert.ToInt32(LastViewDistance) != Convert.ToInt32(Camera.RenderingDistance))
            {
                LastViewDistance = Camera.RenderingDistance;
                GenerateZeroGrid();
            }

            LastChunkPositionOfCamera = GetCurrentChunkOfCamera();

            _Chunks.RemoveAll(chunk => !chunk.InViewDistance());

            Parallel.ForEach(_ZeroedChunkPositions, zeroedChunkPosition =>
            {
                if (!_Chunks.Exists(chunk => chunk.Position == zeroedChunkPosition + LastChunkPositionOfCamera))
                {

                    lock (_Chunks)
                    {
                        _Chunks.Add(new Chunk(zeroedChunkPosition + LastChunkPositionOfCamera));
                    }
                }
            });

            lines.Clear();
            _showoutline = ShowChunkOutline;
            _showfrustrum = ShowChunksInFrustrum;
            if (_showoutline || _showfrustrum)
            {
                GenerateOutlines();
            }
        }
    }
}