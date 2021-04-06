
using System;
using OpenTK.Mathematics;

namespace YEET
{
    public class Transform : Component
    {
        public Transform(Entity owner) : base(owner)
        {
            _position = new Vector3();
            Console.WriteLine("Transform added");
        }

        public void SetPosition(Vector3 newpos)
        {
            _position = newpos;
        }

        public Vector3 GetPosition()
        {
            return _position;
        }

        public void SetX(float x)
        {
            _position.X = x;
        }
        public void SetX(double x)
        {
            _position.X = Convert.ToSingle(x);
        }
        
        public void SetY(float y)
        {
            _position.Y = y;
        }
        public void SetY(double y)
        {
            _position.Y = Convert.ToSingle(y);
        }
        
        public void SetZ(float z)
        {
            _position.Z = z;
        }
        public void SetZ(double z)
        {
            _position.Z = Convert.ToSingle(z);
        }
        
        public Vector3 _position;
    }
}