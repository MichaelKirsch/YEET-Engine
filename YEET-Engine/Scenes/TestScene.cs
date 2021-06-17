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
        private Guid Grid, wellmodel;
        public bool ShowImGUIDemoWindow=false;
        public bool drawlines = false;
    
        public RenderingTest()
        {
            Console.WriteLine("State1 construct");
        }

        public override void OnStart()
        {
            wellmodel =AddEntity(new StaticOBJModel("Well", new Vector3(0, 0, 0),false));
            Grid = AddEntity(new Grid((100, 100), 0.02f));
            _LightPosition = new Vector3(100, 100, 0);
            Console.WriteLine("State1 onstart");
            Camera.Start();
            Camera.Position = (50, 50, 50);
            LightManager.OnStart();
            SpatialManager.GeneratedHeightNeg = 10;
            SpatialManager.GeneratedHeightPos = 20;

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
            ImGui.Checkbox("show random lines", ref drawlines);
            ImGui.Checkbox("Gui Demo Window", ref ShowImGUIDemoWindow);
            ImGui.Checkbox("Show Chunk Outlines", ref SpatialManager.ShowChunkOutline);
            ImGui.Checkbox("Show Chunk Frustrum", ref SpatialManager.ShowChunksInFrustrum);
            ImGui.Text($"Current Chunk ID {SpatialManager.GetTupelChunkIDAndInChunkPos(SpatialManager.GetIDfromWorldPos(Camera.Position))}");
            ImGui.Text($"Chunks {SpatialManager._Chunks.Count}");
            ImGui.Text($"Visible Chunks {SpatialManager.VisibleChunksAccordFrustum.Count}");
            ImGui.Text($"Mouse Delta:{StateMaschine.Context.MouseState.Scroll}");
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
            if (StateMaschine.Context.MouseState.IsButtonDown(MouseButton.Middle))
            {
                Camera.Grab();
            }
            else{
                Camera.Release();
            }
            if (StateMaschine.Context.KeyboardState[Keys.K])
                _WireFrame = !_WireFrame;
            
            base.OnInput();
            Camera.ProcessKeyboard();
            Camera.processMouse();
        }
    }
}