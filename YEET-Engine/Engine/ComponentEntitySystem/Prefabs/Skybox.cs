using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using YEET.Engine.Core;

namespace YEET.Engine.ECS{
    /// <summary>
    /// The skybox is just a simple box of a given size. the real texturing is done by the post processing shader.
    /// </summary>
    public  class Skybox : Entity{
        
        System.Guid _gradient;
        private ShaderLoader _loader = new ("Skybox");
        public int size = 1000;
        private int VBO,VAO,cubeTexture;
        
        float[] skyboxVertices = {
            // positions          
            -500.0f,  500.0f, -500.0f,
            -500.0f, -500.0f, -500.0f,
            500.0f, -500.0f, -500.0f,
            500.0f, -500.0f, -500.0f,
            500.0f,  500.0f, -500.0f,
            -500.0f,  500.0f, -500.0f,

            -500.0f, -500.0f,  500.0f,
            -500.0f, -500.0f, -500.0f,
            -500.0f,  500.0f, -500.0f,
            -500.0f,  500.0f, -500.0f,
            -500.0f,  500.0f,  500.0f,
            -500.0f, -500.0f,  500.0f,

            500.0f, -500.0f, -500.0f,
            500.0f, -500.0f,  500.0f,
            500.0f,  500.0f,  500.0f,
            500.0f,  500.0f,  500.0f,
            500.0f,  500.0f, -500.0f,
            500.0f, -500.0f, -500.0f,

            -500.0f, -500.0f,  500.0f,
            -500.0f,  500.0f,  500.0f,
            500.0f,  500.0f,  500.0f,
            500.0f,  500.0f,  500.0f,
            500.0f, -500.0f,  500.0f,
            -500.0f, -500.0f,  500.0f,

            -500.0f,  500.0f, -500.0f,
            500.0f,  500.0f, -500.0f,
            500.0f,  500.0f,  500.0f,
            500.0f,  500.0f,  500.0f,
            -500.0f,  500.0f,  500.0f,
            -500.0f,  500.0f, -500.0f,

            -500.0f, -500.0f, -500.0f,
            -500.0f, -500.0f,  500.0f,
            500.0f, -500.0f, -500.0f,
            500.0f, -500.0f, -500.0f,
            -500.0f, -500.0f,  500.0f,
            500.0f, -500.0f,  500.0f
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
        }

        public override void OnGui()
        {
            base.OnGui();
        }

        public override void OnRender()
        {
            _loader.UseShader();
            _loader.SetUniformMatrix4F("projection",Camera.Projection);
            _loader.SetUniformVec3("camerapos",Camera.Position);
            _loader.SetUniformMatrix4F("view",Camera.View);
            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            GetComponent<Transform>().Position = Camera.Position;
        }
    }
}