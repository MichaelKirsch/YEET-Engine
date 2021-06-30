
using System;
using ImGuiNET;
using OpenTK.Mathematics;

namespace YEET
{
    public class RotateToObject : Component
    {
        public RotateToObject(Entity owner) : base(owner)
        {
        }

        public override void OnGUI()
        {

            if(ImGui.Button("Rotate to Object")){
                    var t =  Owner.GetComponent<Transform>().GetPosition();

                    var obj_to_cam = (t-Camera.Position).Normalized();

                    Camera.Pitch = Convert.ToSingle(Math.Asin(-obj_to_cam.Y));
                    Camera.Yaw = Convert.ToSingle(Math.Atan2(obj_to_cam.X,obj_to_cam.Z));
                }   
        }
    }
}