using System.Collections.Generic;
using OpenTK.Mathematics;
using ImGuiNET;

namespace YEET{
    public  class Skybox : Entity{
        System.Guid _gradient;
        //private AxisAllignedCube _cube;
        public int size = 1000;
    
        public Skybox(){
            _gradient = AddComponent(new Gradient(this));
            
        }

        public override void OnGui()
        {
            base.OnGui();
        }

        public override void OnRender()
        {
            base.OnRender();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            GetComponent<Transform>().Position = Camera.Position;
        }
    }
}