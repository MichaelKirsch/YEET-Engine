using System;
using OpenTK.Windowing.Common.Input;
using System.Drawing;
using Assimp;
using Image = System.Drawing.Image;

namespace YEET.Engine.Core
{
    public static class CursorControl
    {
        public static void SetDefault()
        {
            StateMaschine.Context.Cursor = MouseCursor.Default;
        }

        public static void SetPredefined(MouseCursor input)
        {
            StateMaschine.Context.Cursor = input;
        }

        public static void SetIcon(string name)
        {
            try
            {
                var x = Image.FromFile($"Models/Icons/{name}");
            }
            catch (Exception e)
            {
                SetDefault();
                Console.WriteLine(e);
            }
            
        }
    }
}