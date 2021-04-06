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
        private StaticOBJModel _staticObjModelTest;
        private bool _MouseLocked, _WireFrame;
        private Vector3 _LightPosition;
        private Grid _Grid;
        private LineDrawer _line;
        private LineDrawer.Line cameraPlayer, treeCamera, TreeLight;
        private TestEntitiy _testEntitiy;

        private Vector3 corner1=(0,0,0), corner2=(0,0,100), corner3=(100,0,100), corner4=(100,0,0);
        
        private float treeposx=0;

        public RenderingTest()
        {
            Console.WriteLine("State1 construct");
        }
        
        public override void OnStart()
        {
            Console.WriteLine("State1 onstart");
            GL.ClearColor(0.1f, 0.0f, 0.2f, 1.0f);
            Camera.Start();
            _staticObjModelTest = new StaticOBJModel("Palmtree", new Vector3(10, 0, 20));

            Camera.Position = (50, 50, 50);
            _line = new LineDrawer(new Vector3(1f, 0.0f, 0.0f));

            cameraPlayer = _line.AddLine(ref Camera.Position, ref _LightPosition, Colors.Red);
            treeCamera = _line.AddLine(ref _staticObjModelTest.GetComponent<Transform>()._position, ref corner1, Colors.Green);
            TreeLight = _line.AddLine(ref _LightPosition, ref _staticObjModelTest.GetComponent<Transform>()._position, Colors.Blue);
            _testEntitiy = new TestEntitiy();
            
            
            
            Camera.GrabCursor(false);
            _Grid = new Grid(new ShaderLoader("Grid", "GridVert", "GridFrag", true), (100, 100), 0.04f);
            base.OnStart();
            
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
            ImGui.SliderFloat("MouseSens", ref Camera.MouseSensitivity, 0.0f, 1.0f);
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
            _staticObjModelTest.GetComponent<Transform>().SetX(treeposx);;
            _staticObjModelTest.GetComponent<Transform>().SetZ(50+treeposx*Math.Sin(treeposx/10));
            treeCamera.Start = corner4;
            treeCamera.End = _staticObjModelTest.GetComponent<Transform>().GetPosition();
            cameraPlayer.Start = _staticObjModelTest.GetComponent<Transform>().GetPosition();
            cameraPlayer.End = corner1;
            TreeLight.End = _LightPosition;
            TreeLight.Start = _staticObjModelTest.GetComponent<Transform>().GetPosition();
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
            _Grid.Draw();
            _line.Draw(Camera.View,Camera.Projection);
            _staticObjModelTest.OnDraw();
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
                Camera.GrabCursor(_MouseLocked);
            }

            if (StateMaschine.Context.KeyboardState[Keys.K])
                _WireFrame = !_WireFrame;


            base.OnInput();
            Camera.ProcessKeyboard();
            Camera.processMouse();
        }
    }
}