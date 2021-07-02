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

        SimpleTexturedButton house1 = new SimpleTexturedButton();
        SimpleTexturedButton house2 = new SimpleTexturedButton();
        SimpleTexturedButton street1 = new SimpleTexturedButton();
        Vector3 first_click, last_click;
        SimpleTexturedButton pillar = new SimpleTexturedButton();

        private MousePicker picker = new MousePicker();
        public RenderingTest()
        {
            Console.WriteLine("State1 construct");
        }

        public override void OnStart()
        {
            wellmodel = AddEntity(new StaticOBJModel("house_type01", new Vector3(0, 0, 0), false));

            var t = AddEntity(new House());
            GetEntity(t).GetComponent<Transform>().Position = new Vector3(10, 0, 10);

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
            ImGui.Checkbox("Sun Gui", ref Sun.ShowGUI);
            ImGui.Checkbox("Gui Demo Window", ref ShowImGUIDemoWindow);
            ImGui.Checkbox("Show Profiler Window", ref Profiler.RenderProfiler);
            ImGui.Text($"Current Chunk ID {SpatialManager.GetTupelChunkIDAndInChunkPos(SpatialManager.GetIDfromWorldPos(Camera.Position))}");
            ImGui.Text($"Chunks {SpatialManager._Chunks.Count}");
            ImGui.Text($"Active Entities {EntitiesCount()}");
            ImGui.Text($"Visible Chunks {SpatialManager.VisibleChunksAccordFrustum.Count}");
            ImGui.Text($"Mouse Delta:{picker.getIntersectionGround()}");
            if (house1.Draw("Models/house_type01.png", new Vector2(100, 100)))
            {
                build_mode = true;
                draw_mode = false;
                wellmodel = ChangeEntity(wellmodel, new House("small_house", new Vector3(Convert.ToInt16(picker.getIntersectionGround().X), 0, Convert.ToInt16(picker.getIntersectionGround().Z))));
                current_build = "house_type01";
            }
            if (pillar.Draw("Models/bridge_pillar.png", new Vector2(100, 100)))
            {
                build_mode = false;
                draw_mode = true;
            }

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


            Vector3 pos = new Vector3();
            Vector3 pos_raw = new Vector3();
            for (int x = 0; x < 100; x++)
            {
                var ray = picker.getIntersectionGround();
                pos = ray.Normalized() * x + Camera.Position;
                pos_raw = pos;
                if (pos.Y < 0)
                {
                    pos.Y = 0;
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
                            item.ShowGUI = true;
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
                    first_click = pos;
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
                    last_click = pos;
                    for (int x = 0; x < (last_pos - first_click).Length; x += 2)
                    {
                        var dir = (last_pos - first_click).Normalized();
                        //AddEntity(new House("small_house",first_click+dir*x));   
                    }

                    wasdown = false;
                    draw_mode = false;
                    Vector3 distance = last_click - first_click;
                    RemoveEntity(tmp);
                    AddEntity(new Area(first_click, last_click, ColorHelper.ConvertColor(Color.Aqua)));
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