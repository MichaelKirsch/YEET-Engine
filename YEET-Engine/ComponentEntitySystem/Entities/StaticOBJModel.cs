using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace YEET.ComponentEntitySystem.Entities
{
    public class StaticOBJModel : Entity
    {
        public OBJLoader _loader;
        
        public StaticOBJModel(string path,Vector3 position, bool GuiVisible) : base(GuiVisible)
        {
            _loader = new OBJLoader(path, new ShaderLoader("Model", "FlatShadedModelVert",
                "FlatShadedModelFrag", true));
            GetComponent<Transform>().SetPosition(position);
            AddComponent(new RotateToObject(this));
        }
        
        public void ChangeModel(string path){
            _loader = new OBJLoader(path, new ShaderLoader("Model", "FlatShadedModelVert",
                "FlatShadedModelFrag", true));
        }

        public override void OnGui()
        {
            if (ShowGUI)
            {
                ImGui.Begin("OBJ"+ ID);
                ImGui.SetWindowFontScale(1.5f);
                ImGui.Checkbox("Active", ref Active);
                base.OnGui();
                ShowGUI = !ImGui.Button("Dont Show");
                ImGui.End();
            }
            ImGui.Checkbox("OBJ " + ID, ref ShowGUI);
        }

        public override void OnRender()
        {
            base.OnRender();
            if (Vector3.Distance(Camera.Position,GetComponent<Transform>().Position)>Camera.RenderingDistance)
                return;
            
            _loader.Draw(GetComponent<Transform>().ModelMatrix);
        }
    }
}