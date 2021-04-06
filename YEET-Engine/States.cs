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
using YEET.ComponentEntitySystem.Entities;
using Buffer = System.Buffer;


namespace YEET
{
    public class RenderingTest : State
    {
        private Camera _Camera;
        private bool _MouseLocked, _WireFrame;
        private OBJLoader _Palmtree;
        private Vector3 _LightPosition;
        private Grid _Grid;
        private LineDrawer _line;
        private TestEntitiy _testEntitiy;

        private Vector3 corner1=(0,0,0), corner2=(0,0,100), corner3=(100,0,100), corner4=(100,0,0);
        
        private float treeposx=0;

        public RenderingTest()
        {
            Console.WriteLine("State1 construct");
            _Camera = new Camera();
        }

        public override void OnGui()
        {
            base.OnGui();
            ImGui.Begin("Main");
            ImGui.SetWindowFontScale(1.5f);
            if (ImGui.Button("Click here for purple"))
            {
                GL.ClearColor(0.1f, 0.0f, 0.2f, 1.0f);
            }

            if (ImGui.Button("Click here for grey"))
            {
                GL.ClearColor(0.1f, 0.1f, 0.2f, 1.0f);
            }

            if (ImGui.Button("Exit"))
            {
                StateMaschine.Exit();
            }

            ImGui.Text("Avg Rendertime:" +
                       StateMaschine.Context.AverageLastFrameRenderTime.ToString("0.000") + "ms");
            ImGui.SliderFloat("MouseSens", ref _Camera.MouseSensitivity, 0.0f, 1.0f);
            ImGui.SliderFloat("Light Height:", ref _LightPosition.Y, 10, 100);
            ImGui.SliderFloat("Light X:", ref _LightPosition.X, 0, 100);
            ImGui.SliderFloat("Light Z:", ref _LightPosition.Z, 0, 100);

            //ImGui.ColorPicker3("Color", ref _Grid.rgb_plane, ImGuiColorEditFlags.Float);
            //ImGui.ColorPicker3("Color Lines", ref _Grid.rgb_grid, ImGuiColorEditFlags.Float);
            ImGui.Checkbox("Wireframe", ref _WireFrame);
            ImGui.Checkbox("Mouse Lock", ref _MouseLocked);
            ImGui.End();
        }

        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);
            treeposx += 0.2f;
            if (treeposx > 100)
                treeposx = 0;
            _Palmtree.Position.X = treeposx;
            _Palmtree.Position.Z = treeposx / 2;
        }


        public override void OnRender()
        {
            base.OnRender();
            if (_WireFrame)
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
            
            
            _Palmtree.Draw();
            _Palmtree.Loader.SetUniformVec3("LightPosition", _LightPosition);
            _Palmtree.Loader.SetUniformMatrix4F("view", ref _Camera.View);
            _Palmtree.Loader.SetUniformMatrix4F("projection", ref _Camera.Projection);
            _Grid.Draw();
            _Grid._loader.SetUniformMatrix4F("view", ref _Camera.View);
            _Grid._loader.SetUniformMatrix4F("projection", ref _Camera.Projection);
            _line.ChangeLine("lightTree",_Palmtree.Position,_LightPosition,new Vector3(1,0,1));
            _line.ChangeLine("treeorigin",_Palmtree.Position,Vector3.Zero, new Vector3(0,0,1));
            _line.ChangeLine("corner1",_Palmtree.Position,corner1,new Vector3(1,1,1));
            _line.ChangeLine("corner2",_Palmtree.Position,corner2,new Vector3(1,1,1));
            _line.ChangeLine("corner3",_Palmtree.Position,corner3,new Vector3(1,1,1));
            _line.ChangeLine("corner4",_Palmtree.Position,corner4,new Vector3(1,1,1));
            _line.ChangeLine("cameraTree",_Palmtree.Position, (_Camera.Position.X,_Camera.Position.Y-1,_Camera.Position.Z),new Vector3(0,1,0));
            _line.Draw(_Camera.View,_Camera.Projection);
        }

        public override void OnStart()
        {
            Console.WriteLine("State1 onstart");
            GL.ClearColor(0.1f, 0.0f, 0.2f, 1.0f);
            _Palmtree = new OBJLoader("BigTree",
                new ShaderLoader("Model", "FlatShadedModelVert", "FlatShadedModelFrag", true));
            _Camera.Start();
            _Camera.Position = (50, 50, 50);
            _line = new LineDrawer(new Vector3(1f, 0.0f, 0.0f));
            _line.AddLine("lightTree",_Palmtree.Position,_LightPosition,new Vector3(1,0,0));
            _line.AddLine("treeorigin",_Palmtree.Position,_LightPosition,new Vector3(1,0,0));
            _line.AddLine("cameraTree",_Palmtree.Position,(_Camera.Position.X,_Camera.Position.Y+2,_Camera.Position.Z),new Vector3(1,0,0));
            
            _line.AddLine("corner1",_Palmtree.Position,corner1,new Vector3(1,1,1));
            _line.AddLine("corner2",_Palmtree.Position,corner2,new Vector3(1,1,1));
            _line.AddLine("corner3",_Palmtree.Position,corner3,new Vector3(1,1,1));
            _line.AddLine("corner4",_Palmtree.Position,corner4,new Vector3(1,1,1));
            _testEntitiy = new TestEntitiy();
            _Camera.GrabCursor(false);
            _Grid = new Grid(new ShaderLoader("Grid", "GridVert", "GridFrag", true), (100, 100), 0.04f);
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