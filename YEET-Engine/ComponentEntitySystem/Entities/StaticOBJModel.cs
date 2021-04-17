using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace YEET.ComponentEntitySystem.Entities
{
    public class StaticOBJModel : Entity
    {
        private OBJLoader _loader;
        
        public StaticOBJModel(string path,Vector3 position, bool GuiVisible) : base(GuiVisible)
        {
            _loader = new OBJLoader(path, new ShaderLoader("Model", "FlatShadedModelVert",
                "FlatShadedModelFrag", true));
            GetComponent<Transform>().SetPosition(position);
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
            Vector3 pos2 = GetComponent<Transform>().GetPosition();
            Vector3 camerapos2 = Camera.Position;
            Vector3 dire = new Vector3(pos2 - camerapos2);
            _loader.Position = pos2;
            if(dire.Length>Camera.RenderingDistance)
                return;
            
            if(MathHelper.RadiansToDegrees(MathHelper.Acos(Vector3.Dot(new Vector3(Camera.Front.X,0,Camera.Front.Z).Normalized(), 
                new Vector3(dire.X,0,dire.Z).Normalized())))> Camera.Frustrum/2f)
                return;
            
            
            _loader.Draw();
        }
    }
}