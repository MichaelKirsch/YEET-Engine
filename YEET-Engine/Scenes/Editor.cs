using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Reflection;
using YEET.Engine.Core;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using YEET.Engine.ECS;
using Vector2 = System.Numerics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace YEET
{
    public class Editor : Scene
    {
        private bool new_prefab_menu=false;
        public Editor()
        {
            Buffer<float> b = new Buffer<float>(BufferTarget.ArrayBuffer);
            b.BufferData(new List<float>());
        }

        public override void OnStart()
        {
            base.OnStart();
            AddEntity(new Grid(new Vector2i(3000, 3000),0.02f));
            Camera.Position = new Vector3(1500, 10, 1500);
            AddEntity(new Skybox());
            
            
        }

        public override void OnGui()
        {
            base.OnGui();
            var window_size = StateMaschine.Context.Size;
            //inspector
            var inspectorwidth = 0.2f;

            
            //inspector window    
            ImGui.SetNextWindowSize(new Vector2(window_size.X*inspectorwidth,window_size.Y*0.6f));
            ImGui.SetNextWindowPos(new Vector2((1f-inspectorwidth)*window_size.X,0));
            ImGui.Begin("inspector", ImGuiWindowFlags.NoCollapse|ImGuiWindowFlags.NoResize|ImGuiWindowFlags.NoNav|ImGuiWindowFlags.NoTitleBar);
            ImGui.Text("Inspector");
            ImGui.Separator();
            ImGui.BeginGroup();
            ImGui.BeginChild($"Selected Entity:{selected.ID}",
                new System.Numerics.Vector2(0, -ImGui.GetFrameHeightWithSpacing()));
            ImGui.Text($"Type:{selected.Name} ID:{selected.ID}");
            ImGui.Separator();
            selected.OnGui();
            ImGui.EndChild();
            ImGui.EndGroup();
            ImGui.End();
            
            
            //lower left
            ImGui.SetNextWindowSize(new Vector2(window_size.X*0.5f,window_size.Y*0.4f));
            ImGui.SetNextWindowPos(new Vector2(0f,window_size.Y*0.6f));
            ImGui.Begin("##rr", ImGuiWindowFlags.NoCollapse|ImGuiWindowFlags.NoResize|ImGuiWindowFlags.NoNav|ImGuiWindowFlags.NoTitleBar);
            ImGui.Text("Low Left");
            ImGui.Separator();
            ImGui.End();
            
            
            //lower right
            ImGui.SetNextWindowSize(new Vector2(window_size.X*0.5f,window_size.Y*0.4f));
            ImGui.SetNextWindowPos(new Vector2(window_size.X*0.5f,window_size.Y*0.6f));
            ImGui.Begin("Entities", ImGuiWindowFlags.NoCollapse|ImGuiWindowFlags.NoResize|ImGuiWindowFlags.NoNav|ImGuiWindowFlags.NoTitleBar);
            ImGui.End();
            
            //upper left window
            
            ImGui.SetNextWindowSize(new Vector2(window_size.X*0.2f,window_size.Y*0.6f));
            ImGui.SetNextWindowPos(new Vector2(0f,0));
            ImGui.Begin("##r3r", 
                ImGuiWindowFlags.NoCollapse|
                ImGuiWindowFlags.NoResize|
                ImGuiWindowFlags.NoNav|
                ImGuiWindowFlags.NoTitleBar);
            ImGui.Text("Scene Overview");
            ImGui.Separator();
            
            foreach (var entity in Entities)
            {
                if (ImGui.Selectable($"{entity.Name}##{entity.ID}"))
                {
                    selected = entity;
                }
            }
            
            if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                new_prefab_menu = true;
                //this is some really weird code but it actually works
                //if scanns the directory for class names and spawns an instance 
                //object needs to have a empty constructor
            }
            ImGui.End();

            if (new_prefab_menu)
            {
                ImGui.Begin("Add_new_prefab");
                foreach (var VARIABLE in Directory.GetFiles("Engine/ComponentEntitySystem/Prefabs"))
                {
                    string name = VARIABLE.Substring("Engine/ComponentEntitySystem/Prefabs".Length + 1,
                        VARIABLE.Length - "Engine/ComponentEntitySystem/Prefabs".Length - 4);
                    if (ImGui.Button(name))
                    {
                        var full = "YEET.Engine.ECS." + name;
                        Type t = Type.GetType(full);
                        Console.WriteLine(t);
                        var x = AddEntity((Entity)Activator.CreateInstance(t));
                        GetEntity(x).GetComponent<Transform>().Position = Camera.Position+Camera.Front*2;
                        new_prefab_menu = false;
                    }
                }
                ImGui.End();
            }
            //Preview Window
            ImGui.SetNextWindowSize(new Vector2(window_size.X*0.6f,window_size.Y*0.6f));
            ImGui.SetNextWindowPos(new Vector2(window_size.X*0.2f,0));
            ImGui.Begin("##framebuffer", 
                ImGuiWindowFlags.NoCollapse|
                ImGuiWindowFlags.NoResize|
                ImGuiWindowFlags.NoNav|
                ImGuiWindowFlags.NoTitleBar|
                ImGuiWindowFlags.NoScrollbar|
                ImGuiWindowFlags.NoMouseInputs);
            ImGui.Image(new IntPtr(texture),new Vector2(window_size.X*0.6f,window_size.Y*0.6f),new Vector2(1,1),new Vector2(0,0));
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