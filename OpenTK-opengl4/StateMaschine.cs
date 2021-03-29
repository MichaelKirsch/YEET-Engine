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
            _Context = new MainWindow(updateRate,frameRate);
            _currentState = start;
            _Context.Run();
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


        private static State _currentState;
        public static MainWindow _Context;
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

    
}