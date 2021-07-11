using System;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace YEET.Engine.Core
{
    public static class MakeForm
    {
        public static List<Vector3> MakeQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            var list = new List<Vector3>();

            list.Add(a);
            list.Add(b);
            list.Add(c);

            list.Add(c);
            list.Add(d);
            list.Add(a);

            return list;
        }
    }

    public class ColorHelper
    {
        public static Vector3 ConvertColor(System.Drawing.Color input){
            return new Vector3(input.R,input.G,input.B);
        }

        public static System.Numerics.Vector4 ConvertColor4(System.Drawing.Color input){
            return new System.Numerics.Vector4(input.R,input.G,input.B,1);
        }
    }

    public class AxisAllignedCube
    {

        public AxisAllignedCube() { }
        public AxisAllignedCube(Vector3 p1, Vector3 diagp1)
        {
            Vertices[0] = p1;
            Vertices[6] = diagp1;
            // float diagonal_length = Math.Abs((p1 - diagp1).Length);
            float dimensionY = Math.Abs(diagp1.Y - p1.Y);
            float dimensionX = Math.Abs(diagp1.X - p1.X);
            float dimensionZ = Math.Abs(diagp1.Z - p1.Z);

            Vertices[0] = p1;
            Vertices[1] = new Vector3(p1.X, p1.Y, p1.Z + dimensionZ);
            Vertices[2] = new Vector3(p1.X, p1.Y + dimensionY, p1.Z + dimensionZ);
            Vertices[3] = new Vector3(p1.X, p1.Y + dimensionY, p1.Z);

            Vertices[4] = new Vector3(p1.X + dimensionX, p1.Y, p1.Z);
            Vertices[5] = new Vector3(p1.X + dimensionX, p1.Y, p1.Z + dimensionZ);
            Vertices[6] = new Vector3(p1.X + dimensionX, p1.Y + dimensionY, p1.Z + dimensionZ);
            Vertices[7] = new Vector3(p1.X + dimensionX, p1.Y + dimensionY, p1.Z);

            foreach (var vertex in Vertices)
            {
                if (vertex.X > MaxX)
                    MaxX = vertex.X;
                if (vertex.Y > MaxY)
                    MaxY = vertex.Y;
                if (vertex.Z > MaxZ)
                    MaxZ = vertex.Z;
                if (vertex.X < MinX)
                    MinX = vertex.X;
                if (vertex.Y < MinY)
                    MinY = vertex.Y;
                if (vertex.Z < MinZ)
                    MinZ = vertex.Z;
            }
        }

        public bool AABB(AxisAllignedCube other)
        {
            return (MinX <= other.MaxX && MaxX >= other.MinX) &&
                   (MinY <= other.MaxY && MaxY >= other.MinY) &&
                   (MinZ <= other.MaxZ && MaxZ >= other.MinZ);
        }

        public float MaxX, MaxY, MaxZ, MinX, MinY, MinZ;
        public Vector3[] Vertices = new Vector3[8];
    }
}