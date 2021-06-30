using System;
using OpenTK.Mathematics;

namespace YEET
{
    struct BoundingBox
    {
        public BoundingBox(Vector3[] input)
        {
            Vertices = input;
        }
        public Vector3[] Vertices;
    }
    
    
    public class BoundingBoxComponent : Component
    {
        BoundingBoxComponent(Entity owner) : base(owner)
        {
            
        }
    }
}