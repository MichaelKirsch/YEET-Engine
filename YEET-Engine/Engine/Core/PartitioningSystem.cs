using System.Collections.Generic;
using YEET.Engine.ECS;

namespace YEET.Engine.Core
{
    public interface PartitioningSystem
    {
        public void InsertEntity(Entity to_add){} //add an entity to the partitioning system
        public void RemoveEntity(Entity to_add){} //remove an entity (when its deleted)
        public void OnUpdate(){} //update the camera position (reload chunks and all that stuff)
        public List<Entity> GetCloseEntities(Entity to_check, float range)
        {
            return new();
        }
    }
}