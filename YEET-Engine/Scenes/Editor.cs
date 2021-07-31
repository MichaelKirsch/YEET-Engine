using System;
using System.Numerics;
using YEET.Engine.Core;
using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using YEET.Engine.ECS;
using Vector2 = System.Numerics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace YEET
{
    public class Editor : Scene
    {
        public Editor()
        {
           
        }

        public override void OnStart()
        {
            base.OnStart();
            AddEntity(new Grid(new Vector2i(3000, 3000),0.02f));
            Camera.Position = new Vector3(1500, 10, 1500);
            AddEntity(new Skybox());

            var x =ImGui.GetStyle();
            x.WindowPadding = new Vector2(15, 15);
            x.WindowRounding = 0f;
            x.FramePadding = new Vector2(5, 5);
            x.FrameRounding = 4f;
            x.ItemSpacing = new Vector2(12, 8);
            x.ItemInnerSpacing = new Vector2(8, 6);
            x.IndentSpacing = 25f;
            x.ScrollbarSize = 15f;
            x.ScrollbarRounding = 9f;
            x.GrabMinSize = 5f;
            x.GrabRounding = 3f;
            ImGui.PushStyleColor(ImGuiCol.Text,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.TextDisabled,new System.Numerics.Vector4(0.24f, 0.23f, 0.29f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.WindowBg,new System.Numerics.Vector4(0.06f, 0.05f, 0.07f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.ChildBg,new System.Numerics.Vector4(0.07f, 0.07f, 0.09f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.PopupBg,new System.Numerics.Vector4(0.07f, 0.07f, 0.09f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.Border,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.88f));
            ImGui.PushStyleColor(ImGuiCol.BorderShadow,new System.Numerics.Vector4(0.92f, 0.91f, 0.88f, 0.00f));
            ImGui.PushStyleColor(ImGuiCol.FrameBg,new System.Numerics.Vector4(0.10f, 0.09f, 0.12f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.FrameBgHovered,new System.Numerics.Vector4(0.24f, 0.23f, 0.29f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.FrameBgActive,new System.Numerics.Vector4(0.56f, 0.56f, 0.58f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.TitleBg,new System.Numerics.Vector4(0.10f, 0.09f, 0.12f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.TitleBgCollapsed,new System.Numerics.Vector4(1.00f, 0.98f, 0.95f, 0.75f));
            ImGui.PushStyleColor(ImGuiCol.TitleBgActive,new System.Numerics.Vector4(0.07f, 0.07f, 0.09f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.MenuBarBg,new System.Numerics.Vector4(0.10f, 0.09f, 0.12f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.ScrollbarBg,new System.Numerics.Vector4(0.10f, 0.09f, 0.12f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.ScrollbarGrab,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.31f));
            ImGui.PushStyleColor(ImGuiCol.ScrollbarGrabHovered,new System.Numerics.Vector4(0.56f, 0.56f, 0.58f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.ScrollbarGrabActive,new System.Numerics.Vector4(0.06f, 0.05f, 0.07f, 1.00f));
            //ImGui.PushStyleColor(ImGuiCol.Combo,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.88f));
            ImGui.PushStyleColor(ImGuiCol.CheckMark,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.31f));
            ImGui.PushStyleColor(ImGuiCol.SliderGrab,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.31f));
            ImGui.PushStyleColor(ImGuiCol.SliderGrabActive,new System.Numerics.Vector4(0.06f, 0.05f, 0.07f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.Button,new System.Numerics.Vector4(0.10f, 0.09f, 0.12f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered,new System.Numerics.Vector4(0.24f, 0.23f, 0.29f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive,new System.Numerics.Vector4(0.56f, 0.56f, 0.58f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.Header,new System.Numerics.Vector4(0.10f, 0.09f, 0.12f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.HeaderHovered,new System.Numerics.Vector4(0.56f, 0.56f, 0.58f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.HeaderActive,new System.Numerics.Vector4(0.06f, 0.05f, 0.07f, 1.00f));
            //ImGui.PushStyleColor(ImGuiCol,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.88f));
            //ImGui.PushStyleColor(ImGuiCol.Border,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.88f));
            //ImGui.PushStyleColor(ImGuiCol.Border,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.88f));
            ImGui.PushStyleColor(ImGuiCol.ResizeGrip,new System.Numerics.Vector4(0.00f, 0.00f, 0.00f, 0.00f));
            ImGui.PushStyleColor(ImGuiCol.ResizeGripHovered,new System.Numerics.Vector4(0.56f, 0.56f, 0.58f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.ResizeGripActive,new System.Numerics.Vector4(0.06f, 0.05f, 0.07f, 1.00f));
            //ImGui.PushStyleColor(ImGuiCol.butt,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.88f));
            //ImGui.PushStyleColor(ImGuiCol.Border,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.88f));
            //ImGui.PushStyleColor(ImGuiCol.Border,new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.88f));
            ImGui.PushStyleColor(ImGuiCol.PlotLines,new System.Numerics.Vector4(0.40f, 0.39f, 0.38f, 0.63f));
            ImGui.PushStyleColor(ImGuiCol.PlotLinesHovered,new System.Numerics.Vector4(0.25f, 1.00f, 0.00f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.PlotHistogram,new System.Numerics.Vector4(0.40f, 0.39f, 0.38f, 0.63f));
            ImGui.PushStyleColor(ImGuiCol.PlotHistogramHovered,new System.Numerics.Vector4(0.25f, 1.00f, 0.00f, 1.00f));
            ImGui.PushStyleColor(ImGuiCol.TextSelectedBg,new System.Numerics.Vector4(0.25f, 1.00f, 0.00f, 0.43f));
            ImGui.PushStyleColor(ImGuiCol.ModalWindowDimBg,new System.Numerics.Vector4(1.00f, 0.98f, 0.95f, 0.73f));
            
        }

        public override void OnGui()
        {
            base.OnGui();
            var window_size = StateMaschine.Context.Size;
            //inspector
            var inspectorwidth = 0.2f;

            
                
            ImGui.SetNextWindowSize(new Vector2(window_size.X*inspectorwidth,window_size.Y*0.7f));
            ImGui.SetNextWindowPos(new Vector2((1f-inspectorwidth)*window_size.X,0));
            ImGui.Begin("inspector", ImGuiWindowFlags.NoCollapse|ImGuiWindowFlags.NoResize|ImGuiWindowFlags.NoNav|ImGuiWindowFlags.NoTitleBar);
            ImGui.Text("Inspector");
            ImGui.Separator();
            ImGui.End();
            ImGui.SetNextWindowSize(new Vector2(window_size.X*0.5f,window_size.Y*0.3f));
            ImGui.SetNextWindowPos(new Vector2(0f,window_size.Y*0.7f));
            ImGui.Begin("##rr", ImGuiWindowFlags.NoCollapse|ImGuiWindowFlags.NoResize|ImGuiWindowFlags.NoNav|ImGuiWindowFlags.NoTitleBar);
            ImGui.Text("ajdwiadjiawodjaiod");
            ImGui.Separator();
            ImGui.End();
            
            ImGui.SetNextWindowSize(new Vector2(window_size.X*0.5f,window_size.Y*0.3f));
            ImGui.SetNextWindowPos(new Vector2(window_size.X*0.5f,window_size.Y*0.7f));
            ImGui.Begin("Entities", ImGuiWindowFlags.NoCollapse|ImGuiWindowFlags.NoResize|ImGuiWindowFlags.NoNav|ImGuiWindowFlags.NoTitleBar);

            ImGui.BeginChild("letft_pane", new System.Numerics.Vector2(150, 0), true);
            foreach (var entity in Entities)
            {
                if (ImGui.Selectable($"{entity.Name}##{entity.ID}"))
                {
                    selected = entity;
                }
            }
            ImGui.EndChild();
            ImGui.SameLine();
            ImGui.BeginGroup();
            ImGui.BeginChild($"Selected Entity:{selected.ID}",
                new System.Numerics.Vector2(0, -ImGui.GetFrameHeightWithSpacing()));
            ImGui.Text($"Type:{selected.Name} ID:{selected.ID}");
            ImGui.Separator();
            selected.OnGui();
            ImGui.EndChild();
            ImGui.EndGroup();
            ImGui.End();
            ImGui.SetNextWindowSize(new Vector2(window_size.X*0.2f,window_size.Y*0.7f));
            ImGui.SetNextWindowPos(new Vector2(0f,0));
            ImGui.Begin("##r3r", ImGuiWindowFlags.NoCollapse|ImGuiWindowFlags.NoResize|ImGuiWindowFlags.NoNav|ImGuiWindowFlags.NoTitleBar);
            ImGui.Text("ajdwiadjiawodjaiod");
            ImGui.Separator();
            ImGui.End();
            ImGui.SetNextWindowSize(new Vector2(window_size.X*0.6f,window_size.Y*0.7f));
            ImGui.SetNextWindowPos(new Vector2(window_size.X*0.2f,0));
            ImGui.Begin("##framebuffer", ImGuiWindowFlags.NoCollapse|ImGuiWindowFlags.NoResize|ImGuiWindowFlags.NoNav|ImGuiWindowFlags.NoTitleBar);
            ImGui.Image(new IntPtr(texture),new Vector2(window_size.X*0.5f,window_size.Y*0.7f));
            ImGui.Separator();
            ImGui.End();

        }

        public override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);
        }

        public override void OnRender()
        {
            base.OnRender();
            ClearFramebuffer();
            RenderScene();
            DisplayCurrentScene();
            
            
        }
    }
}