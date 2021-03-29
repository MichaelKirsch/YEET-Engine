using System;
using System.Collections;
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
            Console.WriteLine("State1 construct");
            counter = 0;
            
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            Console.WriteLine("State1 update");
            counter++;
            if (counter > 100)
            {
                
                StateMaschine.SwitchState(new State2());
            }
            
        }

        public override void OnRender()
        {
            Console.WriteLine("Render");
            base.OnRender();
        }

        public override void OnStart()
        {
            //StateMaschine._Context.SetClearColor(1.0f);
            System.Console.WriteLine("State1 onstart");
            base.OnStart();
        }

        public override void OnLeave()
        {
            System.Console.WriteLine("State1 leaving");
            base.OnLeave();
        }
        private int counter =0;
    }

    public class State2 : State
    {
        public State2()
        {
            System.Console.WriteLine("State2 construct");
            
            counter = 0;
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            System.Console.WriteLine("State2 update");
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
            System.Console.WriteLine("State2 onstart");
            
            base.OnStart();
        }

        public override void OnLeave()
        {
            System.Console.WriteLine("State2 leaving");
            base.OnLeave();
        }

        private int counter = 0;

    }
    
    
}