using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System;
using System.Diagnostics;
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
            base.OnRenderFrame(e);
            
            GL.ClearColor(new Color4(0, 32, 48, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            
            StateMaschine.Render();
            
            ImGui.ShowDemoWindow();
            
            _controller.Render();
            
            SwapBuffers();
            LastFrameRenderTime = watch.Result();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var watch = new Util.StopWatchMilliseconds();
            base.OnUpdateFrame(e);
            _controller.Update(this, (float)e.Time);
            StateMaschine.Update();
            
            LastFrameUpdateTime = watch.Result();
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

        public double LastFrameRenderTime;
        public double LastFrameUpdateTime;
        public ImGuiController _controller;

    }
}