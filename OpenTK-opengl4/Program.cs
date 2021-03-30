using System;
using OpenTK.Windowing.Desktop;


namespace OpenTK_opengl4
{
    class Program
    {
        static void Main(string[] args)
        {
            StateMaschine.Run(new RenderingTest(), 60, 60);
        }
    }
}