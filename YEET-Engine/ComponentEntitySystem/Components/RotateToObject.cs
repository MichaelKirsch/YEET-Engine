
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

        public void JumpToObject(){
            var t =  Owner.GetComponent<Transform>().GetPosition();
                    Camera.Position = new Vector3(t.X+1,t.Y+1,t.Z+1);
            SnapCameraToObject();
        }

        public void SnapCameraToObject(){
            var t =  Owner.GetComponent<Transform>().GetPosition();

            var obj_to_cam = (t-Camera.Position).Normalized();

            Camera.Pitch = MathHelper.RadiansToDegrees(Convert.ToSingle(Math.Asin(obj_to_cam.Y))); 
            Camera.Yaw = MathHelper.RadiansToDegrees(Convert.ToSingle(Math.Atan2(obj_to_cam.X,obj_to_cam.Z)));
        }

        public override void OnGUI()
        {

            if(ImGui.Button("Rotate to Object")){
                    SnapCameraToObject();
                }   

            if(ImGui.Button("Jump to Object")){
                    JumpToObject();
                }   
        }
    }
}