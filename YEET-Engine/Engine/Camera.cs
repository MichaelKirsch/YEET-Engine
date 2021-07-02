using System;
using ImGuiNET;
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
        public static bool ProcessMouseMovement = false;
        static public int Frustrum = 45;
        static public float RenderingDistance = 100f;
        private static double lasttime;
        static public bool ShowGUI;
        static public bool WasDown = false;
        private static Vector2 PositionWhenLockStarted;

        public static bool InvertY = false;

        static Camera()
        {
            Position = new Vector3(0.0f, 0.0f, 0.0f);
            Up = new Vector3(0.0f, 1.0f, 0.0f);
            Yaw = -90.0f;
            Pitch = 0.0f;
            MovementSpeed = 2.5f;
            MouseSensitivity = 0.2f;
            Zoom = 70.0f;
            Frustrum = Convert.ToInt32(Zoom + 10);
        }

        public static void Start()
        {
            PositionWhenLockStarted = StateMaschine.Context.MouseState.Position;
        }

        public static void Grab()
        {
            ProcessMouseMovement = true;
        }

        public static void Release()
        {
            ProcessMouseMovement = false;
        }

        public static void processMouse()
        {
            if (StateMaschine.Context.MouseState.IsButtonDown(MouseButton.Middle))
            {
                if (!WasDown)
                {
                    PositionWhenLockStarted = StateMaschine.Context.MouseState.Position;
                }
                WasDown = true;
                Vector2 delta = new Vector2();
                delta.X = (-StateMaschine.Context.MouseState.Position.X + PositionWhenLockStarted.X);
                if (InvertY)
                {
                    delta.Y = (StateMaschine.Context.MouseState.Position.Y + PositionWhenLockStarted.Y);
                }
                else
                {
                    delta.Y = (-StateMaschine.Context.MouseState.Position.Y + PositionWhenLockStarted.Y);
                }
                var deltaCompensated = delta * MouseSensitivity;
                PositionWhenLockStarted = StateMaschine.Context.MouseState.Position;
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
                
            }
            else{
                WasDown = false;
            }
           
             Front.X = (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) *
                            (float)Math.Cos(MathHelper.DegreesToRadians(Yaw));
            Front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(Pitch));
            Front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) *
                        (float)Math.Sin(MathHelper.DegreesToRadians(Yaw));
            Front = Vector3.Normalize(Front);
            Right = Vector3.Normalize(Vector3.Cross(Up, Front));
            Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Zoom),
            Convert.ToSingle(StateMaschine.Context.Size.X) / Convert.ToSingle(StateMaschine.Context.Size.Y),
            1.01f, 1000f);
            View = Matrix4.LookAt(Position, Position + Front, Up);
        }


        public static void ProcessKeyboard()
        {
            float velocity = Math.Abs(.11f * Convert.ToSingle(StateMaschine.GetElapsedTime() - lasttime));
            lasttime = StateMaschine.GetElapsedTime();
            if (StateMaschine.Context.KeyboardState[Keys.W])
                Position += new Vector3(Front.X, 0, Front.Z).Normalized() * velocity;
            if (StateMaschine.Context.KeyboardState[Keys.S])
                Position -= new Vector3(Front.X, 0, Front.Z).Normalized() * velocity;
            if (StateMaschine.Context.KeyboardState[Keys.A])
                Position += new Vector3(Right.X, 0, Right.Z).Normalized() * velocity;
            if (StateMaschine.Context.KeyboardState[Keys.D])
                Position -= new Vector3(Right.X, 0, Right.Z).Normalized() * velocity;
            Position.Y = StateMaschine.Context.MouseState.Scroll.Y * -4;
            if (Position.X < 0)
                Position.X = 0;
            if (Position.Y < 0)
                Position.Y = 0;
            if (Position.Z < 0)
                Position.Z = 0;
        }

        public static void OnGui()
        {
            if (ShowGUI)
            {
                ImGui.Begin("Camera");
                ImGui.DragFloat("PosX", ref Position.X);
                ImGui.DragFloat("PosY", ref Position.Y);
                ImGui.DragFloat("PosZ", ref Position.Z);
                ImGui.DragFloat("Pitch", ref Pitch);
                ImGui.DragFloat("Yaw", ref Yaw);
                ImGui.DragFloat("View Distance", ref RenderingDistance);
                ImGui.SliderFloat("FOV", ref Zoom, 45f, 110f);
                ImGui.End();
            }
        }
    }
}