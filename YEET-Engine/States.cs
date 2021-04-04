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
using YEET.Ants;
using Buffer = System.Buffer;


namespace YEET
{
    public class RenderingTest : State
    {
        private Terrain _Terrain;
        private Camera _Camera;
        private bool _MouseLocked,_WireFrame;
        private OBJLoader _Palmtree;
        private Vector3 _LightPosition;
        private Grid _Grid;

        public RenderingTest()
        {
            Console.WriteLine("State1 construct");
            _Camera = new Camera();
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
            ImGui.SliderInt("Width",ref _Terrain.Width,100,1000);
            ImGui.SliderInt("Height",ref _Terrain.Height,100,1000);
            ImGui.SliderFloat("MouseSens", ref _Camera.MouseSensitivity, 0.0f, 1.0f);
            ImGui.SliderFloat("Noise Scale", ref _Terrain.NoiseScaleMayor, 0.0f, 1.0f);
            ImGui.SliderFloat("Height Scale", ref _Terrain.HeightScaler, 10.0f, 300.0f);
            ImGui.SliderFloat("Light Height:", ref _LightPosition.Y,10,100);
            ImGui.SliderFloat("Light X:", ref _LightPosition.X,0,100);
            ImGui.SliderFloat("Light Z:", ref _LightPosition.Z,0,100);

            ImGui.ColorPicker3("Color", ref _Grid.rgb_plane, ImGuiColorEditFlags.Float);
            ImGui.ColorPicker3("Color Lines", ref _Grid.rgb_grid, ImGuiColorEditFlags.Float);
            if(ImGui.Button("Generate"))
                _Terrain.Generate();
            ImGui.Checkbox("Wireframe", ref _WireFrame);
            ImGui.Checkbox("Mouse Lock", ref _MouseLocked);
            ImGui.End();
        }

        public override void OnRender()
        {
            base.OnRender();
            if(_WireFrame)
                GL.PolygonMode(MaterialFace.FrontAndBack,PolygonMode.Line);
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack,PolygonMode.Fill);
            }
            //_Terrain.Draw();
            _Terrain.shaderLoader.SetUniformMatrix4F("view",ref _Camera.View);
            _Terrain.shaderLoader.SetUniformMatrix4F("projection",ref _Camera.Projection);
            _Terrain.shaderLoader.SetUniformFloat("MinHeight", _Terrain.MINHEIGHT);
            _Terrain.shaderLoader.SetUniformFloat("MaxHeight", _Terrain.MAXHEIGHT);
            _Palmtree.Draw();
            _Palmtree.SetPosition(_LightPosition);
            _Palmtree.Loader.SetUniformVec3("LightPosition",_LightPosition);
            _Palmtree.Loader.SetUniformMatrix4F("view",ref _Camera.View);
            _Palmtree.Loader.SetUniformMatrix4F("projection",ref _Camera.Projection);
            _Grid.Draw();
            _Grid._loader.SetUniformMatrix4F("view",ref _Camera.View);
            _Grid._loader.SetUniformMatrix4F("projection",ref _Camera.Projection);
        }

        public override void OnStart()
        {
            Console.WriteLine("State1 onstart");
            GL.ClearColor(0.1f,0.0f,0.2f,1.0f);
            _Terrain = new Terrain();
            _Palmtree = new OBJLoader("BigTree",new ShaderLoader("Model", "FlatShadedModelVert", "FlatShadedModelFrag", true));
            _Terrain.Generate();
            _Camera.Start();
            _Camera.GrabCursor(false);
            _Grid = new Grid(new ShaderLoader("Grid","GridVert","GridFrag",true), (100, 100), 0.02f);
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
            {
                _MouseLocked = !_MouseLocked;
                _Camera.GrabCursor(_MouseLocked);
            }
            if (StateMaschine.Context.KeyboardState[Keys.K])
                _WireFrame = !_WireFrame;
            
            
                
            
            base.OnInput();
            _Camera.ProcessKeyboard();
            _Camera.processMouse();
        }
    }
}