using System;
using System.Collections.Generic;

namespace YEET
{
    public class EntityManager
    {

        public EntityManager()
        {
            _entities = new List<Entity>();
        }

        private List<Entity> _entities;

        public Guid AddEntity(Entity toadd)
        {
            _entities.Add(toadd);
            return toadd.ID;
        }

        public T GetEntity<T>(Guid id_to_search) where T : Entity
        {
            foreach (var entity in _entities)
            {
                if (entity.ID == id_to_search )
                    return (T) entity;
            }
            return null;
        }
    }
    
    
}