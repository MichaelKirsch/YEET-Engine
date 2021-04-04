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
        private Vector3 LightPosition;
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

            
            
            
            ImGui.Text("Avg Rendertime:" + 
                       StateMaschine.Context.AverageLastFrameRenderTime.ToString("0.000")+"ms");
            ImGui.SliderInt("Width",ref _playfield.Width,100,1000);
            ImGui.SliderInt("Height",ref _playfield.Height,100,1000);
            ImGui.SliderFloat("MouseSens", ref _camera.MouseSensitivity, 0.0f, 1.0f);
            ImGui.SliderFloat("Noise Scale", ref _playfield.NoiseScaleMayor, 0.0f, 1.0f);
            ImGui.SliderFloat("Height Scale", ref _playfield.HeightScaler, 10.0f, 300.0f);
            ImGui.SliderFloat("Light Height:", ref LightPosition.Y,10,100);
            ImGui.SliderFloat("Light X:", ref LightPosition.X,0,100);
            ImGui.SliderFloat("Light Z:", ref LightPosition.Z,0,100);
            if(ImGui.Button("Generate"))
                _playfield.Generate();
            
            ImGui.Checkbox("Wireframe", ref wireframe);
            
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
            _playfield.shaderLoader.SetUniformMatrix4F("view",ref _camera.View);
            _playfield.shaderLoader.SetUniformMatrix4F("projection",ref _camera.Projection);
            _playfield.shaderLoader.SetUniformFloat("MinHeight", _playfield.MINHEIGHT);
            _playfield.shaderLoader.SetUniformFloat("MaxHeight", _playfield.MAXHEIGHT);
            loader.Draw();
            loader.SetPosition(LightPosition);
            loader.Loader.SetUniformVec3("LightPosition",LightPosition);
            loader.Loader.SetUniformMatrix4F("view",ref _camera.View);
            loader.Loader.SetUniformMatrix4F("projection",ref _camera.Projection);
            
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
            loader = new OBJLoader("Palmtree",new ShaderLoader("Model", "FlatShadedModelVert", "FlatShadedModelFrag", true));
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