using System.Runtime.CompilerServices;
using System.Threading;
using OpenTK.Input;
using OpenTK.Windowing.Common.Input;


namespace OpenTK_opengl4
{
    public class State1 : State
    {
        public State1(StateMaschine parent):base(parent)
        {
            System.Console.WriteLine("State1 construct");
        }

        public override void OnUpdate()
        {
            System.Console.WriteLine("State1 update");
            counter++;
            if (counter > 10)
            {
                _Parent.SwitchState(new State2(_Parent));
            }
            Thread.Sleep(50);
            base.OnUpdate();
        }

        public override void OnStart()
        {
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
        public State2(StateMaschine parent):base(parent)
        {
            System.Console.WriteLine("State2 construct");
        }
        public override void OnUpdate()
        {
            System.Console.WriteLine("State2 update");
            counter++;

            
            
            if (counter > 10)
            {
                _Parent.SwitchState(new State1(_Parent));
            }
            Thread.Sleep(50);
            base.OnUpdate();
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