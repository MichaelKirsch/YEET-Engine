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
using YEET.Engine.ECS;
using Buffer = System.Buffer;
using System.IO;
using System.Drawing;
using System.Numerics;
using OpenTK.Windowing.Common.Input;
using YEET.Engine.Core;
using Vector3 = OpenTK.Mathematics.Vector3;


namespace YEET
{
    public class MainGame : Engine.Core.Scene
    {
        private bool _WireFrame, wasdown;
        private Vector3 _LightPosition;
        private Guid Grid, wellmodel, tmp;
        public bool ShowImGUIDemoWindow = false;
        public bool drawlines = false;
        bool build_mode = false;
        bool draw_mode = false;
        private Vector3 last_pos;
        private Random rnd = new Random();
        private string current_build = "";

        private Area.AreaType currentAreaType;

        SimpleTexturedButton house1 = new SimpleTexturedButton();
        SimpleTexturedButton house2 = new SimpleTexturedButton();
        SimpleTexturedButton street1 = new SimpleTexturedButton();
        Vector3 first_click, last_click;
        SimpleTexturedButton pillar = new SimpleTexturedButton();

        private MousePicker picker = new MousePicker();
        public MainGame()
        {
            Console.WriteLine("State1 construct");
        }

        public override void OnStart()
        {
            Camera.Position = new Vector3(3000, 0, 3000);
            
            var groundplane = AddEntity(new Square(new Vector3(0, -0.1f, 0), new Vector3(6000, -0.1f, 6000), new Vector3(0f, 0, 0f)));
            //var skyplane = AddEntity(new Square(new Vector3(0, 500f, 0), new Vector3(6000, 500f, 6000), new Vector3(0f, 0, 0f)));
            AddEntity(new Terrain(3000,3000,1));
            //skybox = AddEntity(new Skybox());
            tmp = AddEntity(new House("small_house", Camera.Position));
            wellmodel = AddEntity(new StaticOBJModel("house_type01", new Vector3(Camera.Position.X - 100, 0, Camera.Position.X - 100), false));
            Grid = AddEntity(new Grid((100, 100), 0.02f));
            GetEntity(Grid).GetComponent<Transform>().Position = (3000, 0.01f, 3000);
            _LightPosition = new Vector3(100, 100, 0);
            Console.WriteLine("State1 onstart");
            Camera.Start();
            SpatialManager.GeneratedHeightNeg = 10;
            SpatialManager.GeneratedHeightPos = 20;
            CursorControl.SetPredefined(MouseCursor.Hand);
            if (true) //testing
            {
                AddEntity(new Area(new Vector3(3000,0,3000), new Vector3(3200,0,3200), new Vector3()));
            }
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
            ImGui.Text("FPS:" +
                       (1000f/StateMaschine.Context.AverageLastFrameRenderTime).ToString("0.0") + "fps");
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
            ImGui.Checkbox("Sun Gui", ref Sun.ShowGUI);
            ImGui.Checkbox("Gui Demo Window", ref ShowImGUIDemoWindow);
            ImGui.Checkbox("Show Profiler Window", ref Profiler.RenderProfiler);
            ImGui.Text($"Current Chunk ID {SpatialManager.GetTupelChunkIDAndInChunkPos(SpatialManager.GetIDfromWorldPos(Camera.Position))}");
            ImGui.Text($"Chunks {SpatialManager._Chunks.Count}");
            ImGui.Text($"Active Entities {EntitiesCount()}");
            ImGui.Text($"Visible Chunks {SpatialManager.VisibleChunksAccordFrustum.Count}");
            ImGui.Text($"Mouse Delta:{picker.getIntersectionGround()}");
            ImGui.Separator();
            ImGui.Text("Framebuffer");
            ImGui.Image(new System.IntPtr(StateMaschine.GetCurrentScene().texture),new Vector2(300,150),new Vector2(1,1),new Vector2(0,0));
            ImGui.Text("Depthbuffer");
            ImGui.Image(new System.IntPtr(StateMaschine.GetCurrentScene().depth_texture),new Vector2(300,150),new Vector2(1,1),new Vector2(0,0));

            StateMaschine.Context.WireMode(_WireFrame);
            if (build_mode)
            {
                StateMaschine.Context.CursorVisible = false;
            }

            float[] x = StateMaschine.Context.ListLastFrameTimes.ToArray();
            if (ShowImGUIDemoWindow)
            {
                ImGui.ShowDemoWindow();
            }
            ImGui.End();

            ImGui.SetNextWindowSize(new Vector2(1000, 120));
            ImGui.SetNextWindowPos(new Vector2(StateMaschine.Context.Size.X / 2f - 500, StateMaschine.Context.Size.Y - 200));
            ImGui.Begin("", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar);
            if (house1.Draw("Data/Icons/MiningIcons_33_t.PNG", new Vector2(100, 100)))
            {
                build_mode = false;
                draw_mode = true;
                currentAreaType = Area.AreaType.SmallSuburb;
            }
            ImGui.SameLine();
            if (house2.Draw("Data/Models/bridge_pillar.png", new Vector2(100, 100)))
            {
                build_mode = false;
                draw_mode = true;
                currentAreaType = Area.AreaType.DenseHouses;
            }
            ImGui.End();
        
            InstanceRenderer.OnGui();
            
            

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
            if (ImGui.GetIO().WantCaptureMouse) { return; };//if the mouse is over gui dont act
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


            Vector3 pos = new Vector3();
            Vector3 pos_raw = new Vector3();
            for (int x = 0; x < 100; x++)
            {
                var ray = picker.getIntersectionGround();
                pos = ray.Normalized() * x + Camera.Position;
                pos_raw = pos;
                if (pos.Y < 0)
                {
                    pos.Y = 0.0f;
                    pos.X = Convert.ToSingle(Math.Floor(pos.X));
                    pos.Z = Convert.ToSingle(Math.Floor(pos.Z));
                    break;
                }
            }

            if (!build_mode && !draw_mode)
            {
                foreach (var item in GetEntitiesByType<Area>())
                {
                    if (item.GetComponent<Collider>().CheckCollision(new Vector3(pos_raw.X, 0.04f, pos_raw.Z)))
                    {
                        item.Color = new Vector3(0.231f, 0.231f, 0.231f);
                        item.GetComponent<Transform>().SetY(+0.02f);
                        if (StateMaschine.Context.MouseState.IsButtonDown(MouseButton.Left))
                        {
                            item.Color = new Vector3(0.478f, 0.752f, 0.207f);
                            selected = item;
                        }
                    }
                    else
                    {
                        item.Color = new Vector3(0.411f, 0.411f, 0.411f);
                        item.GetComponent<Transform>().SetY(-0.03f);
                    }
                }
            }







            if (draw_mode)
            {

                if (!wasdown && StateMaschine.Context.MouseState.IsButtonDown(MouseButton.Left))
                {
                    first_click = new Vector3(pos.X,0.2f,pos.Z);
                    wasdown = true;
                    tmp = AddEntity(new Square(first_click, first_click, ColorHelper.ConvertColor(Color.Aqua)));
                }

                if (wasdown && StateMaschine.Context.MouseState.IsButtonDown(MouseButton.Left))
                {
                    if (last_pos != pos)
                    {
                        last_pos = pos;
                        tmp = ChangeEntity(tmp, new Square(first_click, pos, ColorHelper.ConvertColor(Color.Aqua)));
                    }
                }
                if (wasdown && !StateMaschine.Context.MouseState.IsButtonDown(MouseButton.Left))
                {
                    last_click = new Vector3(pos.X,0.2f,pos.Z);;
                    for (int x = 0; x < (last_pos - first_click).Length; x += 2)
                    {
                        var dir = (last_pos - first_click).Normalized();
                    }
                    wasdown = false;
                    draw_mode = false;
                    Vector3 distance = last_click - first_click;
                    RemoveEntity(tmp);
                    switch (currentAreaType)
                    {
                        case Area.AreaType.SmallSuburb:
                            AddEntity(new Area(first_click, last_click, ColorHelper.ConvertColor(Color.Aqua)));
                            break;
                        default:
                            AddEntity(new Area(first_click, last_click, ColorHelper.ConvertColor(Color.Aqua), Area.AreaType.DenseHouses));
                            break;
                    }

                }
            }

            if (!build_mode)
            {
                GetEntity(wellmodel).Active = false;
            }

            else
            {
                if (StateMaschine.Context.MouseState.IsButtonDown(MouseButton.Left))
                {

                    if (!wasdown)
                    {
                        if (current_build != "")
                        {
                            //var t = AddEntity(new StaticOBJModel(current_build,new Vector3(Convert.ToInt16(pos.X),0,Convert.ToInt16(pos.Z)),false));
                            var t = AddEntity(new House("small_house", new Vector3(Convert.ToInt16(pos.X), 0, Convert.ToInt16(pos.Z))));
                            build_mode = false;
                            StateMaschine.Context.CursorVisible = true;

                        }

                    }
                    wasdown = true;
                }
                else
                {
                    wasdown = false;
                }
                GetEntity(wellmodel).GetComponent<Transform>().SetPosition(new Vector3(Convert.ToInt16(pos.X), 0, Convert.ToInt16(pos.Z)));
                GetEntity(wellmodel).Active = true;
            }

        }
    }
}