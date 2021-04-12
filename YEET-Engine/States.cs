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
        private LineDrawer.Line cameraPlayer, treeCamera, TreeLight, FrustrumRight, FrustrumLeft;
        private List<Entity> _entities;
        private Vector3 corner1 = (0, 0, 0), corner2 = (0, 0, 100), corner3 = (100, 0, 100), corner4 = (100, 0, 0);
        private List<PointCloudTest> terrain;
        private int noise_divider=100;
        private float pointcloud_scale = 0.3f;
        public RenderingTest()
        {
            Console.WriteLine("State1 construct");
        }

        public override void OnStart()
        {
            terrain = new List<PointCloudTest>();
            Random rand2 = new Random();
            var seed = rand2.Next();
            ShaderLoader loader = new ShaderLoader("Marching", "MarchingCubesVert", "MarchingCubesFrag", true);

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    terrain.Add(new PointCloudTest(seed, new Vector3i(i*32, 0, j*32),loader));
                    terrain.Add(new PointCloudTest(seed, new Vector3i(i*32, 32, j*32),loader));
                    terrain.Add(new PointCloudTest(seed, new Vector3i(i*32, 64, j*32),loader));
                }
            }
            
            

            Console.WriteLine("State1 onstart");
            GL.ClearColor(0.415f, 0.439f, 0.4f, 1.0f);
            Camera.Start();
            Camera.Position = (50, 50, 50);
            Camera.GrabCursor(false);
            
            _entities = new List<Entity>();
            var rand = new Random();
            for (int x = 0; x < 100; x++)
            {
                _entities.Add(new StaticOBJModel("Palmtree", new Vector3(rand.Next() % 100, 0, rand.Next() % 100)));
            }


            _staticObjModelTest = new StaticOBJModel("Palmtree", new Vector3(10, 0, 20));
            _Grid = new Grid(new ShaderLoader("Grid", "GridVert", "GridFrag", true), (100, 100), 0.04f);
            _line = new LineDrawer(new Vector3(1f, 0.0f, 0.0f));

            cameraPlayer = _line.AddLine(ref Camera.Position, ref _LightPosition, Colors.Red);
            treeCamera = _line.AddLine(ref _staticObjModelTest.GetComponent<Transform>()._position, ref corner1,
                Colors.Green);
            TreeLight = _line.AddLine(ref _LightPosition, ref _staticObjModelTest.GetComponent<Transform>()._position,
                Colors.Blue);
            FrustrumLeft = _line.AddLine(ref _LightPosition,
                ref _staticObjModelTest.GetComponent<Transform>()._position, new Vector3(1, 0.70f, 0.039f));
            FrustrumRight = _line.AddLine(ref _LightPosition,
                ref _staticObjModelTest.GetComponent<Transform>()._position, new Vector3(1, 0.70f, 0.039f));
            
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
            ImGui.SliderInt("Frustrum", ref Camera.Frustrum, 45, 90);
            ImGui.SliderInt("Noise Divider", ref noise_divider, 3, 50);
            ImGui.SliderFloat("Scale", ref pointcloud_scale, 0.01f, 0.1f);
            ImGui.SliderFloat("Render Distance", ref Camera.RenderingDistance, 20, 150);
            
            //ImGui.ColorPicker3("Color", ref _Grid.rgb_plane, ImGuiColorEditFlags.Float);
            //ImGui.ColorPicker3("Color Lines", ref _Grid.rgb_grid, ImGuiColorEditFlags.Float);
            ImGui.Checkbox("Wireframe", ref _WireFrame);
            ImGui.Checkbox("Mouse Lock", ref _MouseLocked);
            ImGui.End();
        }

        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);
            treeCamera.Start = corner4;
            treeCamera.End = _staticObjModelTest.GetComponent<Transform>().GetPosition();

            TreeLight.End = _LightPosition;
            TreeLight.Start = _staticObjModelTest.GetComponent<Transform>().GetPosition();


            var st = new Vector3(Camera.Position.X, 0.5f, Camera.Position.Z);

            cameraPlayer.Start = st;
            cameraPlayer.End = st + new Vector3(Camera.Front.X, 0.0f, Camera.Front.Z).Normalized() * 30f;

            FrustrumLeft.Start = st;
            FrustrumRight.Start = st;
            Matrix3 rot = new Matrix3();
            Matrix3.CreateRotationY(MathHelper.DegreesToRadians(Camera.Frustrum/2f), out rot);
            FrustrumLeft.End =st+ new Vector3(new Vector3(Camera.Front.X, 0.0f, Camera.Front.Z) * rot).Normalized()*30f;
            Matrix3.CreateRotationY(MathHelper.DegreesToRadians(-Camera.Frustrum/2f), out rot);
            FrustrumRight.End = st +  new Vector3(new Vector3(Camera.Front.X, 0.0f, Camera.Front.Z) * rot).Normalized()*30f;
            foreach (var pointCloudTest in terrain)
            {
                pointCloudTest.Divider = noise_divider;
                pointCloudTest.Scale = pointcloud_scale;
                pointCloudTest.OnUpdate();
            }
            _staticObjModelTest.OnUpdate();
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
            _line.Draw();
            _staticObjModelTest.OnDraw();
            foreach (var entity in _entities)
            {
                entity.OnDraw();
            }

            foreach (var dPointCloudTest in terrain)
            {
                dPointCloudTest.OnDraw(_LightPosition);
            }
            
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