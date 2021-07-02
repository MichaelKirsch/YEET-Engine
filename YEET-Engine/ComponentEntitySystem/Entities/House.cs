using System;
using OpenTK.Mathematics;
using ImGuiNET;
using System.Xml;
namespace YEET.ComponentEntitySystem.Entities
{
    public class House : Entity
    {
        private Guid grassMesh, house;
        
        public House()
        {
            grassMesh = AddComponent(new Mesh(this,"2x2_grass"));
            house = AddComponent(new Mesh(this,"house_type12"));
            AddComponent(new RotateToObject(this));
            AddComponent(new Collider(this,new Vector3(0,0,0),new Vector3(2,2,2)));
        }
        
        public House(Vector3 position)
        {
            grassMesh = AddComponent(new Mesh(this,"2x2_grass"));
            house = AddComponent(new Mesh(this,"house_type12"));
            GetComponent<Transform>().Position = position;
        }
        /// <summary>
        /// construct a house from a xml file
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        public House(String name,Vector3 position)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load($"Models/{name}.xml");
            Random t = new Random();
            var variants = doc.GetElementsByTagName("variant");
            
            var chosen = variants[t.Next(variants.Count-1)].FirstChild;

            for (int i = 0; i < chosen.ChildNodes.Count; i++)
            {
                AddComponent(new Mesh(this,chosen.ChildNodes[i].InnerText));
            }
            GetComponent<Transform>().Position = position;
        }

        public override void OnGui()    
        {
            if (ShowGUI)
            {
                ImGui.Begin("House"+ ID);
                ImGui.SetWindowFontScale(1.5f);
                ImGui.Checkbox("Active", ref Active);
                base.OnGui();
                ShowGUI = !ImGui.Button("Dont Show");
                ImGui.End();
            }
            ImGui.Checkbox("House " + ID, ref ShowGUI);
        }


        public override void OnRender(){
            foreach (var item in GetComponents<Mesh>())
            {
                item.OnDraw();
            }
        }
        
    }
}