using System.Collections.Generic;
using System.Collections;
using OpenTK.Mathematics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace YEET
{
    public class Mesh : Component
    {
        
        public ShaderLoader Loader { get; private set; }
        public float MaxX, MinX, MaxY, MinY, MaxZ, MinZ;
        public enum VertexAttribType
        {
            None = 0, Float = 1, V2 = 2, V3 = 3, V4 = 4
        }

        private BufferUsageHint Usage = BufferUsageHint.StaticDraw;
        private bool reloadStructure, reloadData = false;
        public List<float> Data = new List<float>();
        private List<VertexAttribType> VertexAttribStructure = new List<VertexAttribType>();

        public int VAO { get; private set; }
        public int VBO { get; private set; }
        public Queue<Tuple<string,float>> FloatUniforms = new Queue<Tuple<string, float>>();
        public Queue<Tuple<string,Vector3>> Vec3Uniforms = new Queue<Tuple<string, Vector3>>();
        public Queue<Tuple<string,Vector4>> Vec4Uniforms = new Queue<Tuple<string, Vector4>>();
        public Queue<Tuple<string,Matrix4>> Mat4Uniforms = new Queue<Tuple<string, Matrix4>>();
        


        bool OBJMesh = false;
        private OBJLoader _objloader;

        public void SetUniform(string name,Vector3 input){
            Vec3Uniforms.Enqueue(new Tuple<string, Vector3>(name,input));
        }

        public void SetUniform(string name,Vector4 input){
            Vec4Uniforms.Enqueue(new Tuple<string, Vector4>(name,input));
        }

        public void SetUniform(string name,float input){
            FloatUniforms.Enqueue(new Tuple<string, float>(name,input));
        }

        public void SetUniform(string name,Matrix4 input){
            Mat4Uniforms.Enqueue(new Tuple<string, Matrix4>(name,input));
        }
        public PrimitiveType PrimitiveTypeToRender = PrimitiveType.Triangles;

        public Mesh(Entity owner,ShaderLoader loader) :base(owner) 
        {
            VAO = 0;
            VBO =0;
            Loader = loader;
        }

        public Mesh(Entity owner,String model_name) :base(owner) 
        {
            VAO = 0;
            VBO =0;
            Loader = new ShaderLoader();
            GetFromOBJ(model_name);
        }

        public void GetFromOBJ(string path, string shader_name = "FlatShadedModel")
        {
            Loader = new ShaderLoader(shader_name);
            _objloader = new OBJLoader(path, Loader);
            OBJMesh = true;
        }

        public void AddData(List<float> newData)
        {
            Data.AddRange(newData);
            GenerateBuffers();
        }

        public void AddData(List<float> newData, List<VertexAttribType> newStructure)
        {
            Data.AddRange(newData);
            VertexAttribStructure = newStructure;
            reloadStructure = true;
            reloadData = true;
            GenerateBuffers();
        }

        public void SetData(List<float> newData)
        {
            Data = newData;
            reloadData = true;
            GenerateBuffers();
        }

        public void SetData(List<float> newData, List<VertexAttribType> newStructure)
        {
            Data = newData;
            reloadStructure = true;
            VertexAttribStructure = newStructure;
            reloadData = true;
            GenerateBuffers();
        }

        public void SetData(List<Vector3> newData, List<VertexAttribType> newStructure)
                {
                    foreach(Vector3 dp in newData){
                        Data.Add(dp.X); 
                        Data.Add(dp.Y); 
                        Data.Add(dp.Z); 
                    }
                    reloadStructure = true;
                    VertexAttribStructure = newStructure;
                    reloadData = true;
                    GenerateBuffers();
                }


        public void SetStructure(List<VertexAttribType> newStructure)
        {
            VertexAttribStructure = newStructure;
            reloadStructure = true;
            GenerateBuffers();
        }

        public void SetUsage(BufferUsageHint newHint){
            Usage = newHint;
            reloadData =true;
            GenerateBuffers();
        }


        public void GenerateBuffers()
        {
            OBJMesh = false;
            if (VAO == 0)
                VAO = GL.GenVertexArray();
            if (VBO == 0)
                VBO = GL.GenBuffer();
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            if (reloadData)
                GL.BufferData(BufferTarget.ArrayBuffer, Data.Count * sizeof(float), Data.ToArray(), Usage);
            if (reloadStructure)
            {
                reloadStructure = false;
                int stride = 0;
                foreach (var el in VertexAttribStructure)
                {
                    stride += (int)el;
                }
                int offset = 0;
                for (int i = 0; i < VertexAttribStructure.Count; i++)
                {
                    if ((int)VertexAttribStructure[i] != 0)
                    {
                        GL.VertexAttribPointer(i, (int)VertexAttribStructure[i], VertexAttribPointerType.Float, false, stride * sizeof(float), offset * sizeof(float));
                        GL.EnableVertexAttribArray(i);
                        offset += (int)VertexAttribStructure[i];
                    }
                }
                GL.BindVertexArray(0);
            }

        }


        public override void OnDraw()
        {
            base.OnDraw();
            if(OBJMesh){
                _objloader.Draw(Owner.GetComponent<Transform>().ModelMatrix);
                return;
            }
            if(VertexAttribStructure.Count == 0)
                throw new ArgumentException("Mesh loaded without structure");
            Loader.UseShader();
            while(FloatUniforms.Count !=0)
            {
                var f = FloatUniforms.Dequeue();
                Loader.SetUniformFloat(f.Item1,f.Item2);
            }
            
            while(Vec3Uniforms.Count!=0)
            {
                var f = Vec3Uniforms.Dequeue();
                Loader.SetUniformVec3(f.Item1,f.Item2);
            }

            while(Mat4Uniforms.Count!=0){
                var f = Mat4Uniforms.Dequeue();
                Loader.SetUniformMatrix4F(f.Item1,f.Item2);
            }
                
            Loader.SetUniformMatrix4F("projection", Camera.Projection);
            Loader.SetUniformMatrix4F("view", Camera.View);
            Loader.SetUniformMatrix4F("model", Owner.GetComponent<Transform>().ModelMatrix);
            GL.BindVertexArray(VAO);
            if(PrimitiveTypeToRender == PrimitiveType.Triangles){
                GL.DrawArrays(PrimitiveType.Triangles, 0, Data.Count / 3);
            } //standard{

            else{
                switch(PrimitiveTypeToRender){
                    case PrimitiveType.Lines:
                        GL.DrawArrays(PrimitiveType.Triangles, 0, Data.Count / 2);
                        break;
                    case PrimitiveType.Points:
                        GL.DrawArrays(PrimitiveType.Triangles, 0, Data.Count);
                        break;
                }
            }    
            GL.BindVertexArray(0);

    
        }

    }
}

