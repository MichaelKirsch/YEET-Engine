using System;
using System.Collections.Generic;

namespace YEET.Engine.ECS{

    public class Terrain : Entity{

        List<Guid> ChunkIds = new List<Guid>();
        public Terrain(){

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