using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading;
using ImGuiNET;


namespace YEET.Engine.Core
{
    public class MainWindow : GameWindow
    {
        public MainWindow(int updateRate,int frameRate) : base(GameWindowSettings.Default, new NativeWindowSettings(){Size = new Vector2i(1920,1080), APIVersion = new Version(4, 5)})
        {
            UpdateFrequency = updateRate;
            RenderFrequency = frameRate;
            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Back);
        }
        
        public void WireMode(bool status)
        {
            if (status)
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
        }


        private Query testQuery;
        protected override void OnLoad()
        {
            base.OnLoad();
            
            Title = "YEET-Engine";
            Title += ": OpenGL Version: " + GL.GetString(StringName.Version);
            
            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
            var x = ImGui.GetIO();
            x.FontGlobalScale = 0.5f;
            PushCustomStyle();

            testQuery = new Query(1000); // update every second (low values can result in performance decrease < 30)
        }


        public void PushCustomStyle()
        {
            var x =ImGui.GetStyle();
            x.WindowPadding = new System.Numerics.Vector2(15, 15);
            x.WindowRounding = 0f;
            x.FramePadding = new System.Numerics.Vector2(5, 5);
            x.FrameRounding = 4f;
            x.ItemSpacing = new System.Numerics.Vector2(12, 8);
            x.ItemInnerSpacing = new System.Numerics.Vector2(8, 6);
            x.IndentSpacing = 25f;
            x.ScrollbarSize = 15f;
            x.ScrollbarRounding = 9f;
            x.GrabMinSize = 5f;
            x.GrabRounding = 3f;
            ImGui.PushStyleColor(ImGuiCol.Text,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.TextDisabled,new System.Numerics.Vector4(0.24f, 0.23f, 0.29f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.WindowBg,new System.Numerics.Vector4(0.388f, 0.388f, 0.388f,1.0f));
            ImGui.PushStyleColor(ImGuiCol.ChildBg,new System.Numerics.Vector4(0.398f, 0.398f, 0.398f,1.0f));
            ImGui.PushStyleColor(ImGuiCol.PopupBg,new System.Numerics.Vector4(0.07f, 0.07f, 0.09f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.Border,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.88f));
            ImGui.PushStyleColor(ImGuiCol.BorderShadow,new System.Numerics.Vector4(0.92f, 0.91f, 0.88f, 0.00f));
            ImGui.PushStyleColor(ImGuiCol.FrameBg,new System.Numerics.Vector4(0.10f, 0.09f, 0.12f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.FrameBgHovered,new System.Numerics.Vector4(0.24f, 0.23f, 0.29f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.FrameBgActive,new System.Numerics.Vector4(0.56f, 0.56f, 0.58f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.TitleBg,new System.Numerics.Vector4(0.10f, 0.09f, 0.12f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.TitleBgCollapsed,new System.Numerics.Vector4(1.00f, 0.98f, 0.95f, 0.75f));
            ImGui.PushStyleColor(ImGuiCol.TitleBgActive,new System.Numerics.Vector4(0.07f, 0.07f, 0.09f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.MenuBarBg,new System.Numerics.Vector4(0.10f, 0.09f, 0.12f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.ScrollbarBg,new System.Numerics.Vector4(0.10f, 0.09f, 0.12f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.ScrollbarGrab,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.31f));
            ImGui.PushStyleColor(ImGuiCol.ScrollbarGrabHovered,new System.Numerics.Vector4(0.56f, 0.56f, 0.58f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.ScrollbarGrabActive,new System.Numerics.Vector4(0.06f, 0.05f, 0.07f, 1.00f));
            //ImGui.PushStyleColor(ImGuiCol.Combo,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.88f));
            ImGui.PushStyleColor(ImGuiCol.CheckMark,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.31f));
            ImGui.PushStyleColor(ImGuiCol.SliderGrab,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.31f));
            ImGui.PushStyleColor(ImGuiCol.SliderGrabActive,new System.Numerics.Vector4(0.06f, 0.05f, 0.07f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.Button,new System.Numerics.Vector4(0.10f, 0.09f, 0.12f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered,new System.Numerics.Vector4(0.24f, 0.23f, 0.29f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive,new System.Numerics.Vector4(0.56f, 0.56f, 0.58f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.Header,new System.Numerics.Vector4(0.10f, 0.09f, 0.12f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.HeaderHovered,new System.Numerics.Vector4(0.56f, 0.56f, 0.58f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.HeaderActive,new System.Numerics.Vector4(0.06f, 0.05f, 0.07f, 1.00f));
            //ImGui.PushStyleColor(ImGuiCol,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.88f));
            //ImGui.PushStyleColor(ImGuiCol.Border,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.88f));
            //ImGui.PushStyleColor(ImGuiCol.Border,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.88f));
            ImGui.PushStyleColor(ImGuiCol.ResizeGrip,new System.Numerics.Vector4(0.00f, 0.00f, 0.00f, 0.00f));
            ImGui.PushStyleColor(ImGuiCol.ResizeGripHovered,new System.Numerics.Vector4(0.56f, 0.56f, 0.58f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.ResizeGripActive,new System.Numerics.Vector4(0.06f, 0.05f, 0.07f, 1.00f));
            //ImGui.PushStyleColor(ImGuiCol.butt,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.88f));
            //ImGui.PushStyleColor(ImGuiCol.Border,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.88f));
            //ImGui.PushStyleColor(ImGuiCol.Border,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.88f));
            ImGui.PushStyleColor(ImGuiCol.PlotLines,new System.Numerics.Vector4(0.40f, 0.39f, 0.38f, 0.63f));
            ImGui.PushStyleColor(ImGuiCol.PlotLinesHovered,new System.Numerics.Vector4(0.25f, 1.00f, 0.00f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.PlotHistogram,new System.Numerics.Vector4(0.40f, 0.39f, 0.38f, 0.63f));
            ImGui.PushStyleColor(ImGuiCol.PlotHistogramHovered,new System.Numerics.Vector4(0.25f, 1.00f, 0.00f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.TextSelectedBg,new System.Numerics.Vector4(0.25f, 1.00f, 0.00f, 0.43f));
            ImGui.PushStyleColor(ImGuiCol.ModalWindowDimBg,new System.Numerics.Vector4(1.00f, 0.98f, 0.95f, 0.73f));
        }
        
        
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            // Update the opengl viewport
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

            // Tell ImGui of the new size
            _controller.WindowResized(ClientSize.X, ClientSize.Y);
        }

        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            var watch = new Util.StopWatchMilliseconds();
            testQuery.Start();
            {
                GL.Enable(EnableCap.DepthTest);
                //GL.Enable(EnableCap.CullFace);
                GL.DepthMask(true);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

                _controller.Update(this, (float)e.Time);

                StateMaschine.Render();
                _controller.Render();
            }
            testQuery.StopAndReset(); // everthing that runs on the gpu from testQuery.Start() to here will precisely be messured in ms
            
            //LastFrameRenderTime = Convert.ToSingle(watch.Result());
            LastFrameRenderTime = testQuery.ElapsedMilliseconds;
            
            SwapBuffers();
            
            GenerateAverageFrameRenderTime();
           // Console.WriteLine($"System.Diagnostics.Stopwatch: {watch.Result()}ms");
           // Console.WriteLine($"Actual GPU render time: {testQuery.ElapsedMilliseconds}ms");

            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var watch = new Util.StopWatchMilliseconds(); // i would suggest creating only one timer and simply reseting that instead of creating a new instance every update
            base.OnUpdateFrame(e);
            StateMaschine.Input();
            StateMaschine.Update(e);
            
            LastFrameUpdateTime = Convert.ToSingle(watch.Result());
        }


        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnMouseEnter()
        {
            base.OnMouseEnter();
            
        }


        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            _controller.PressChar((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            
            _controller.MouseScroll(e.Offset);
        }

        private void GenerateAverageFrameRenderTime()
        {
            ListLastFrameTimes.Enqueue(LastFrameRenderTime);
            if (ListLastFrameTimes.Count > 50)
                ListLastFrameTimes.Dequeue();
            AverageLastFrameRenderTime = ListLastFrameTimes.Average();
        }
        
        public float LastFrameRenderTime;
        public float AverageLastFrameRenderTime;
        public float LastFrameUpdateTime;
        public ImGuiController _controller; 
        public Queue<float> ListLastFrameTimes = new Queue<float>();
    }
    
    
    
}