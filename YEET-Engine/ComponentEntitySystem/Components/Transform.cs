
using System;
using OpenTK.Mathematics;

namespace YEET
{
    public class Transform : Component
    {
        public Transform(Entity owner) : base(owner)
        {
            Position = new Vector3();
            Console.WriteLine("Transform added");
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
        
        
        public Vector3 Position;
    }
}