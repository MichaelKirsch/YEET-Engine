using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace YEET
{
    public class Camera
    {
        public Vector3 Position, Front, Up, Right, WorldUp;
        public float Yaw,Pitch,MovementSpeed,MouseSensitivity,Zoom;
        private Vector2 delta,prev_mouse;
        public Matrix4 View, Projection;
        public Camera()
        {
            Position = new Vector3(0.0f, 0.0f, 0.0f);
            Up = new Vector3(0.0f, 1.0f, 0.0f);
            Yaw = -90.0f;
            Pitch = 0.0f;
            MovementSpeed = 2.5f;
            MouseSensitivity = 0.1f;
            Zoom = 90.0f;
        }
        

        
        
        public void Reset()
        {
            StateMaschine.Context.MousePosition = Vector2.Zero;
            prev_mouse = Vector2.Zero;
        }

        public void processMouse()
        {
            var new_mouse = StateMaschine.Context.MouseState.Position;

            delta = new_mouse - prev_mouse;

            prev_mouse = new_mouse;
            
            
            var deltaCompensated = delta * MouseSensitivity;
            Pitch -= deltaCompensated.Y;
            Yaw += deltaCompensated.X;
            if(Pitch > 89.0f)
            {
                Pitch = 89.0f;
            }
            else if(Pitch < -89.0f)
            {
                Pitch = -89.0f;
            }
            Front.X = (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(Yaw));
            Front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(Pitch));
            Front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(Yaw));
            Front = Vector3.Normalize(Front);
            Right = Vector3.Normalize(Vector3.Cross(Up, Front));
            Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Zoom), StateMaschine.Context.Size.X/StateMaschine.Context.Size.Y, 1.01f, 1000f);
            View = Matrix4.LookAt(Position,Position+Front,Up);
        }
        public void ProcessKeyboard()
        {
        float velocity = 1.01f*Convert.ToSingle(StateMaschine.Context.LastFrameRenderTime);
            if (StateMaschine.Context.KeyboardState[Keys.W])
                Position += Front * velocity;
            if (StateMaschine.Context.KeyboardState[Keys.S])
                Position -= Front * velocity;
            if (StateMaschine.Context.KeyboardState[Keys.A])
                Position += Right * velocity;
            if (StateMaschine.Context.KeyboardState[Keys.D])
                Position -= Right * velocity;
        }
        
        
        
    }
}