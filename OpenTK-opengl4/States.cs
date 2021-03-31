using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK;
using OpenTK.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using SimplexNoise;
using Buffer = System.Buffer;
using SFML;
using SFML.Window;

namespace OpenTK_opengl4
{
    public class RenderingTest : State
    {
        
        private ShaderLoader ProgrammTriangle;
        
        float[] triangle1  = {
            -0.005f, -0.005f, 0.0f, //Bottom-left vertex
            0.005f, -0.005f, 0.0f, //Bottom-right vertex
            -0.005f,  0.005f, 0.0f,  //Top vertex
        };
        
        float[] triangle2  = {
            0.005f, -0.005f, 0.0f, //Bottom-right vertex
            0.005f,  0.005f, 0.0f,  //Top Right
            -0.005f, 0.005f, 0.0f, //Bottom-left vertex
        };
        
        
        
        private int offsetx=1000, offsety=1000;

        private float perlin_noise_scale;
        private int VAO, VBO;
        float red,green,blue;
        private bool scroll;
        
        public RenderingTest()
        {
            Console.WriteLine("State1 construct");
            
        }

        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);
            if (scroll)
            {
                offsetx++;
                offsety++;  
            }
            List<Vector3> buffer =new List<Vector3>();
            
            for (int x = -100; x < 100; x++)
            {
                for (int y = -100; y < 100; y++)
                {
                    var xPos = x / 100f;
                    var yPos = y / 100f;
                    float rnd_col = SimplexNoise.Noise.CalcPixel2D(x+offsetx, y+offsety, perlin_noise_scale) % 255.0f / 255.0f;
                    var Color = new Vector3(rnd_col,rnd_col,rnd_col);
                    buffer.Add((triangle1[0] + xPos, triangle1[1] + yPos, triangle1[2]));
                    buffer.Add(Color);
                    buffer.Add((triangle1[3] + xPos, triangle1[4] + yPos, triangle1[5]));
                    buffer.Add(Color);
                    buffer.Add((triangle1[6] + xPos, triangle1[7] + yPos, triangle1[8]));
                    buffer.Add(Color);
                    buffer.Add((triangle2[0] + xPos, triangle2[1] + yPos, triangle2[2]));
                    buffer.Add(Color);
                    buffer.Add((triangle2[3] + xPos, triangle2[4] + yPos, triangle2[5]));
                    buffer.Add(Color);
                    buffer.Add((triangle2[6] + xPos, triangle2[7] + yPos, triangle2[8]));
                    buffer.Add(Color);
                }
            }
            
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer,VBO);
            GL.BufferData(BufferTarget.ArrayBuffer,buffer.Count*3*sizeof(float),buffer.ToArray(),BufferUsageHint.StreamDraw);
        }
        
        public override void OnGui()
        {
            base.OnGui();

            ImGui.Begin("Main");
            ImGui.SetWindowFontScale(2.0f);
            if (ImGui.Button("Click here for purple"))
            {
                GL.ClearColor(0.1f,0.0f,0.2f,1.0f);
            }
            
            if (ImGui.Button("Click here for grey"))
            {
                GL.ClearColor(0.1f,0.1f,0.2f,1.0f);
            }
                
            if (ImGui.Button("Exit"))
            {
                StateMaschine.Exit();
            }
            ImGui.Text("Mouse Pos:" +Mouse.GetPosition());
            ImGui.Text("Frametime Average:" + StateMaschine.Context.AverageLastFrameRenderTime);
            ImGui.SliderFloat("Sealevel", ref red, 0.0f, 1.0f);
            ImGui.SliderFloat("Landlevel", ref green, 0.0f, 1.0f);
            ImGui.SliderFloat("Mountainlevel", ref blue, 0.0f, 1.0f);
            ImGui.SliderFloat("Scale Noise", ref perlin_noise_scale, 0.0f, 0.1f);
            ImGui.Checkbox("Scroll", ref scroll);
            
            ImGui.End();
        }

        public override void OnRender()
        {
            base.OnRender();
            
            GL.BindVertexArray(VAO);
            ProgrammTriangle.UseShader();
            GL.Uniform3(ProgrammTriangle.GetUniformLocation("rgb"),red,green,blue);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 200*200*3*2);
        }

        public override void OnStart()
        {
            Console.WriteLine("State1 onstart");
            Random rnd = new Random();
            SimplexNoise.Noise.Seed = rnd.Next();
            GL.ClearColor(0.1f,0.0f,0.2f,1.0f);
            ProgrammTriangle = new ShaderLoader("Triangle","TriangleVert", "TriangleFrag", true);
            base.OnStart();
            
            
            
            VBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer,VBO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3*sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        public override void OnLeave()
        {
            Console.WriteLine("State1 leaving");
            base.OnLeave();
        }

        public override void OnInput()
        {
            base.OnInput();
            
            
        }
    }
}