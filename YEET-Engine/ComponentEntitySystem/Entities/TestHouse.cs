using System;

namespace YEET.ComponentEntitySystem.Entities
{
    public class TestHouse : Entity
    {
        private Guid grassMesh, house;
        
        public TestHouse()
        {
            grassMesh = AddComponent(new Mesh());
            house = AddComponent(new Mesh())
        }
        
        
        
    }
}