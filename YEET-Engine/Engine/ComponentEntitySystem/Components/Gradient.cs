using System;
using ImGuiNET;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using System.Numerics;
using YEET.Engine.Core;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;




namespace YEET.Engine.ECS
{
    public class Gradient : Component
    {
        public bool GenerateCubeMapTexture = false;

        ShaderLoader loader;
        int vbo, vao, framebuffer, texture;
        float current_total_weight;
        public List<Vector4> colors = new List<Vector4>();
        public Gradient(Entity owner) : base(owner)
        {
            loader = new ShaderLoader("Gradient");
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            framebuffer = GL.GenFramebuffer();
            texture = GL.GenTexture();
            
        }

        public Vector3 GetColor(float pos)
        {
            return new Vector3();
        }

        public IntPtr GetTexturePointer()
        {
            return new IntPtr(texture);
        }

        public int GetTextureAddress()
        {
            return texture;
        }

        public override void OnGUI()
        {
            base.OnGUI();
            ImGui.BeginChild($"{CompID}");
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(700, 500));

            ImGui.Text("Gradient");
            if (ImGui.Button("Add Color"))
            {
                colors.Add(new Vector4(0, 0, 0, 200));
            }

            ImGui.SameLine();
            if (ImGui.Button("Remove Color"))
            {
                if (colors.Count > 0)
                {
                    colors.RemoveAt(colors.Count - 1);
                }
            }

            ImGui.Separator();
            ImGui.Checkbox("CubeMap", ref GenerateCubeMapTexture);
            ImGui.Separator();

            ImGui.TextColored(ColorHelper.ConvertColor4(System.Drawing.Color.OrangeRed), "Preview");
            ImGui.Image(new IntPtr(texture), new System.Numerics.Vector2(800, 600));
            float totalweight = 0;
            ImGui.TextColored(ColorHelper.ConvertColor4(System.Drawing.Color.Aqua), "Colors + Weights");
            for (int x = 0; x < colors.Count; x++)
            {
                System.Numerics.Vector3 placeholder =
                    new System.Numerics.Vector3(colors[x].X, colors[x].Y, colors[x].Z);
                float weightplaceholder = colors[x].W;
                ImGui.SetNextItemWidth(300);
                ImGui.ColorEdit3($"Color{x}##{CompID}", ref placeholder);
                totalweight += weightplaceholder;
                colors[x] = new Vector4(placeholder.X, placeholder.Y, placeholder.Z, weightplaceholder);
            }

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(2, 1));
            for (int x = 0; x < colors.Count; x++)
            {
                System.Numerics.Vector3 placeholder =
                    new System.Numerics.Vector3(colors[x].X, colors[x].Y, colors[x].Z);
                float weightplaceholder = colors[x].W;

                ImGui.SetNextItemWidth((weightplaceholder / totalweight) * 500);
                ImGui.PushStyleColor(ImGuiCol.FrameBg,
                    new System.Numerics.Vector4(placeholder.X, placeholder.Y, placeholder.Z, 1f));
                ImGui.PushStyleColor(ImGuiCol.FrameBgHovered,
                    new System.Numerics.Vector4(placeholder.X, placeholder.Y, placeholder.Z, 1f));
                ImGui.PushStyleColor(ImGuiCol.FrameBgActive,
                    new System.Numerics.Vector4(placeholder.X, placeholder.Y, placeholder.Z, 1f));
                ImGui.PushStyleColor(ImGuiCol.Text,
                    new System.Numerics.Vector4(placeholder.X, placeholder.Y, placeholder.Z, 1f));
                ImGui.PushStyleColor(ImGuiCol.SliderGrab, new System.Numerics.Vector4(1, 1, 1, 1f));
                ImGui.DragFloat($"##{x}##{CompID}", ref weightplaceholder);
                if (weightplaceholder < 1) weightplaceholder = 1;
                ImGui.PopStyleColor(5);
                ImGui.SameLine();
                colors[x] = new Vector4(placeholder.X, placeholder.Y, placeholder.Z, weightplaceholder);
            }

            ImGui.PopStyleVar();
            ImGui.Separator();
            GenerateTexture();
            current_total_weight = totalweight;
            ImGui.EndChild();
        }

        public override void OnUpdate()
        {
        }

        void GenerateTexture()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);

            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, 800, 600, 0, PixelFormat.Rgb,
                PixelType.UnsignedByte, new System.IntPtr());
            GL.TextureParameter(texture, TextureParameterName.TextureMinFilter, (int) TextureMagFilter.Linear);
            GL.TextureParameter(texture, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, texture, 0);


            DrawBuffersEnum[] drawBuffersEnum = {DrawBuffersEnum.ColorAttachment0};
            GL.DrawBuffers(1, drawBuffersEnum);
            GL.BindVertexArray(vao);
            List<Vector3> vertexData = new List<Vector3>();

            float xoffset = 1;
            float offset = GenerateCubeMapTexture ? -0.5f : -1f;
            for (int x = 0; x < colors.Count; x++)
            {
                float width = (colors[x].W / current_total_weight) * (GenerateCubeMapTexture ? 1f : 2f);
                //Left Triangle
                vertexData.Add(new Vector3(-xoffset, offset, 0));
                vertexData.Add(colors[x].Xyz);
                vertexData.Add(new Vector3(xoffset, offset, 0));
                vertexData.Add(colors[x].Xyz);
                vertexData.Add(new Vector3(-xoffset, offset + width, 0));
                vertexData.Add(x == colors.Count - 1 ? colors[x].Xyz : colors[x + 1].Xyz);
                //Right Triangle
                vertexData.Add(new Vector3(-xoffset, offset + width, 0));
                vertexData.Add(x == colors.Count - 1 ? colors[x].Xyz : colors[x + 1].Xyz);
                vertexData.Add(new Vector3(xoffset, offset, 0));
                vertexData.Add(colors[x].Xyz);
                vertexData.Add(new Vector3(xoffset, offset + width, 0));
                vertexData.Add(x == colors.Count - 1 ? colors[x].Xyz : colors[x + 1].Xyz);
                offset += width;
            }


            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Count * 3 * sizeof(float), vertexData.ToArray(),
                BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            loader.UseShader();
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexData.Count / 2);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        
        
        public override void OnDraw()
        {
            base.OnDraw();
            
        }
    }
}