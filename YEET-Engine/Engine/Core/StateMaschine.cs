using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;


namespace YEET.Engine.Core
{
    public static class StateMaschine
    {
        private static long _startTime, _startTicks;
        private static Stopwatch _stopwatch;

        private static Scene _currentScene;
        public static MainWindow Context;
        
        private static ulong _globalID;

        static float[] rectangleVertices =
        {
            // Coords    // texCoords
            1.0f, -1.0f, 1.0f, 0.0f,
            -1.0f, -1.0f, 0.0f, 0.0f,
            -1.0f, 1.0f, 0.0f, 1.0f,

            1.0f, 1.0f, 1.0f, 1.0f,
            1.0f, -1.0f, 1.0f, 0.0f,
            -1.0f, 1.0f, 0.0f, 1.0f
        };


        static StateMaschine()
        {
            _stopwatch = Stopwatch.StartNew();
            _stopwatch.Start();
            _startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            _startTicks = DateTime.Now.Ticks;
            //if you dont load old games than counter starts at 0
            _globalID = 0;
        }

        public static ulong GetNewID()
        {
            return _globalID++;
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
                base.OnUpdate(e);
                SwitchState(_startScene);
            }

            private Scene _startScene;
        }

        public static double GetElapsedTimeTicks()
        {
            return DateTime.Now.Ticks - _startTicks;
        }

        public static double GetElapsedTime()
        {
            return (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - _startTime;
        }

        public static void Run(Scene start, int updateRate, int frameRate)
        {
            _currentScene = new Startupstate(start);
            Context = new MainWindow(updateRate, frameRate);
            Context.Run();
        }

        public static void Render()
        {
            new TimeSlot("Main Render");
            _currentScene.OnRender();
            Profiler.StopTimeSlot("Main Render");
            new TimeSlot("Gui Render");
            _currentScene.OnGui();
            Profiler.StopTimeSlot("Gui Render");
            Profiler.StopFrame();
            InstanceRenderer.ClearStacks(); //should be done after all rendering is done
            Profiler.RenderProfilerWindow();
        }

        public static void Input()
        {
            Profiler.StartFrame();
            Profiler.StartTimeSlot("Input");
            _currentScene.OnInput();
            
            Profiler.StopTimeSlot("Input");
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

        public static Scene GetCurrentScene()
        {
            return _currentScene;
        }
        
        
    }
}