using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.IO;
using ImGuiNET;
namespace YEET.Engine.Core
{
    static class Util
    {
        [Pure]
        public static float Clamp(float value, float min, float max)
        {
            return value < min ? min : value > max ? max : value;
        }

        [Conditional("DEBUG")]
        public static void CheckGLError(string title)
        {
            var error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                Debug.Print($"{title}: {error}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LabelObject(ObjectLabelIdentifier objLabelIdent, int glObject, string name)
        {
            GL.ObjectLabel(objLabelIdent, glObject, name.Length, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateTexture(TextureTarget target, string Name, out int Texture)
        {
            GL.CreateTextures(target, 1, out Texture);
            LabelObject(ObjectLabelIdentifier.Texture, Texture, $"Texture: {Name}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateProgram(string Name, out int Program)
        {
            Program = GL.CreateProgram();
            LabelObject(ObjectLabelIdentifier.Program, Program, $"Program: {Name}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateShader(ShaderType type, string Name, out int Shader)
        {
            Shader = GL.CreateShader(type);
            LabelObject(ObjectLabelIdentifier.Shader, Shader, $"Shader: {type}: {Name}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateBuffer(string Name, out int Buffer)
        {
            GL.CreateBuffers(1, out Buffer);
            LabelObject(ObjectLabelIdentifier.Buffer, Buffer, $"Buffer: {Name}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateVertexBuffer(string Name, out int Buffer) => CreateBuffer($"VBO: {Name}", out Buffer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateElementBuffer(string Name, out int Buffer) => CreateBuffer($"EBO: {Name}", out Buffer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateVertexArray(string Name, out int VAO)
        {
            GL.CreateVertexArrays(1, out VAO);
            LabelObject(ObjectLabelIdentifier.VertexArray, VAO, $"VAO: {Name}");
        }

        public class StopWatchSeconds
        {
            public StopWatchSeconds()
            {
                _stopwatch = new Stopwatch();
                _stopwatch.Start();
            }

            public double result()
            {
                _stopwatch.Stop();
                TimeSpan stopwatchElapsed = _stopwatch.Elapsed;
                return stopwatchElapsed.TotalSeconds;
            }

            private Stopwatch _stopwatch;

        }
        
        public class StopWatchMilliseconds
        {
            public StopWatchMilliseconds()
            {
                _stopwatch = new Stopwatch();
                _stopwatch.Start();
            }

            public double Result()
            {
                _stopwatch.Stop();
                TimeSpan stopwatchElapsed = _stopwatch.Elapsed;
                return stopwatchElapsed.TotalMilliseconds;
            }

            private Stopwatch _stopwatch;
        }

        
        public static Int64 GetBitValue(ref Int64 to_extract_from, int position, int bitcount)
        {
            Int64 mask0 = 0;
            for (int i = 0; i < bitcount; i++)
            {
                mask0 = mask0<<1;
                mask0 += 1;
                    
            }
            mask0 = mask0 << position;
            return (to_extract_from & mask0)>>position;
        } 
    }

    public class SimpleTexturedButton{
        public Vector2 size {get;set;}
        private Texture _texture;
        private string _currentpath ="";
        public SimpleTexturedButton(){
        }

        public void ChangeTexture(string new_texturepath){
            Stream image = new FileStream(new_texturepath, FileMode.Open);
            var bmap = new Bitmap(image);
            _texture = new Texture("__", bmap, true, false);
            _currentpath = new_texturepath;
            image.Close();
        }

        public bool Draw(string path,Vector2 _size){
            if(path!=_currentpath){
                ChangeTexture(path);
            }
            size = _size;
            return ImGui.ImageButton(new IntPtr(_texture.GLTexture),size);
        }
    }

}
