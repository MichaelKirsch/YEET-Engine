using OpenTK.Mathematics;
using YEET.Engine.Core;
using YEET.Engine.ECS;

namespace YEET
{
    public class MousePicker
    {
        public Entity lastPickedEntity;
        public float distanceToLastPicked;
        public float positionOfIntersectionLastPicked;

        public bool stupidIntersection()
        {
            return false;
        }
        
        public MousePicker(){

        }

        public Vector3 getIntersectionGround(){
            return calculateMouseRay();
        }
        public Vector3 getMouseVector(){
            return calculateMouseRay();
        }
        
        

        

        private Vector3 calculateMouseRay(){
            Vector2 mousePos = StateMaschine.Context.MouseState.Position;
            Vector2i screenSize = StateMaschine.Context.Size;
            Vector2 normCoords = normalizedDeviceCoords(mousePos,screenSize);
            Vector4 clipCoords = new Vector4(normCoords.X,normCoords.Y,-1f,1f);
            Vector4 eyeCoords = toEyeCoords(clipCoords);
            Vector3 worldRay = toWorldCoords(eyeCoords);
            return worldRay;
        }

    	private Vector3 toWorldCoords(Vector4 eyeCoords){
            Vector4 rayWorld = Camera.View*eyeCoords;
            Vector3 mouseRay = new Vector3(rayWorld.X,rayWorld.Y,rayWorld.Z);
            return mouseRay.Normalized();
        }

        private Vector4 toEyeCoords(Vector4 clipSpace){
            Matrix4 invertedProjection = new Matrix4();
            Matrix4.Invert(Camera.Projection,out invertedProjection);
            Vector4 eyeCoords = invertedProjection*clipSpace;
            return new Vector4(eyeCoords.X,eyeCoords.Y,-1f,0f);
        }

        private Vector2 normalizedDeviceCoords(Vector2 mousePos, Vector2 screenSize){
            float x = (2f*mousePos.X)/ screenSize.X -1f;
            float y = (2f*mousePos.Y)/screenSize.Y -1f;
            return new Vector2(x,-y);
        }
    }   
}