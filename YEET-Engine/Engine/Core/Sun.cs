using OpenTK.Mathematics;
using ImGuiNET;

namespace YEET{
    public static class Sun{
        private static Vector3 _currentPosition, _origin,_currentColor;
        private static float _radius, _length_one_day;

        public static bool ShowGUI=false;
        static Sun(){
            _currentPosition = new Vector3(1000,1000,1000);
            _currentColor = new Vector3(1, 1f, 1f);
        }

        public static void SetOrbit(float radius, Vector3 origin, float length_one_day){
            _radius = radius;
            _origin = origin;
            _length_one_day = length_one_day;
            _currentPosition = origin; //TODO: delete that
        }

        public static void OnGui(){
            if (ShowGUI)
            {
                ImGui.Begin("Sun");
                ImGui.DragFloat("PosX", ref _currentPosition.X);
                ImGui.DragFloat("PosY", ref _currentPosition.Y);
                ImGui.DragFloat("PosZ", ref _currentPosition.Z);
                System.Numerics.Vector3 c = new System.Numerics.Vector3(_currentColor.X,_currentColor.Y,_currentColor.Z);
                ImGui.ColorEdit3("Color",ref c);
                _currentColor = new Vector3(c.X,c.Y,c.Z);
                ImGui.End();
            }
        }

        public static Vector3 getSunPosition(){
            return _currentPosition;
        }

        public static Vector3 getColor(){
            return _currentColor;
        }
    }
}