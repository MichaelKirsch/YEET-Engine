using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OpenTK_opengl4
{
    public class MainWindow : GameWindow
    {
        public MainWindow(NativeWindowSettings nativeWindowSettings, GameWindowSettings gameWindowSettings,int frameRate,int updateRate) : 
            base(gameWindowSettings,nativeWindowSettings)
        {
            UpdateFrequency = updateRate;
            RenderFrequency = frameRate;
        }
        
    }
}