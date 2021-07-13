using System;
using System.Collections.Generic;

namespace YEET.Engine.ECS{

    public class Terrain : Entity{

        List<Guid> ChunkIds = new List<Guid>();
        public Terrain(int sizex,int sizey, float tree_density){
            

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
        }

    }
}