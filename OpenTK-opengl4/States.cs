using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using OpenTK.Windowing.Common.Input;
using SimplexNoise;


namespace OpenTK_opengl4
{
    public class State1 : State
    {
        public State1()
        {
            Debug.WriteLine("State1 construct");
            counter = 0;
            
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            Debug.WriteLine("State1 update");
            counter++;
            if (counter > 100)
            {
                
                StateMaschine.SwitchState(new State2());
            }
            
        }

        public override void OnRender()
        {
            Debug.WriteLine("Render");
            base.OnRender();
        }

        public override void OnStart()
        {
            //StateMaschine._Context.SetClearColor(1.0f);
            Debug.WriteLine("State1 onstart");
            base.OnStart();
        }

        public override void OnLeave()
        {
            Debug.WriteLine("State1 leaving");
            base.OnLeave();
        }
        private int counter =0;
    }

    public class State2 : State
    {
        public State2()
        {
            Debug.WriteLine("State2 construct");
            
            counter = 0;
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            Debug.WriteLine("State2 update");
            counter++;
            if (counter > 100)
            {
                
                StateMaschine.SwitchState(new State1());
            }
            
        }

        public override void OnRender()
        {
            
            base.OnRender();
        }

        public override void OnStart()
        {
            Debug.WriteLine("State2 onstart");
            
            base.OnStart();
        }

        public override void OnLeave()
        {
            Debug.WriteLine("State2 leaving");
            base.OnLeave();
        }

        private int counter = 0;

    }
    
    
}