using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK;
using OpenTK.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using SimplexNoise;
using YEET.ComponentEntitySystem.Entities;
using Buffer = System.Buffer;


namespace YEET
{
    public class RenderingTest : State
    {
        private bool _MouseLocked, _WireFrame;
        private Vector3 _LightPosition;
        private LineDrawer _line;
        private LineDrawer.Line cameraPlayer;

        private Guid Grid, Terrain;

        public RenderingTest()
        {
            Console.WriteLine("State1 construct");
        }

        public override void OnStart()
        {
            Grid = AddEntity(new Grid((100, 100), 0.02f));
            Terrain = AddEntity(new MarchingCubeTerrain((5, 3, 5)));
            _LightPosition = new Vector3(100, 100, 0);
            Console.WriteLine("State1 onstart");
            GL.ClearColor(0.415f, 0.439f, 0.4f, 1.0f);
            Camera.Start();
            Camera.Position = (50, 50, 50);
            Camera.GrabCursor(false);
            _line = new LineDrawer(Vector3.One);
            cameraPlayer = _line.AddLine(ref Camera.Position, ref _LightPosition, Colors.Red);
            
            base.OnStart();
        }

        public override void OnGui()
        {
            base.OnGui();
            ImGui.Begin("Main");
            ImGui.SetWindowFontScale(1.5f);
            if (ImGui.Button("Exit"))
            {
                StateMaschine.Exit();
            }

            ImGui.Text("Avg Rendertime:" +
                       StateMaschine.Context.AverageLastFrameRenderTime.ToString("0.000") + "ms");

            ImGui.Checkbox("Wireframe", ref _WireFrame);
            ImGui.Checkbox("Mouse Lock", ref _MouseLocked);
            ImGui.End();
        }

        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);
            var st = new Vector3(Camera.Position.X, 0.5f, Camera.Position.Z);
            cameraPlayer.Start = st;
            cameraPlayer.End = st + new Vector3(Camera.Front.X, 0.0f, Camera.Front.Z).Normalized() * 30f;
        }

        public override void OnRender()
        {
            base.OnRender();

            _line.Draw();
        }
        

        public override void OnInput()
        {
            if (StateMaschine.Context.KeyboardState[Keys.Escape])
            {
                _MouseLocked = !_MouseLocked;
                Camera.GrabCursor(_MouseLocked);
            }

            if (StateMaschine.Context.KeyboardState[Keys.K])
                _WireFrame = !_WireFrame;


            base.OnInput();
            Camera.ProcessKeyboard();
            Camera.processMouse();
        }
    }
}