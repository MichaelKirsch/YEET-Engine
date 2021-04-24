using System;
using OpenTK.Mathematics;

namespace YEET
{
    public class Voxel{
        private Vector3 m_PositionWorldCoordinates;
        public Voxel(Chunk area){
            m_PositionWorldCoordinates = SpatialManager.ConvertChunkToWorldCoordinates(area.Position);
            
        }
    }
}