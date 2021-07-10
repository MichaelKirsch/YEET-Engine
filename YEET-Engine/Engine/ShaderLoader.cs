using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace YEET
{
    public struct UniformFieldInfo
    {
        public int Location;
        public string Name;
        public int Size;
        public ActiveUniformType Type;
    }

    public class ShaderLoader
    {
        public readonly string Name;
        public int Program { get; private set; }
        private readonly Dictionary<string, int> UniformToLocation = new Dictionary<string, int>();
        private bool Initialized = false;

        private readonly (ShaderType Type, string Path)[] Files;
        
        /// <summary>
        /// Load Shaders from File or from Strings. Shaders have to be placed n the "Shaders"-Folder
        /// </summary>
        /// <param name="name"></param>
        /// <param name="vertexShader"></param>
        /// <param name="fragmentShader"></param>
        /// <param name="isFromFile"></param>
        public ShaderLoader(string name, string vertexShader, string fragmentShader,bool isFromFile=false)
        {
            
            Name = name;
            if (!isFromFile)
            {
                Files = new[]{
                    (ShaderType.VertexShader, vertexShader),
                    (ShaderType.FragmentShader, fragmentShader),
                };
            }
            else
            {
                string VertexShaderSource;
                using (StreamReader reader = new StreamReader("Shaders/" + vertexShader+".vert", Encoding.UTF8))
                {
                    VertexShaderSource = reader.ReadToEnd();
                }

                string FragmentShaderSource;

                using (StreamReader reader = new StreamReader("Shaders/" +fragmentShader+".frag", Encoding.UTF8))
                {
                    FragmentShaderSource = reader.ReadToEnd();
                }
                Files = new[]{
                    (ShaderType.VertexShader, VertexShaderSource),
                    (ShaderType.FragmentShader, FragmentShaderSource),
                };
            }
            Program = CreateProgram(name, Files);
        }

        public ShaderLoader(string name)
        {
            Name = name;
            string VertexShaderSource;
                using (StreamReader reader = new StreamReader("Shaders/" + name+".vert", Encoding.UTF8))
                {
                    VertexShaderSource = reader.ReadToEnd();
                }

                string FragmentShaderSource;

                using (StreamReader reader = new StreamReader("Shaders/" +name+".frag", Encoding.UTF8))
                {
                    FragmentShaderSource = reader.ReadToEnd();
                }
                Files = new[]{
                    (ShaderType.VertexShader, VertexShaderSource),
                    (ShaderType.FragmentShader, FragmentShaderSource),
                };
                Program = CreateProgram(name, Files);
        }
        
        public ShaderLoader()
        {
            string name = "FlatShadedModel";
            Name = name;
            string VertexShaderSource;
            using (StreamReader reader = new StreamReader("Shaders/" + name+".vert", Encoding.UTF8))
            {
                VertexShaderSource = reader.ReadToEnd();
            }

            string FragmentShaderSource;

            using (StreamReader reader = new StreamReader("Shaders/" +name+".frag", Encoding.UTF8))
            {
                FragmentShaderSource = reader.ReadToEnd();
            }
            Files = new[]{
                (ShaderType.VertexShader, VertexShaderSource),
                (ShaderType.FragmentShader, FragmentShaderSource),
            };
            Program = CreateProgram(name, Files);
        }
        
        
        
        public void UseShader()
        {
            GL.UseProgram(Program);
        }

        public void Dispose()
        {
            if (Initialized)
            {
                GL.DeleteProgram(Program);
                Initialized = false;
            }
        }

        UniformFieldInfo[] GetUniforms()
        {
            GL.GetProgram(Program, GetProgramParameterName.ActiveUniforms, out int UnifromCount);

            UniformFieldInfo[] Uniforms = new UniformFieldInfo[UnifromCount];

            for (int i = 0; i < UnifromCount; i++)
            {
                string Name = GL.GetActiveUniform(Program, i, out int Size, out ActiveUniformType Type);

                UniformFieldInfo FieldInfo;
                FieldInfo.Location = GetUniformLocation(Name);
                FieldInfo.Name = Name;
                FieldInfo.Size = Size;
                FieldInfo.Type = Type;

                Uniforms[i] = FieldInfo;
            }

            return Uniforms;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetUniformLocation(string uniform)
        {
            if (UniformToLocation.TryGetValue(uniform, out int location) == false)
            {
                location = GL.GetUniformLocation(Program, uniform);
                UniformToLocation.Add(uniform, location);

                if (location == -1)
                {
                    Console.WriteLine($"The uniform '{uniform}' does not exist in the shader '{Name}'!");
                }
            }
            
            return location;
        }

        private int CreateProgram(string name, params (ShaderType Type, string source)[] shaderPaths)
        {
            Util.CreateProgram(name, out int Program);

            int[] Shaders = new int[shaderPaths.Length];
            for (int i = 0; i < shaderPaths.Length; i++)
            {
                Shaders[i] = CompileShader(name, shaderPaths[i].Type, shaderPaths[i].source);
            }

            foreach (var shader in Shaders)
                GL.AttachShader(Program, shader);

            GL.LinkProgram(Program);

            GL.GetProgram(Program, GetProgramParameterName.LinkStatus, out int Success);
            if (Success == 0)
            {
                string Info = GL.GetProgramInfoLog(Program);
                Debug.WriteLine($"GL.LinkProgram had info log [{name}]:\n{Info}");
            }

            foreach (var Shader in Shaders)
            {
                GL.DetachShader(Program, Shader);
                GL.DeleteShader(Shader);
            }

            Initialized = true;

            return Program;
        }

        private int CompileShader(string name, ShaderType type, string source)
        {
            Util.CreateShader(type, name, out int Shader);
            GL.ShaderSource(Shader, source);
            GL.CompileShader(Shader);

            GL.GetShader(Shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string Info = GL.GetShaderInfoLog(Shader);
                Console.WriteLine($"GL.CompileShader for shader '{Name}' [{type}] had info log:\n{Info}");
            }
            
            return Shader;
        }

        public List<UniformFieldInfo> GetAllUniforms(){
            var x = GetUniforms();
            var  r  =new List<UniformFieldInfo>();
            foreach (var item in x)
            {
                r.Add(item);
            }
            return r;
        }


        public void SetUniformMatrix4F(string name, Matrix4 to_set)
        {
            GL.UniformMatrix4(GetUniformLocation(name),false,ref to_set);
        }
        
        public void SetUniformFloat(string name,float to_set)
        {
            GL.Uniform1(GetUniformLocation(name),to_set);
        }
        
        public void SetUniformVec3(string name,Vector3 to_set)
        {
            GL.Uniform3(GetUniformLocation(name),to_set);
        }
        
        public void SetUniformVec3(string name,float x, float y,float z)
        {
            GL.Uniform3(GetUniformLocation(name),new Vector3(x,y,z));
        }
        
    }
}
