using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

            testQuery = new Query(1000); // update every second (low values can result in performance decrease < 30)
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
            Console.WriteLine($"System.Diagnostics.Stopwatch: {watch.Result()}ms");
            Console.WriteLine($"Actual GPU render time: {testQuery.ElapsedMilliseconds}ms");

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