using System;
using OpenTK.Mathematics;
using Vector3 = System.Numerics.Vector3;

namespace YEET
{
    
    public class Chunk
    {
        public Chunk(Vector3i position)
        {
            Position = position;
        }
        
        
        
        public Int64 ID
        {
            get
            {
                Int16 x_pos = Convert.ToInt16(Position.X/SpatialManager.ChunkSize);
                Int16 y_pos = Convert.ToInt16(Position.Y/SpatialManager.ChunkSize);
                Int16 z_pos = Convert.ToInt16(Position.Z/SpatialManager.ChunkSize);
                Int64 to_return = x_pos;
                to_return = (to_return << 16) + y_pos;
                return (to_return << 16) + z_pos;
            }
        }

        public Vector3i Position;

        public bool InViewDistance()
        {
            return Math.Abs((Position - Camera.Position).Length) <= Camera.RenderingDistance;
        }
    }
}