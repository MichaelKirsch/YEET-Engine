
using System;
using ImGuiNET;
using OpenTK.Mathematics;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace YEET
{
    public class Gradient : Component
    {
        ShaderLoader loader;
        int vbo,vao,framebuffer,texture;
        float current_total_weight;
        public static List<Vector4> colors = new List<Vector4>();
        public Gradient(Entity owner) : base(owner)
        {
            loader = new ShaderLoader("Gradient");
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            framebuffer = GL.GenFramebuffer();
            texture = GL.GenTexture();
        }

        public Vector3 GetColor(float pos){
            return new Vector3();
        }

        public override void OnGUI()
        {
            base.OnGUI();
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(700,500));

            ImGui.Begin("Skybox");
            if(ImGui.Button("Add Color")){
                colors.Add(new Vector4(0,0,0,200));
            }
            ImGui.SameLine();
            if(ImGui.Button("Remove Color")){
                if(colors.Count>0){colors.RemoveAt(colors.Count-1);}
            }
            ImGui.Separator();

            float totalweight = 0;

            for(int x=0;x<colors.Count;x++)
            {
                System.Numerics.Vector3 placeholder = new System.Numerics.Vector3(colors[x].X,colors[x].Y,colors[x].Z);
                float weightplaceholder = colors[x].W;
                ImGui.SetNextItemWidth(300);
                ImGui.ColorEdit3($"Color{x}",ref placeholder);
                totalweight+=weightplaceholder;
                colors[x] = new Vector4(placeholder.X,placeholder.Y,placeholder.Z,weightplaceholder);
            }
            ImGui.Separator();
            ImGui.TextColored(ColorHelper.ConvertColor4(System.Drawing.Color.Aqua),"Weights");
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing,new System.Numerics.Vector2(2,1));
            for(int x=0;x<colors.Count;x++)
            {
                System.Numerics.Vector3 placeholder = new System.Numerics.Vector3(colors[x].X,colors[x].Y,colors[x].Z);
                float weightplaceholder = colors[x].W;
                
                ImGui.SetNextItemWidth((weightplaceholder/totalweight)*700);
                ImGui.PushStyleColor(ImGuiCol.FrameBg,new System.Numerics.Vector4(placeholder.X,placeholder.Y,placeholder.Z,1f));
                ImGui.PushStyleColor(ImGuiCol.FrameBgHovered,new System.Numerics.Vector4(placeholder.X,placeholder.Y,placeholder.Z,1f));
                ImGui.PushStyleColor(ImGuiCol.FrameBgActive,new System.Numerics.Vector4(placeholder.X,placeholder.Y,placeholder.Z,1f));
                ImGui.PushStyleColor(ImGuiCol.Text,new System.Numerics.Vector4(placeholder.X,placeholder.Y,placeholder.Z,1f));
                ImGui.PushStyleColor(ImGuiCol.SliderGrab,new System.Numerics.Vector4(1,1,1,1f));
                ImGui.DragFloat($"##{x}",ref weightplaceholder);
                if(weightplaceholder<1) weightplaceholder=1;
                ImGui.PopStyleColor(5);
                ImGui.SameLine();
                colors[x] = new Vector4(placeholder.X,placeholder.Y,placeholder.Z,weightplaceholder);
            }
            ImGui.PopStyleVar();
            ImGui.Separator();
            ImGui.TextColored(ColorHelper.ConvertColor4(System.Drawing.Color.OrangeRed),"Preview");
            ImGui.Text($"Totalweight:{current_total_weight}");
            ImGui.End();
            current_total_weight = totalweight;

             GL.BindVertexArray(vao);
            List<Vector3> vertexData = new List<Vector3>();
            float offset =-1f;
            for(int x=0;x<colors.Count;x++)
            {
                float width = (colors[x].W/current_total_weight)*2f;
                //Left Triangle
                vertexData.Add(new Vector3(offset,-1,0));
                vertexData.Add(colors[x].Xyz);
                vertexData.Add(new Vector3(offset,1,0));
                vertexData.Add(colors[x].Xyz);
                vertexData.Add(new Vector3(offset+width,-1,0));
                vertexData.Add(x==colors.Count-1?colors[x].Xyz:colors[x+1].Xyz);
                //Right Triangle
                vertexData.Add(new Vector3(offset+width,-1,0));
                vertexData.Add(x==colors.Count-1?colors[x].Xyz:colors[x+1].Xyz);
                vertexData.Add(new Vector3(offset,1,0));
                vertexData.Add(colors[x].Xyz);
                vertexData.Add(new Vector3(offset+width,1,0));
                vertexData.Add(x==colors.Count-1?colors[x].Xyz:colors[x+1].Xyz);
                offset+=width;
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer,vbo);
            GL.BufferData(BufferTarget.ArrayBuffer,vertexData.Count*3*sizeof(float),vertexData.ToArray(),BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(0,3,VertexAttribPointerType.Float,false,6*sizeof(float),0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1,3,VertexAttribPointerType.Float,false,6*sizeof(float),3*sizeof(float));
            GL.EnableVertexAttribArray(1);
            loader.UseShader();
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles,0,vertexData.Count/2);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
        }

        public override void OnDraw()
        {
            base.OnDraw();
           
        }

    }
}