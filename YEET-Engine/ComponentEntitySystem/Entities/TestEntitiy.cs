using System;
using OpenTK.Mathematics;

namespace YEET.ComponentEntitySystem.Entities
{
    public class TestEntitiy : Entity
    {
        public  TestEntitiy()
        {
            Console.WriteLine("Test Entitiy. Set pos to 1,10,1");
            GetComponent<Transform>().SetPosition(new Vector3(1,10,1));
            AddComponent(new LinearMover(this,Vector3.Zero,new Vector3(100,0,100),0.3f));
            AddComponent(new OBJComponent(this));
            Console.WriteLine(GetComponent<Transform>().GetPosition());
        }
    }
}