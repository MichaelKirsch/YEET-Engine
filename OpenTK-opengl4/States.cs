using System;
using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using OpenTK.Windowing.Common.Input;
using SimplexNoise;


namespace OpenTK_opengl4
{
    public class RenderingTest : State
    {
        public RenderingTest()
        {
            Console.WriteLine("State1 construct");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
        }

        public override void OnRender()
        {
            //Console.WriteLine("Render");
            ImGui.Begin("Debug Window");
            ImGui.SetWindowFontScale(3.0f);
            if (ImGui.Button("Click here for purple"))
            {
                Console.WriteLine("Was Clicked");
                GL.ClearColor(0.1f,0.0f,0.2f,1.0f);
            }
            
            if (ImGui.Button("Click here for grey"))
            {
                Console.WriteLine("Was Clicked");
                GL.ClearColor(0.1f,0.1f,0.2f,1.0f);
            }
                
            if (ImGui.Button("Click here for State 2"))
            {
                Console.WriteLine("Was Clicked");
                StateMaschine.SwitchState(new RenderingTest2());
            }
            
            
            
            ImGui.Text("Frametime:" + StateMaschine.Context.LastFrameRenderTime);
            ImGui.Text("Frametime Average:" + StateMaschine.Context.AverageLastFrameRenderTime);
            ImGui.End();
            base.OnRender();
        }

        public override void OnStart()
        {
            Console.WriteLine("State1 onstart");
            GL.ClearColor(0.1f,0.0f,0.2f,1.0f);
            base.OnStart();
        }

        public override void OnLeave()
        {
            Console.WriteLine("State1 leaving");
            base.OnLeave();
        }
    }
    
    public class RenderingTest2 : State
    {
        public RenderingTest2()
        {
            Console.WriteLine("State1 construct");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
        }

        public override void OnRender()
        {
            //Console.WriteLine("Render");
            ImGui.Begin("Debug Window");
            ImGui.SetWindowFontScale(3.0f);
            if (ImGui.Button("Click here for purple"))
            {
                Console.WriteLine("Was Clicked");
                GL.ClearColor(0.1f,0.0f,0.2f,1.0f);
            }
            
            if (ImGui.Button("Click here for grey"))
            {
                Console.WriteLine("Was Clicked");
                GL.ClearColor(0.1f,0.1f,0.2f,1.0f);
            }
            
            if (ImGui.Button("Exit"))
            {
                StateMaschine.Exit();
            }
            
            if (ImGui.Button("Click here for State 1"))
            {
                Console.WriteLine("Was Clicked");
                StateMaschine.SwitchState(new RenderingTest());
            }
            
            ImGui.Text("Frametime:" + StateMaschine.Context.LastFrameRenderTime);
            ImGui.Text("Frametime Average:" + StateMaschine.Context.AverageLastFrameRenderTime);
            ImGui.End();
            base.OnRender();
        }

        public override void OnStart()
        {
            Console.WriteLine("State1 onstart");
            GL.ClearColor(0.0f,0.2f,0.2f,1.0f);
            base.OnStart();
        }

        public override void OnLeave()
        {
            Console.WriteLine("State1 leaving");
            base.OnLeave();
        }
    }
    
    
    
    
}