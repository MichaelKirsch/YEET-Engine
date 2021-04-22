using System;
using System.Collections.Generic;
using System.ComponentModel;
using OpenTK.Mathematics;


namespace YEET
{
    public static class SpatialManager
    {
        public static List<Chunk> _Chunks = new List<Chunk>();
        private static List<Vector3i> _ZeroedChunkPositions = new List<Vector3i>();
        private static Vector3i LastChunkPositionOfCamera = new Vector3i();
        private static LineBlob lines = new LineBlob();
        private static float LastViewDistance;
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

            Console.WriteLine($"Added {_ZeroedChunkPositions.Count} Zero Chunks");
        }


        public static void OnUpdate()
        {
            
            if (GetCurrentChunkOfCamera() == LastChunkPositionOfCamera && Convert.ToInt32(LastViewDistance) == Convert.ToInt32(Camera.RenderingDistance))
                return;
            int added = 0;

            if (Convert.ToInt32(LastViewDistance) != Convert.ToInt32(Camera.RenderingDistance))
            {
                LastViewDistance = Camera.RenderingDistance;
                GenerateZeroGrid();
            }
            
            LastChunkPositionOfCamera = GetCurrentChunkOfCamera();


            List<Vector3i> NeededChunks = new List<Vector3i>();

            foreach (var zeroedChunkPosition in _ZeroedChunkPositions)
            {
                bool found = false;
                NeededChunks.Add(zeroedChunkPosition + LastChunkPositionOfCamera);
                foreach (var chunk in _Chunks)
                {
                    if ((zeroedChunkPosition + LastChunkPositionOfCamera) == chunk.Position)
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    _Chunks.Add(new Chunk(zeroedChunkPosition + LastChunkPositionOfCamera));
                    added += 1;
                }
            }

            int removed = 0;
            Queue<Chunk> toremove = new Queue<Chunk>();
            foreach (var chunk in _Chunks)
            {
                bool found = NeededChunks.Contains(chunk.Position);
                if (!found)
                {
                    toremove.Enqueue(chunk);
                    removed++;
                }
            }

            while (toremove.Count>0)
            {
                _Chunks.Remove(toremove.Dequeue());
            }
            
            lines.Clear();
            foreach (var chunk in _Chunks)
            {
                lines.AddAxisAllignedCube(chunk.Position*32,ChunkSize,Colors.Green);
            }
            LineDrawer.AddBlob(lines);
            Console.WriteLine($"Added {added} chunks, {removed} deleted, Total {_Chunks.Count}");
        }
    }
}