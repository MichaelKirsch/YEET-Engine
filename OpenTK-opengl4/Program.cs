using System;
using OpenTK.Windowing.Desktop;


namespace OpenTK_opengl4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            StateMaschine stateMaschine = new StateMaschine();
            stateMaschine.Run(new State1(stateMaschine));
            
            
        }
    }
}