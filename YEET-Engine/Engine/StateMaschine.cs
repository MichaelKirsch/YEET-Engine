using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace YEET
{
    public static class StateMaschine
    {
        static StateMaschine()
        {
            Console.WriteLine("StateMaschine Started");
        }
        
        private class Startupstate : State
        {
            public Startupstate(State new_state)
            {
                _startState = new_state;
            }
            public override void OnUpdate(FrameEventArgs e)
            {
                Console.WriteLine("Startup created. Will switch to First State now");
                base.OnUpdate(e);
                StateMaschine.SwitchState(_startState);
            }

            private State _startState;
        }

        public static double GetElapsedTime()
        {
            return (DateTime.Now - Process.GetCurrentProcess().StartTime).Milliseconds;
        }
        
        
        public static void Run(State start,int updateRate, int frameRate)
        {
            _currentState = new Startupstate(start);
            Context = new MainWindow(updateRate,frameRate);
            Context.Run();
        }
        
        public static void Render()
        {
            _currentState.OnRender();
            _currentState.OnGui();
        }

        public static void Input()
        {
            _currentState.OnInput();
        }
        
        public static void Update(FrameEventArgs e)
        {
            
            _currentState.OnUpdate(e);
        }
        
        public static void SwitchState(State nextState)
        {
            _currentState.OnLeave();
            _currentState = nextState;
            _currentState.OnStart();
        }

        public static void Exit()
        {
            _currentState.OnLeave();
            Context.Close();
            Context.Dispose();
        }
        
        

        private static State _currentState;
        public static MainWindow Context;
    }

    public class State
    {

        private Dictionary<Guid, Entity> Entities;


        public Guid AddEntity(Entity toadd)
        {
            var x = Guid.NewGuid();
            Entities.Add(x,toadd);
            return x;
        }
        
        public T GetEntity<T>(Guid tofind) where T: Entity 
        {
            foreach (var item in Entities)
            {
                if (item.Key == tofind)
                    return (T) item.Value;
            }
            return null;
        }
        
        
        public State()
        {
            Entities = new Dictionary<Guid, Entity>();
        }
        /// <summary>
        /// Gets called after the render call. add all Imgui Code in this block
        /// </summary>
        public virtual void OnGui()
        {
            foreach (var entity in Entities)
            {
                if(entity.Value.ShowGUI)
                    entity.Value.OnGui();
            }
        }

        public virtual void OnInput()
        {
        }
        
        /// <summary>
        /// Add all update functions in this block. It is Time-managed through the opentk game window
        /// </summary>
        public virtual void OnUpdate( FrameEventArgs e)
        {
            foreach (var entity in Entities)
            {
                entity.Value.OnUpdate();
            }
        }
        /// <summary>
        /// all Rendercode should be here or you will run into gl.clear problems
        /// </summary>
        public virtual void OnRender()
        {
            foreach (var entity in Entities)
            {
                entity.Value.OnRender();
            }
        }
        /// <summary>
        /// gets called once when the state gets loaded
        /// </summary>
        public virtual void OnStart()
        {
            foreach (var entity in Entities)
            {
                entity.Value.OnStart();
            }
        }
        /// <summary>
        /// gets called once when the state gets switched away by the statemaschine
        /// </summary>
        public virtual void OnLeave()
        {
            foreach (var entity in Entities)
            {
                entity.Value.OnLeave();
            }
        }
    }

    
    

    
}