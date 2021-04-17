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
    public class RenderingTest : Scene
    {
        private bool _MouseLocked, _WireFrame;
        private Vector3 _LightPosition;
        private LineDrawer _line;
        private LineDrawer.Line cameraPlayer;
        private bool griddirection;
        private float gridheight;
        private Guid Grid, Terrain;

        public RenderingTest()
        {
            Console.WriteLine("State1 construct");
        }

        public override void OnStart()
        {
            AddEntity(new StaticOBJModel("Well", new Vector3(0, 0, 0),false));
            Grid = AddEntity(new Grid((100, 100), 0.02f));
            Terrain = AddEntity(new MarchingCubeTerrain((5, 3, 5)));
            _LightPosition = new Vector3(100, 100, 0);
            Console.WriteLine("State1 onstart");
            Camera.Start();
            Camera.Position = (50, 50, 50);
            Camera.GrabCursor(false);
            _line = new LineDrawer(Vector3.One);
            cameraPlayer = _line.AddLine(ref Camera.Position, ref _LightPosition, Colors.Red);
            
            base.OnStart();
        }

        public override void OnGui()
        {
            base.OnGui(); // call first for all entities
            ImGui.Begin("Main");
            ImGui.SetWindowFontScale(1.5f);
            if (ImGui.Button("Exit"))
            {
                StateMaschine.Exit();
            }

            ImGui.ColorEdit4("ClearColor", ref ClearColor);
            ImGui.Text("Avg Rendertime:" +
                       StateMaschine.Context.AverageLastFrameRenderTime.ToString("0.000") + "ms");

            if (ImGui.Button("Tree"))
            {
                AddEntity(new StaticOBJModel("Well", new Vector3(0, 0, 0),false));
            }
            ImGui.Checkbox("Camera Gui", ref Camera.ShowGUI);
            ImGui.Checkbox("Wireframe", ref _WireFrame);
            StateMaschine.Context.WireMode(_WireFrame);
            ImGui.Checkbox("Mouse Lock", ref _MouseLocked);
            ImGui.End();
        }

        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);
            var st = new Vector3(Camera.Position.X, 0.5f, Camera.Position.Z);
            cameraPlayer.Start = st;
            cameraPlayer.End = st + new Vector3(Camera.Front.X, 0.0f, Camera.Front.Z).Normalized() * 30f;

            if (gridheight > 20|| gridheight <0)
                griddirection = !griddirection;

            if (griddirection)
                gridheight += 0.1f;
            else
                gridheight -= 0.1f;

            GetEntity<Grid>(Grid).GetComponent<Transform>().SetY(gridheight);
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