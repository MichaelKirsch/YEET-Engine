using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;

namespace YEET{
    public  class Skybox : Entity{
        
        System.Guid _gradient;
        private ShaderLoader _loader = new ("Skybox");
        public int size = 1000;
        private int VBO,VAO,cubeTexture;
        
        float[] skyboxVertices = {
            // positions          
            -1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            1.0f, -1.0f, -1.0f,
            1.0f, -1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            -1.0f,  1.0f, -1.0f,
            1.0f,  1.0f, -1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
            1.0f, -1.0f,  1.0f
        };
        
        public Skybox()
        {

            Name = "Skybox";
            _gradient = AddComponent(new Gradient(this));
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            GL.EnableVertexArrayAttrib(VAO,0);
            VBO=GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer,VBO);
            GL.BufferData(BufferTarget.ArrayBuffer,4*36*sizeof(float),skyboxVertices,BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0,3,VertexAttribPointerType.Float,false,0,new IntPtr());

            cubeTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap,cubeTexture);
            for(int i = 0; i < 6; i++)
                {
                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX+i,0,PixelInternalFormat.Rgb,800,600,0,PixelFormat.Rgb,PixelType.Float,new IntPtr());
                }
        }

        public override void OnGui()
        {
            base.OnGui();
        }

        public override void OnRender()
        {
            GL.DepthMask(false);
            _loader.UseShader();
            _loader.SetUniformMatrix4F("projection",Camera.Projection);
            _loader.SetUniformMatrix4F("view",Camera.View);
            GL.BindVertexArray(VAO);
            GL.BindTexture(TextureTarget.TextureCubeMap,GetComponent<Gradient>(_gradient).GetTextureAddress());
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.DepthMask(true);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            GetComponent<Transform>().Position = Camera.Position;
        }
    }
}