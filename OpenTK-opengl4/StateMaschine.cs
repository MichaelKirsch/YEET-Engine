using System;
using System.Runtime.InteropServices;
using System.Timers;

namespace OpenTK_opengl4
{
    public static class StateMaschine
    {
        static StateMaschine()
        {
            Console.WriteLine("StateMaschine Started");
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
        }

        public static void Update()
        {
            
            _currentState.OnUpdate();
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
        public State()
        {
            
        }

        public virtual void OnUpdate()
        {
            
        }

        public virtual void OnRender()
        {
            
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnLeave()
        {
        }
    }

    public class Startupstate : State
    {
        public Startupstate(State new_state)
        {
            _startState = new_state;
        }
        public override void OnUpdate()
        {
            Console.WriteLine("Startup created. Will switch to First State now");
            base.OnUpdate();
            StateMaschine.SwitchState(_startState);
        }

        private State _startState;
    }
    

    
}