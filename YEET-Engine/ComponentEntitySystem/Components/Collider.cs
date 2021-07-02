using System.Net;
using OpenTK.Graphics.ES20;
using OpenTK.Mathematics;


namespace YEET
{
    public class AABox{

        public float minX,minY,minZ,maxX,maxY,maxZ;

        public Vector3 Point_one,Point_two;

        public AABox(Vector3 point_one,Vector3 point_two, Matrix4 model){
            Point_two = (new Vector4(point_two.X,point_two.Y,point_two.Z,0f)*model).Xyz;
            Point_one = (new Vector4(point_one.X,point_one.Y,point_one.Z,0f)*model).Xyz;
            if(point_one.X>point_two.X)
            {
                maxX=point_one.X;
                minX=point_two.X;
            }
            else{
                minX=point_one.X;
                maxX=point_two.X;
            }
            if(point_one.Y>point_two.Y)
            {
                maxY=point_one.Y;
                minY=point_two.Y;
            }
            else{
                minY=point_one.Y;
                maxY=point_two.Y;
            }

            if(point_one.Z>point_two.Z)
            {
                maxZ=point_one.Z;
                minZ=point_two.Z;
            }
            else{
                minZ=point_one.Z;
                maxZ=point_two.Z;
            }
        }
    }


    public class Collider : Component
    {

        public Vector3 Point_one,Point_two;
        
        public Collider(Entity owner, Vector3 point_one, Vector3 point_two) : base(owner)
        {
            Point_one= point_one;
            Point_two =point_two;
        }


        public AABox GetBox(){
            Matrix4 model = Owner.GetComponent<Transform>().ModelMatrix;
            return new AABox(Point_one,Point_two,model);
        }

        public bool CheckCollision(Vector3 point){
            Matrix4 model = Owner.GetComponent<Transform>().ModelMatrix;
            var Box = new AABox(Point_one,Point_two,model);
            return (point.X >= Box.minX && point.X <= Box.maxX) &&
                   (point.Y >= Box.minY && point.Y <= Box.maxY) &&
                   (point.Z >= Box.minZ && point.Z <= Box.maxZ);
            }
    }
}