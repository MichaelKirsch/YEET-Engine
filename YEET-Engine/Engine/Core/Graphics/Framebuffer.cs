using System;
using OpenTK.Graphics.OpenGL4;
using YEET.Engine.Core;

namespace YEET
{
    public class Framebuffer
    {
        public int framebufferID, texture, depth_texture, stmVAO, stmVBO;
        public ShaderLoader stmLoader;
        public Framebuffer()
        {
            stmLoader = new ShaderLoader("FrameBufferDefault");
            GenerateBuffers();
        }
        
        private void GenerateBuffers()
        {
            framebufferID = GL.GenFramebuffer();
            texture = GL.GenTexture();
            depth_texture = GL.GenTexture();
            stmVAO = GL.GenVertexArray();
            stmVBO = GL.GenBuffer();
            
            GL.BindVertexArray(stmVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, stmVBO);
            float[] rectangleVertices =
            {
                // Coords    // texCoords
                1.0f, -1.0f, 1.0f, 0.0f,
                -1.0f, -1.0f, 0.0f, 0.0f,
                -1.0f, 1.0f, 0.0f, 1.0f,

                1.0f, 1.0f, 1.0f, 1.0f,
                1.0f, -1.0f, 1.0f, 0.0f,
                -1.0f, 1.0f, 0.0f, 1.0f
            };
            GL.BufferData(BufferTarget.ArrayBuffer, rectangleVertices.Length * sizeof(float), rectangleVertices,
                BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), new IntPtr(0));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float),
                new IntPtr(2 * sizeof(float)));
        }
        
       public void ClearFramebuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferID);
            GL.Clear(ClearBufferMask.DepthBufferBit |
                     ClearBufferMask.ColorBufferBit); // we're not using the stencil buffer now

            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, StateMaschine.Context.Size.X, StateMaschine.Context.Size.Y, 0,
                PixelFormat.Rgb,
                PixelType.UnsignedByte, new System.IntPtr());
            GL.TextureParameter(texture, TextureParameterName.TextureMinFilter, (int) TextureMagFilter.Nearest);
            GL.TextureParameter(texture, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);
            GL.TextureParameter(texture, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge);
            GL.TextureParameter(texture, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, texture, 0);

            GL.BindTexture(TextureTarget.Texture2D, depth_texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, StateMaschine.Context.Size.X,
                StateMaschine.Context.Size.Y, 0, PixelFormat.DepthComponent, PixelType.Float, new System.IntPtr());
            GL.TextureParameter(depth_texture, TextureParameterName.TextureMinFilter, (int) TextureMagFilter.Linear);
            GL.TextureParameter(depth_texture, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
            GL.TextureParameter(depth_texture, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge);
            GL.TextureParameter(depth_texture, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge);
            GL.TextureParameter(depth_texture, TextureParameterName.TextureCompareMode, (int) TextureCompareMode.None);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                TextureTarget.Texture2D, depth_texture, 0);
            //DrawBuffersEnum[] drawBuffersEnum = {DrawBuffersEnum.ColorAttachment0};
            //GL.DrawBuffers(1, drawBuffersEnum);
        }
        
       protected void RenderTo()
       {
           GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferID);
           stmLoader.UseShader();
           GL.Disable(EnableCap.DepthTest);
           GL.BindVertexArray(stmVAO);
           GL.ActiveTexture(TextureUnit.Texture0);
           GL.BindTexture(TextureTarget.Texture2D, texture);
           GL.ActiveTexture(TextureUnit.Texture1);
           GL.BindTexture(TextureTarget.Texture2D, depth_texture);
           stmLoader.SetUniformInt("colorTexture", 0);
           stmLoader.SetUniformInt("depthTexture", 1);
           stmLoader.SetUniformVec3("cameraPos",Camera.Position);
           stmLoader.SetUniformVec3("cameraFront",Camera.Front);
           stmLoader.SetUniformVec3("sunpos",Sun.getSunPosition());
           stmLoader.SetUniformVec3("suncolor",Sun.getColor());
           OpenTK.Mathematics.Matrix4 invper;
           OpenTK.Mathematics.Matrix4.Invert(Camera.Projection,out invper);
           OpenTK.Mathematics.Matrix4 invview;
           OpenTK.Mathematics.Matrix4.Invert(Camera.View,out invview);
           stmLoader.SetUniformMatrix4F("invperspective",invper);
           stmLoader.SetUniformMatrix4F("invview",invview);
           stmLoader.SetUniformFloat("time",Convert.ToSingle(StateMaschine.GetElapsedTime()));
           stmLoader.SetUniformVec3("screen",new OpenTK.Mathematics.Vector3(StateMaschine.Context.Size.X,StateMaschine.Context.Size.Y,0));
           GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
           GL.Enable(EnableCap.DepthTest);
       }
        
    }
}