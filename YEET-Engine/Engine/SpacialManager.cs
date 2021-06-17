using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Vector3 = OpenTK.Mathematics.Vector3;


namespace YEET
{
    
    public static class SpatialManager
    {
        public static List<Chunk> VisibleChunksAccordFrustum = new List<Chunk>();
        public static List<Chunk> _Chunks = new List<Chunk>();
        private static List<Vector3i> _ZeroedChunkPositions = new List<Vector3i>();
        private static Vector3i LastChunkPositionOfCamera = new Vector3i();
        private static float LastViewDistance;
        private static bool _showoutline, _showfrustrum;
        public static bool ShowChunkOutline = false;
        public static int GeneratedHeightPos = 100, GeneratedHeightNeg = 100;
        public static bool ShowChunksInFrustrum = false;


        public const UInt16 ChunkSize = 32;

        public const UInt32 SideLengthWorld = 2097152; //2^21

        public const int MaxChunksPerSide = 65536;
        
        
        public static UInt64 GetIDfromWorldPos(Vector3i worldpos)
        {
            return Convert.ToUInt64(worldpos.Z+ worldpos.Y * SideLengthWorld + worldpos.X*Math.Floor(Math.Pow(2,42)));
        }   
        
        public static UInt64 GetIDfromWorldPos(Vector3 worldpos)
        {
            return Convert.ToUInt64(Convert.ToInt32(worldpos.Z)+ Convert.ToInt32(worldpos.Y) * SideLengthWorld + Convert.ToInt32(worldpos.X)*Math.Floor(Math.Pow(2,42)));
        }   
        
        
        public static UInt64 GetChunkIDfromWorldPos(Vector3 worldpos)
        {
            
            return GetIDfromWorldPos(worldpos / ChunkSize);
        }   

        public static UInt64 GetChunkIDfromWorldPos(Vector3i worldpos)
        {
            return GetIDfromWorldPos(worldpos / ChunkSize);
        }   
        
        
        

        public static Vector3i GetWorldPosFromID(UInt64 id)
        {
            var z = Convert.ToInt32(id % SideLengthWorld);
            var y = Convert.ToInt32(id / SideLengthWorld % SideLengthWorld);
            var x = Convert.ToInt32(id / Convert.ToUInt64(Math.Pow(2,42)));
            return new Vector3i(x, y, z);
        }

        

        public static Tuple<Vector3i, Vector3i> GetTupelChunkPosAndInChunkPos(UInt64 id)
        {
            var worldpos = GetWorldPosFromID(id);
            var posinChunk = new Vector3i(worldpos.X%ChunkSize,worldpos.Y%ChunkSize,worldpos.Z%ChunkSize);
            return Tuple.Create(GetChunkPosFromID(id), posinChunk);
        }
        
        public static Tuple<UInt64, Vector3i> GetTupelChunkIDAndInChunkPos(UInt64 id)
        {
            var worldpos = GetWorldPosFromID(id);
            var posinChunk = new Vector3i(worldpos.X%ChunkSize,worldpos.Y%ChunkSize,worldpos.Z%ChunkSize);
            return Tuple.Create(GetChunkIDfromWorldID(id), posinChunk);
        }
        

        public static UInt64 GetChunkIDfromWorldID(UInt64 world)
        {
            return GetChunkIDfromWorldPos(GetWorldPosFromID(world));
        }
        

        public static Vector3i GetChunkPosFromID(UInt64 id)
        {
            return GetWorldPosFromID(id) / ChunkSize;
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
                for (int j = -Convert.ToInt32(ViewDistanceInChunks * (GeneratedHeightNeg / 100f));
                    j < Convert.ToInt32(ViewDistanceInChunks * (GeneratedHeightPos / 100f));
                    j++)
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
            
        }

        public static bool FrustumCheckChunk(Vector3i pos_to_check)
        {
            return (MathHelper.RadiansToDegrees(MathHelper.Acos(Vector3.Dot(Camera.Front.Normalized(),
                        (Camera.Position - ConvertChunkToWorldCoordinates(pos_to_check)).Normalized()))) >
                    Camera.Frustrum / 2f);
        }

        private static void UpdateFrustrum()
        {
            VisibleChunksAccordFrustum = _Chunks.FindAll(chunk => FrustumCheckChunk(chunk.Position));
        }

        public static void OnUpdate()
        {
            UpdateFrustrum();
            if (GetCurrentChunkOfCamera() == LastChunkPositionOfCamera &&
                Convert.ToInt32(LastViewDistance) == Convert.ToInt32(Camera.RenderingDistance) &&
                _showoutline == ShowChunkOutline)
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

           
            _showoutline = ShowChunkOutline;
            _showfrustrum = ShowChunksInFrustrum;
            if (_showoutline || _showfrustrum)
            {
                GenerateOutlines();
            }
        }
    }
}