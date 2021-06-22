using OpenTK.Mathematics;


namespace YEET{
    public static class Sun{
        private static Vector3 _currentPosition, _origin;
        private static float _radius, _length_one_day;
        static Sun(){

        }

        public static void SetOrbit(float radius, Vector3 origin, float length_one_day){
            _radius = radius;
            _origin = origin;
            _length_one_day = length_one_day;
            _currentPosition = origin; //TODO: delete that
        }

        public static Vector3 getSunPosition(){
            return new Vector3();
        }
    }
}