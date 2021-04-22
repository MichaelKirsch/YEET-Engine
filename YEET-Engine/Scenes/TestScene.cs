using System;
using System.Collections.Generic;
using System.Linq;
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
        private Guid Grid, Terrain;
        public bool ShowImGUIDemoWindow=false;
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
            LightManager.OnStart();
            LineBlob lineBlob = new LineBlob();
            lineBlob.AddAxisAllignedCube(Vector3.Zero, 100, Colors.Red);
            LineDrawer.AddBlob(lineBlob);
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
                AddEntity(new StaticOBJModel("CITY", new Vector3(0, 0, 0),false));
            }
            ImGui.Checkbox("Camera Gui", ref Camera.ShowGUI);
            ImGui.Checkbox("Wireframe", ref _WireFrame);
            StateMaschine.Context.WireMode(_WireFrame);
            ImGui.Checkbox("Mouse Lock", ref _MouseLocked);
            ImGui.Checkbox("Gui Demo Window", ref ShowImGUIDemoWindow);
            ImGui.Text($"Current Chunk {SpatialManager.GetCurrentChunkOfCamera()}");
            ImGui.Text($"Current Chunk (W) {SpatialManager.ConvertChunkToWorldCoordinates(SpatialManager.GetCurrentChunkOfCamera())}");
            float[] x = StateMaschine.Context.ListLastFrameTimes.ToArray();
            if (ShowImGUIDemoWindow)
            {
                ImGui.ShowDemoWindow();
            }
            ImGui.End();
        }

        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);

        }

        public override void OnRender()
        {
            base.OnRender();
            
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