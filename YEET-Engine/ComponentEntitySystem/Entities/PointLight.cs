using OpenTK.Mathematics;

namespace YEET.ComponentEntitySystem.Entities
{
    public class PointLight : Component
    {
        public Vector3 Color;
        public Vector3 Offset;
        public float LightStrength;
        public PointLight(Entity owner, Vector3 color, Vector3 offset, float strength) : base(owner)
        {
            Color = color;
            Offset = offset;
            LightStrength = strength;
        }
    }
}