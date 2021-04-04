using System;
using OpenTK.Windowing.Desktop;


namespace YEET
{
    class Program
    {
        static void Main(string[] args)
        {
            StateMaschine.Run(new RenderingTest(), 60, 60);
        }
    }
}