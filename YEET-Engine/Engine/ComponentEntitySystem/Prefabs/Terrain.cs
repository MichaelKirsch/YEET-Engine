using System;
using System.Collections.Generic;
using YEET.Engine.Core;
using SimplexNoise;
using OpenTK.Mathematics;

namespace YEET.Engine.ECS{

    public class Terrain : Entity{
        
        UInt32 mesh;
        List<UInt32> ChunkIds = new List<UInt32>();
        
        public Terrain(int sizex,int sizey, float tree_density){
            var heights = Noise.Calc2D(sizex,sizey,0.01f);
            float factor = 0.1f;
            float sealevel = -5f;
            List<Vector3> vertices = new List<Vector3>();
            mesh = AddComponent(new Mesh(this,new ShaderLoader("Terrain")));
            var offset = new Vector3(Camera.Position.X,0,Camera.Position.Z);
            for(int x=0;x<sizex-1;x++){
                for(int y =0;y<sizey-1;y++){
                    vertices.AddRange(MakeForm.MakeQuad(
                        new Vector3(x,  (heights[x,y]*factor)+sealevel,y)+offset,
                        new Vector3(x+1,(heights[x+1,y]*factor)+sealevel,y)+offset,
                        new Vector3(x+1,(heights[x+1,y+1]*factor)+sealevel,y+1)+offset,
                        new Vector3(x,  (heights[x,y+1]*factor)+sealevel,y+1)+offset));
                }
            }   
            GetComponent<Mesh>(mesh).SetData(vertices,new List<Mesh.VertexAttribType>(){Mesh.VertexAttribType.V3});
        }

        public override void OnGui()
        {
            base.OnGui();
        }

        public override void OnRender()
        {
            GetComponent<Mesh>(mesh).SetUniform("rgb",ColorHelper.ConvertColor(System.Drawing.Color.Green));
            GetComponent<Mesh>(mesh).OnDraw();
            //base.OnRender();
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
        }

    }
}