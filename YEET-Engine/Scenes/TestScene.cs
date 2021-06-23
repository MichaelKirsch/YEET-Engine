using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK;
using OpenTK.Input;
using OpenTK.Windowing.Common;
using SimplexNoise;
using YEET.ComponentEntitySystem.Entities;
using Buffer = System.Buffer;
using System.IO;
using System.Drawing;
using System.Numerics;
using Vector3 = OpenTK.Mathematics.Vector3;


namespace YEET
{
    public class RenderingTest : Scene
    {
        private bool _WireFrame,wasdown;
        private Vector3 _LightPosition;
        private Guid Grid, wellmodel;
        public bool ShowImGUIDemoWindow = false;
        public bool drawlines = false;
        bool build_mode = false;
        private Random rnd = new Random();
        private string current_build = "";
        
        SimpleTexturedButton house1 = new SimpleTexturedButton();
        SimpleTexturedButton house2 = new SimpleTexturedButton();

        SimpleTexturedButton street1 = new SimpleTexturedButton();
        
        private MousePicker picker = new MousePicker();
        public RenderingTest()
        {
            Console.WriteLine("State1 construct");
        }

        public override void OnStart()
        {
            wellmodel = AddEntity(new StaticOBJModel("house_type01", new Vector3(0, 0, 0), false));
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
            if (ImGui.Button("Exit"))
            {
                StateMaschine.Exit();
            }

            ImGui.ColorEdit4("ClearColor", ref ClearColor);
            ImGui.Text("Avg Rendertime:" +
                       StateMaschine.Context.AverageLastFrameRenderTime.ToString("0.000") + "ms");

            if (ImGui.Button("Tree"))
            {

                var listof_obj = Directory.GetFiles("Models/", "*.obj");
                for (int f = 0; f < 1000; f++)
                {
                    var item = listof_obj[rnd.Next(0, listof_obj.Length - 1)];
                    var t = AddEntity(new StaticOBJModel(item.Remove(item.Length - 4).Substring(7), new Vector3(0, 0, 0), false));
                    GetEntity(t).GetComponent<Transform>().SetPosition(new Vector3(rnd.Next(0, 100), 0, rnd.Next(0, 100)));
                }

            }
            ImGui.Checkbox("Camera Gui", ref Camera.ShowGUI);
            ImGui.Checkbox("Wireframe", ref _WireFrame);
            StateMaschine.Context.WireMode(_WireFrame);
            ImGui.Checkbox("Print Profiler", ref Profiler.DebugPrint);
            ImGui.Checkbox("show random lines", ref drawlines);
            ImGui.Checkbox("Gui Demo Window", ref ShowImGUIDemoWindow);
            ImGui.Checkbox("Show Chunk Outlines", ref SpatialManager.ShowChunkOutline);
            ImGui.Checkbox("Show Profiler Window", ref Profiler.RenderProfiler);
            ImGui.Text($"Current Chunk ID {SpatialManager.GetTupelChunkIDAndInChunkPos(SpatialManager.GetIDfromWorldPos(Camera.Position))}");
            ImGui.Text($"Chunks {SpatialManager._Chunks.Count}");
            ImGui.Text($"Visible Chunks {SpatialManager.VisibleChunksAccordFrustum.Count}");
            ImGui.Text($"Mouse Delta:{picker.getIntersectionGround()}");
            if(house1.Draw("Models/house_type01.png",new Vector2(100,100))){
                    build_mode = true;
                    GetEntity<StaticOBJModel>(wellmodel).ChangeModel("house_type01");
                    current_build = "house_type01";
            }
            if(house2.Draw("Models/house_type02.png",new Vector2(100,100))){
                    build_mode = true;
                    GetEntity<StaticOBJModel>(wellmodel).ChangeModel("house_type02");
                    current_build = "house_type02";
            }

            if(street1.Draw("Models/road_straight.png",new Vector2(100,100))){        
                    build_mode = true;
                    GetEntity<StaticOBJModel>(wellmodel).ChangeModel("road_straight");
                    current_build = "road_straight";
            }

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
            else
            {
                Camera.Release();
            }
            if (StateMaschine.Context.KeyboardState[Keys.K])
                _WireFrame = !_WireFrame;

            base.OnInput();
            Camera.ProcessKeyboard();
            Camera.processMouse();
            if(build_mode){
            for(int x=0;x<100;x++){
                var ray = picker.getIntersectionGround();
                Vector3 pos = ray.Normalized()*x+Camera.Position;
                if(pos.Y<0)
                {
                    if(StateMaschine.Context.MouseState.IsButtonDown(MouseButton.Left)){
                        
                        if(!wasdown)
                        {
                            if(current_build!=""){
                                var t = AddEntity(new StaticOBJModel(current_build,new Vector3(Convert.ToInt16(pos.X),0,Convert.ToInt16(pos.Z)),false));
                                build_mode = false;
                            }
                            
                        }
                        wasdown=true;   
                    }
                    else{
                        wasdown=false;
                    }
                        GetEntity(wellmodel).GetComponent<Transform>().SetPosition(new Vector3(Convert.ToInt16(pos.X),0,Convert.ToInt16(pos.Z)));
                    break;
                }
            }
            }
        }
    }
}