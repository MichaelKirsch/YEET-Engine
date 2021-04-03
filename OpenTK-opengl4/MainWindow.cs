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


namespace OpenTK_opengl4
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
        
        protected override void OnLoad()
        {
            base.OnLoad();

            Title += ": OpenGL Version: " + GL.GetString(StringName.Version);
            
            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
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
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            base.OnRenderFrame(e);
            _controller.Update(this, (float)e.Time);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            StateMaschine.Render();
            _controller.Render();
            SwapBuffers();
            LastFrameRenderTime = watch.Result();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var watch = new Util.StopWatchMilliseconds();
            base.OnUpdateFrame(e);
            StateMaschine.Input();
            StateMaschine.Update(e);
            LastFrameUpdateTime = watch.Result();
            GenerateAverageFrameRenderTime();
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
        
        public double LastFrameRenderTime;
        public double AverageLastFrameRenderTime;
        public double LastFrameUpdateTime;
        public ImGuiController _controller;
        private Queue<double> ListLastFrameTimes = new Queue<double>();
    }
    
    
    
}