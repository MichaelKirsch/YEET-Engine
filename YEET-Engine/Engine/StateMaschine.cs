using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Timers;
using ImGuiNET;
using OpenTK.Graphics.ES11;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace YEET
{
    public static class StateMaschine
    {
        private static Stopwatch _stopwatch;
        static StateMaschine()
        {
            Console.WriteLine("StateMaschine Started");
            _stopwatch = Stopwatch.StartNew();
            _stopwatch.Start();
        }
        
        private class Startupstate : Scene
        {
            public Startupstate(Scene newScene)
            {
                _startScene = newScene;
            }
            public override void OnUpdate(FrameEventArgs e)
            {
                Console.WriteLine("Startup created. Will switch to First State now");
                base.OnUpdate(e);
                StateMaschine.SwitchState(_startScene);
            }

            private Scene _startScene;
        }

        public static double GetElapsedTime()
        {
            return _stopwatch.ElapsedMilliseconds;
        }
        
        
        public static void Run(Scene start,int updateRate, int frameRate)
        {
            _currentScene = new Startupstate(start);
            Context = new MainWindow(updateRate,frameRate);
            Context.Run();
        }
        
        public static void Render()
        {
            _currentScene.OnRender();
            _currentScene.OnGui();
        }

        public static void Input()
        {
            _currentScene.OnInput();
        }
        
        public static void Update(FrameEventArgs e)
        {
            
            _currentScene.OnUpdate(e);
        }
        
        public static void SwitchState(Scene nextScene)
        {
            _currentScene.OnLeave();
            _currentScene = nextScene;
            _currentScene.OnStart();
        }

        public static void Exit()
        {
            _currentScene.OnLeave();
            Context.Close();
            Context.Dispose();
        }
        
        

        private static Scene _currentScene;
        public static MainWindow Context;
    }

    public class Scene
    {

        private List<Entity> Entities;
        public Vector4 ClearColor= new Vector4(0.415f, 0.439f, 0.4f, 1.0f);

        public Guid AddEntity(Entity toadd)
        {
            Entities.Add(toadd);
            return toadd.ID;
        }
        
        public T GetEntity<T>(Guid tofind) where T: Entity 
        {
            foreach (var item in Entities)
            {
                if (item.ID == tofind)
                    return (T) item;
            }
            return null;
        }
        
        
        public Scene()
        {
            Entities = new List<Entity>();
        }
        /// <summary>
        /// Gets called after the render call. add all Imgui Code in this block
        /// </summary>
        public virtual void OnGui()
        {
            ImGui.Begin("Entities");
            ImGui.SetWindowFontScale(1.5f);
            foreach (var entity in Entities)
            {
                entity.OnGui();
            }
            ImGui.End();
            Camera.OnGui();
        }

        public virtual void OnInput()
        {
        }
        
        /// <summary>
        /// Add all update functions in this block. It is Time-managed through the opentk game window
        /// </summary>
        public virtual void OnUpdate( FrameEventArgs e)
        {
            GL.ClearColor(ClearColor.X,ClearColor.Y,ClearColor.Z,ClearColor.W);
            foreach (var entity in Entities)
            {
                if(entity.Active)
                    entity.OnUpdate();
            }
        }
        /// <summary>
        /// all Rendercode should be here or you will run into gl.clear problems
        /// </summary>
        public virtual void OnRender()
        {
            foreach (var entity in Entities)
            {
                if(entity.Active)
                    entity.OnRender();
            }
        }
        /// <summary>
        /// gets called once when the state gets loaded
        /// </summary>
        public virtual void OnStart()
        {
            foreach (var entity in Entities)
            {
                entity.OnStart();
            }
        }
        /// <summary>
        /// gets called once when the state gets switched away by the statemaschine
        /// </summary>
        public virtual void OnLeave()
        {
            foreach (var entity in Entities)
            {
                entity.OnLeave();
            }
        }
    }

    
    

    
}