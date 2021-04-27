
using System;
using ImGuiNET;
using OpenTK.Mathematics;

namespace YEET
{
    public class Transform : Component
    {
        public Transform(Entity owner) : base(owner)
        {
            Position = new Vector3();
            Console.WriteLine("Transform added");
            Matrix4.CreateTranslation(0, 0, 0,out ModelMatrix);
        }

        public void SetPosition(Vector3 newpos)
        {
            Position = newpos;
        }

        public Vector3 GetPosition()
        {
            return Position;
        }

        public void SetX(float x)
        {
            Position.X = x;
        }
        public void SetX(double x)
        {
            Position.X = Convert.ToSingle(x);
        }
        
        public void SetY(float y)
        {
            Position.Y = y;
        }
        public void SetY(double y)
        {
            Position.Y = Convert.ToSingle(y);
        }
        
        public void SetZ(float z)
        {
            Position.Z = z;
        }
        public void SetZ(double z)
        {
            Position.Z = Convert.ToSingle(z);
        }

        public float GetZ()
        {
            return Position.Z;
        }
        
        public float GetX()
        {
            return Position.X;
        }
        
        public float GetY()
        {
            return Position.Y;
        }

        public void RotX(float to)
        {
            Matrix4.CreateRotationY(to, out ModelMatrix);
        }
        public void RotY(float to)
        {
            Matrix4.CreateRotationY(to, out ModelMatrix);
        }
        public void RotZ(float to)
        {
            Matrix4.CreateRotationY(to, out ModelMatrix);
        }
        public override void OnGUI()
        {
            
            ImGui.Text("Transform");
            ImGui.DragFloat("Pos X", ref Position.X);
            ImGui.DragFloat("Pos Y", ref Position.Y);
            ImGui.DragFloat("Pos Z", ref Position.Z);
            ImGui.DragFloat("Rot X", ref Rotation.X);
            ImGui.DragFloat("Rot Y", ref Rotation.Y);
            ImGui.DragFloat("Rot Z", ref Rotation.Z);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            //Matrix4.CreateFromAxisAngle(Rotation,1,out ModelMatrix);
            var TRans = Matrix4.CreateTranslation(Position);
            ModelMatrix = TRans;
        }
        
        private Vector3 Rotation;
        public Vector3 Position;
        public Matrix4 ModelMatrix;
    }
}