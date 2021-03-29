using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System;
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
            base.OnRenderFrame(e);

            _controller.Update(this, (float)e.Time);
            
            GL.ClearColor(new Color4(0, 32, 48, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            
            StateMaschine.Render();
            
            ImGui.ShowDemoWindow();
            
            _controller.Render();
            

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            StateMaschine.Update();
            base.OnUpdateFrame(args);
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
        
        
        private ImGuiController _controller;

    }
}