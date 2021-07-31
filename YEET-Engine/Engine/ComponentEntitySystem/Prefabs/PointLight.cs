using OpenTK.Mathematics;

namespace YEET.Engine.ECS
{
    public class PointLight : Component
    {
        private Vector3 ColorStrengthCombination; //the color is not limited to range 0-1 as this is the indication for the strength.
        //You can calculate the strength by doing modulo 255. 

        private Vector3 _color;
        private Vector3 _offset;
        private float _strength;

        public Vector3 Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                CalculateColorStrength();
            }
        }

        public Vector3 Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
                CalculateColorStrength();
            }
        }
        
        public float Strength
        {
            get
            {
                return _strength;
            }
            set
            {
                _strength = value;
                CalculateColorStrength();
            }
        }
        
        public Vector3 WorldPosition
        {
            get
            {
                return Owner.GetComponent<Transform>().Position + Offset;
            }
        }
        
        private void CalculateColorStrength()
        {
            ColorStrengthCombination = _color * _strength;
        }

        public PointLight(Entity owner, Vector3 color, Vector3 offset, float strength) : base(owner)
        {
            Color = color;
            Offset = offset;
            Strength = strength;
        }
    }
}