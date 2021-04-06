
using System;
using System.Numerics;

namespace YEET
{
    public class Transform : Component
    {
        public Transform(Entity owner) : base(owner)
        {
            _position = new Vector2();
            Console.WriteLine("Transform added");
        }

        public void SetPosition(Vector2 newpos)
        {
            _position = newpos;
        }

        public Vector2 GetPosition()
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
            _position.X = Convert.ToSingle(y);
        }
        
        private Vector2 _position;
    }
}