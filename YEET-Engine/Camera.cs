using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace YEET
{
    public static class Camera
    {
        static public Vector3 Position, Front, Up, Right, WorldUp;
        static public float Yaw, Pitch, MovementSpeed, MouseSensitivity, Zoom;
        static public Matrix4 View, Projection;
        static private Vector2 MousePrevious = Vector2.Zero;
        static private bool Grabbed;
        static public int Frustrum=45;
        static public float RenderingDistance = 100f;

        static Camera()
        {
            Position = new Vector3(0.0f, 0.0f, 0.0f);
            Up = new Vector3(0.0f, 1.0f, 0.0f);
            Yaw = -90.0f;
            Pitch = 0.0f;
            MovementSpeed = 2.5f;
            MouseSensitivity = 0.1f;
            Zoom = 90.0f;
        }

        public static void Start()
        {
            Console.WriteLine(StateMaschine.Context.MousePosition);
            GrabCursor(false);
            StateMaschine.Context.MousePosition =
                new Vector2(StateMaschine.Context.Size.X / 2, StateMaschine.Context.Size.Y / 2);
            MousePrevious = StateMaschine.Context.MousePosition;
        }

        public static void GrabCursor(bool state)
        {
            //StateMaschine.Context.CursorGrabbed = state;
            StateMaschine.Context.CursorVisible = !state;
            ResetCursor();
            Grabbed = state;
        }

        public static void ResetCursor()
        {
            StateMaschine.Context.MousePosition =
                new Vector2(StateMaschine.Context.Size.X / 2, StateMaschine.Context.Size.Y / 2);
            MousePrevious = new Vector2(StateMaschine.Context.Size.X / 2, StateMaschine.Context.Size.Y / 2);
        }

        public static void processMouse()
        {
            if (Grabbed)
            {
                Vector2 NewMouse = StateMaschine.Context.MousePosition;
                var delta = MousePrevious - NewMouse;
                ResetCursor();
                var deltaCompensated = delta * MouseSensitivity;

                Pitch += deltaCompensated.Y;
                Yaw -= deltaCompensated.X;
                if (Pitch > 89.0f)
                {
                    Pitch = 89.0f;
                }
                else if (Pitch < -89.0f)
                {
                    Pitch = -89.0f;
                }

                Front.X = (float) Math.Cos(MathHelper.DegreesToRadians(Pitch)) *
                          (float) Math.Cos(MathHelper.DegreesToRadians(Yaw));
                Front.Y = (float) Math.Sin(MathHelper.DegreesToRadians(Pitch));
                Front.Z = (float) Math.Cos(MathHelper.DegreesToRadians(Pitch)) *
                          (float) Math.Sin(MathHelper.DegreesToRadians(Yaw));
                Front = Vector3.Normalize(Front);
                Right = Vector3.Normalize(Vector3.Cross(Up, Front));
                Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Zoom),
                    StateMaschine.Context.Size.X / StateMaschine.Context.Size.Y, 1.01f, 1000f);
                View = Matrix4.LookAt(Position, Position + Front, Up);
            }
        }

        public static void ProcessKeyboard()
        {
            float velocity = 1.01f * Convert.ToSingle(StateMaschine.Context.LastFrameRenderTime);
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