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
        
        float[] vertices = {
            -0.5f, -0.5f, 0.0f, //Bottom-left vertex
            0.5f, -0.5f, 0.0f, //Bottom-right vertex
            0.0f,  0.5f, 0.0f  //Top vertex
        };

        private int VAO, VBO;
        float red;

        public RenderingTest()
        {
            Console.WriteLine("State1 construct");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            StateMaschine.Context.Title = "Opengl C# Game Engine. FPS:" + StateMaschine.Context.AverageLastFrameRenderTime.ToString();

        }

        public override void OnGui()
        {
            base.OnGui();
            ImGui.Begin("Debug Window");
            ImGui.SetWindowFontScale(2.0f);
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
            ImGui.Text("Frametime:" + StateMaschine.Context.LastFrameUpdateTime);
            ImGui.Text("Frametime Average:" + StateMaschine.Context.AverageLastFrameRenderTime);
            ImGui.SliderFloat("Red", ref red, 0.0f, 1.0f);
            
            ImGui.End();
        }

        public override void OnRender()
        {
            base.OnRender();
            ProgrammTriangle.UseShader();
            GL.Uniform1(ProgrammTriangle.GetUniformLocation("red"),red);
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        public override void OnStart()
        {
            Console.WriteLine("State1 onstart");
            
            GL.ClearColor(0.1f,0.0f,0.2f,1.0f);
            ProgrammTriangle = new ShaderLoader("Triangle","TriangleVert", "TriangleFrag", true);
            base.OnStart();
            VBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer,VBO);
            GL.BufferData(BufferTarget.ArrayBuffer,vertices.Length*sizeof(float),vertices,BufferUsageHint.StaticDraw);
            
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            
        }

        public override void OnLeave()
        {
            Console.WriteLine("State1 leaving");
            base.OnLeave();
        }
    
        private ShaderLoader ProgrammTriangle;

    }
}