using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace YEET.Engine.Core
{
    public struct DataStructure<T>
    {
        public T t0;
    }

    public struct DataStructure<T, T1>
    {
        public T t0;
        public T1 t1;
    }
    
    public struct DataStructure<T, T1,T2>
    {
        public T t0;
        public T1 t1;
        public T2 t2;
    }
    
    public struct DataStructure<T, T1,T2,T3>
    {
        public T t0;
        public T1 t1;
        public T2 t2;
        public T3 t3;
    }
    
    public enum BufferType
    {
        EBO,VBO,SSBO
    }
    public class Buffer<T> where T : unmanaged//generic Buffer class
    {
        public DataStructure<int, int, int> x = new DataStructure<int, int, int>();
        private List<T> _bufferedData;
        private BufferTarget _target;
        public bool KeepData = false;
        public int ID { get; }

        public Buffer(BufferTarget target)
        {
            ID = GL.GenBuffer();
            _target = target;
        }

        public void BufferData(List<T> to_buffer,BufferUsageHint hint = BufferUsageHint.StaticDraw)
        {
            Bind();
            GL.BufferData(_target,Marshal.SizeOf(to_buffer)*8,_bufferedData.ToArray(),hint);
            //yada yada buffer that shit
            
            if (!KeepData)
            {
                _bufferedData.Clear();
            }
        }

        public void Bind()
        {
            GL.BindBuffer(_target, ID);
        }
    }
}