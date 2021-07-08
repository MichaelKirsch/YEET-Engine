using System;
using OpenTK.Mathematics;
using ImGuiNET;
using System.Xml;
using System.Collections.Generic;
namespace YEET.ComponentEntitySystem.Entities
{
    public class Square : Entity
    {
        private Guid mesh;
        public Vector3 Color;
        public Square(Vector3 point_one,Vector3 point_two,Vector3 color)
        {
            Name = "Square";
            var _loader = new ShaderLoader("Grid");
            Color = color;
            mesh = AddComponent(new Mesh(this,_loader));
            GetComponent<Mesh>(mesh).SetData(MakeForm.MakeQuad(new Vector3(point_one.X,0,point_one.Z),
            new Vector3(point_one.X,0,point_two.Z),
            new Vector3(point_two.X,0,point_two.Z),
            new Vector3(point_two.X,0,point_one.Z)),new List<Mesh.VertexAttribType>(){Mesh.VertexAttribType.V3});
            AddComponent(new Collider(this,point_one,new Vector3(point_two.X,point_two.Y+0.1f,point_two.Z)));
            

        }
        
        
        public override void OnGui()    
        {
            if (ShowGUI)
            {
                ImGui.Begin("Square"+ ID);
                ImGui.Checkbox("Active", ref Active);

                System.Numerics.Vector3 ref_c = new System.Numerics.Vector3(Color.X,Color.Y,Color.Z);
                ImGui.ColorEdit3("Color",ref ref_c);
                Color = new Vector3(ref_c.X,ref_c.Y,ref_c.Z);
                base.OnGui();
                ShowGUI = !ImGui.Button("Dont Show");
                ImGui.End();
            }
            ImGui.Checkbox("Square " + ID, ref ShowGUI);
        }


        public override void OnRender(){
            GetComponent<Mesh>(mesh).SetUniform("rgb",new Vector3(Color.X,Color.Y,Color.Z));
            GetComponent<Mesh>(mesh).OnDraw();
        }
        
    }
}