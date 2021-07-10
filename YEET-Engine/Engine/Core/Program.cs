using System;
using OpenTK.Windowing.Desktop;


namespace YEET
{
    class Program
    {
        static void Main(string[] args)
        {
            StateMaschine.Run(new MainGame(), 60, 60);
        }
    }
}