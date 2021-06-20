using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using ImGuiNET;
using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace YEET
{
    public static class StateMaschine
    {
        private static long _startTime,_startTicks;
        private static Stopwatch _stopwatch;
        static StateMaschine()
        {
            Console.WriteLine("StateMaschine Started");
            _stopwatch = Stopwatch.StartNew();
            _stopwatch.Start();
            _startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            _startTicks = DateTime.Now.Ticks;
        }
        /// <summary>
        /// 
        /// </summary>
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

        public static double GetElapsedTimeTicks()
        {
            return  DateTime.Now.Ticks -_startTicks;
        }

        public static double GetElapsedTime()           
        {
            return  (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond)-_startTime;
        }
        public static void Run(Scene start, int updateRate, int frameRate)
        {
            _currentScene = new Startupstate(start);
            Context = new MainWindow(updateRate, frameRate);
            Context.Run();
        }

        public static void Render()
        {
            var ts = new TimeSlot("Main Render");
            _currentScene.OnRender();
            ts.Stop();
            var guits = new TimeSlot("Gui Render");
            _currentScene.OnGui();
            guits.Stop();
            Profiler.StopFrame();
            Profiler.RenderProfilerWindow();
        }

        public static void Input()
        {
            Profiler.StartFrame();
            var ts = new TimeSlot("Input");
            _currentScene.OnInput();
            ts.Stop();
        }

        public static void Update(FrameEventArgs e)
        {
             var ts = new TimeSlot("Update");
            _currentScene.OnUpdate(e);
            ts.Stop();
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
        private List<Entity> Entities = new List<Entity>();
        public Vector4 ClearColor = new Vector4(0.415f, 0.439f, 0.4f, 1.0f);

        public Scene()
        {
            
        }

        public Guid AddEntity(Entity toadd)
        {
            Entities.Add(toadd);
            return toadd.ID;
        }

        public T GetEntity<T>(Guid tofind) where T : Entity
        {
            foreach (var item in Entities)
            {
                if (item.ID == tofind)
                    return (T) item;
            }

            return null;
        }

        public Entity GetEntity(Guid tofind)
        {
            return (Entities.Find(item => item.ID == tofind));
        }
        
        /// <summary>
        /// Gets called after the render call. add all Imgui Code in this block
        /// </summary>
        public virtual void OnGui()
        {
            ImGui.Begin("Entities");
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
        public virtual void OnUpdate(FrameEventArgs e)
        {
            //generate ActiveChunks;
            
            SpatialManager.OnUpdate();
            GL.ClearColor(ClearColor.X, ClearColor.Y, ClearColor.Z, ClearColor.W);
            foreach (var entity in Entities)
            {
                if (entity.Active)
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
                if (entity.Active)
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