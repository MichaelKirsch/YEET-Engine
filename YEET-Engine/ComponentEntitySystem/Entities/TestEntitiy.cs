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
            Console.WriteLine(GetComponent<Transform>().GetPosition());
        }
    }
}