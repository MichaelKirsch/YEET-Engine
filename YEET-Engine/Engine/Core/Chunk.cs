using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace YEET
{
    
    public class Chunk
    {

        private List<Entity> staticEntities = new List<Entity>();
        
        public float DistanceToCamera = 0;
        public bool IsEmpty = true;
        public bool IsVisible=false;

        public float[,,] LightCloud =
            new float[SpatialManager.ChunkSize, SpatialManager.ChunkSize, SpatialManager.ChunkSize];
        //public 3d lighting Texture
        //public 3d collision Texture
        
        public Chunk(Vector3i position)
        {
            Position = position;
        }

        public void SubscribeStaticEntityToChunk(Entity to_add)
        {
            staticEntities.Add(to_add);
        }
        

        public Vector3i Position;

        public bool InViewDistance()
        {
            return Math.Abs((Position*32 - Camera.Position).Length) <= Camera.RenderingDistance;
        }
    }
}