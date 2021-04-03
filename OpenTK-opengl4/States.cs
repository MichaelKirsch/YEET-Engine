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
using OpenTK_opengl4.Ants;
using SimplexNoise;
using Buffer = System.Buffer;


namespace OpenTK_opengl4
{
    public class RenderingTest : State
    {
        private Playfield _playfield;
        private Camera _camera;
        private bool mouse_locked,wireframe;
        private OBJLoader loader;
        public RenderingTest()
        {
            Console.WriteLine("State1 construct");
            _camera = new Camera();
        }

        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);

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

            
            
            
            ImGui.Text("Frametime Average:" + 1000.0f/StateMaschine.Context.AverageLastFrameRenderTime);
            ImGui.SliderInt("Width",ref _playfield.Width,100,1000);
            ImGui.SliderInt("Height",ref _playfield.Height,100,1000);
            ImGui.SliderFloat("MouseSens", ref _camera.MouseSensitivity, 0.0f, 1.0f);
            ImGui.Text((_camera.Yaw,_camera.Pitch).ToString());
            ImGui.SliderFloat("Noise Scale", ref _playfield.NoiseScaleMayor, 0.0f, 1.0f);
            ImGui.SliderFloat("Height Scale", ref _playfield.HeightScaler, 10.0f, 300.0f);
            if(ImGui.Button("Generate"))
                _playfield.Generate();
            
            ImGui.Checkbox("Wireframe", ref wireframe);
            
            ImGui.Text(_camera.Position.ToString());
            ImGui.Checkbox("Mouse Lock", ref mouse_locked);
            ImGui.End();
        }

        public override void OnRender()
        {
            base.OnRender();
            if(wireframe)
                GL.PolygonMode(MaterialFace.FrontAndBack,PolygonMode.Line);
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack,PolygonMode.Fill);
            }
            _playfield.Draw();
            GL.UniformMatrix4(_playfield.shaderLoader.GetUniformLocation("view"),false,ref _camera.View);
            GL.UniformMatrix4(_playfield.shaderLoader.GetUniformLocation("projection"),false,ref _camera.Projection);
            GL.Uniform1(_playfield.shaderLoader.GetUniformLocation("MinHeight"), _playfield.MINHEIGHT);
            GL.Uniform1(_playfield.shaderLoader.GetUniformLocation("MaxHeight"), _playfield.MAXHEIGHT);
            loader.Draw();
            GL.UniformMatrix4(loader.Loader.GetUniformLocation("view"),false,ref _camera.View);
            GL.UniformMatrix4(loader.Loader.GetUniformLocation("projection"),false,ref _camera.Projection);
        }

        public override void OnStart()
        {
            GL.Enable(EnableCap.CullFace);
            Console.WriteLine("State1 onstart");
            Random rnd = new Random();
            SimplexNoise.Noise.Seed = rnd.Next();
            GL.ClearColor(0.1f,0.0f,0.2f,1.0f);
            _playfield = new Playfield();
            _playfield.Generate();
            _camera.Reset();
            loader = new OBJLoader("Well");
            base.OnStart();
        }

        public override void OnLeave()
        {
            Console.WriteLine("State1 leaving");
            base.OnLeave();
        }

        public override void OnInput()
        {
            if (StateMaschine.Context.KeyboardState[Keys.Escape])
                mouse_locked = !mouse_locked;

            if (StateMaschine.Context.KeyboardState[Keys.K])
                wireframe = !wireframe;
            
            base.OnInput();
            _camera.ProcessKeyboard();
            if (!mouse_locked)
            {
                StateMaschine.Context.CursorVisible = false;
                
                _camera.processMouse();
            }
            else
            {
                StateMaschine.Context.CursorVisible = true;
            }
        }
    }
}