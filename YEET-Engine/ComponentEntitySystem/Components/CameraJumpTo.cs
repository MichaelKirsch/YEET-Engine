
using System;
using ImGuiNET;
using OpenTK.Mathematics;

namespace YEET
{
    public class CameraJumpTo : Component
    {
        public CameraJumpTo(Entity owner) : base(owner)
        {
        }

        public override void OnGUI()
        {
            if(ImGui.Button("Jump to pos")){
                    var t =  Owner.GetComponent<Transform>().GetPosition();
                    Camera.Position = new Vector3(t.X,10,t.Z);
                }   
        }
    }
}