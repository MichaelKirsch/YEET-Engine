using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace YEET
{
    public static class StateMaschine
    {
        private static long _startTime, _startTicks;
        private static Stopwatch _stopwatch;
        public static int framebuffer, texture, depth_texture, stmVAO,stmVBO;

        static float[] rectangleVertices =
            {
            	// Coords    // texCoords
            	 1.0f, -1.0f,  1.0f, 0.0f,
                -1.0f, -1.0f,  0.0f, 0.0f,
                -1.0f,  1.0f,  0.0f, 1.0f,

                 1.0f,  1.0f,  1.0f, 1.0f,
                 1.0f, -1.0f,  1.0f, 0.0f,
                -1.0f,  1.0f,  0.0f, 1.0f
            };

        static ShaderLoader stmLoader;




        static StateMaschine()
        {

            Console.WriteLine("StateMaschine Started");
            _stopwatch = Stopwatch.StartNew();
            _stopwatch.Start();
            _startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            _startTicks = DateTime.Now.Ticks;
        }
        /// <summary>
        /// 
        /// </summary>
        private class Startupstate : Scene
        {
            public Startupstate(Scene newScene)
            {
                _startScene = newScene;
            }

            public override void OnUpdate(FrameEventArgs e)
            {
                Console.WriteLine("Startup created. Will switch to First State now");
                base.OnUpdate(e);
                StateMaschine.SwitchState(_startScene);
            }

            private Scene _startScene;
        }

        public static double GetElapsedTimeTicks()
        {
            return DateTime.Now.Ticks - _startTicks;
        }

        public static double GetElapsedTime()
        {
            return (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - _startTime;
        }
        public static void Run(Scene start, int updateRate, int frameRate)
        {
            _currentScene = new Startupstate(start);
            Context = new MainWindow(updateRate, frameRate);
            framebuffer = GL.GenFramebuffer();
            texture = GL.GenTexture();
            depth_texture = GL.GenTexture();
            stmVAO = GL.GenVertexArray();
            stmVBO = GL.GenBuffer();
            stmLoader = new ShaderLoader("default");
            GL.BindVertexArray(stmVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer,stmVBO);
            GL.BufferData(BufferTarget.ArrayBuffer,rectangleVertices.Length*sizeof(float),rectangleVertices,BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0,2,VertexAttribPointerType.Float,false,4*sizeof(float),new IntPtr(0));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1,2,VertexAttribPointerType.Float,false,4*sizeof(float), new IntPtr(2*sizeof(float)));
            Context.Run();
        }

        public static void Render()
        {
            var ts = new TimeSlot("Main Render");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit); // we're not using the stencil buffer now

            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Context.Size.X, Context.Size.Y, 0, PixelFormat.Rgb,
                PixelType.UnsignedByte, new System.IntPtr());
            GL.TextureParameter(texture, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
            GL.TextureParameter(texture, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TextureParameter(texture, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TextureParameter(texture, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, texture, 0);

            GL.BindTexture(TextureTarget.Texture2D, depth_texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent,  Context.Size.X, Context.Size.Y, 0, PixelFormat.DepthComponent, PixelType.Float, new System.IntPtr());
            GL.TextureParameter(depth_texture, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
            GL.TextureParameter(depth_texture, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TextureParameter(depth_texture, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TextureParameter(depth_texture, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TextureParameter(depth_texture, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.None);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                TextureTarget.Texture2D, depth_texture, 0);
            //DrawBuffersEnum[] drawBuffersEnum = {DrawBuffersEnum.ColorAttachment0};
            //GL.DrawBuffers(1, drawBuffersEnum);
            _currentScene.OnRender();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            stmLoader.UseShader();
            GL.Disable(EnableCap.DepthTest);
            GL.BindVertexArray(stmVAO);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D,texture);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D,depth_texture);
            stmLoader.SetUniformInt("colorTexture",0);
            stmLoader.SetUniformInt("depthTexture",1);
            GL.DrawArrays(PrimitiveType.Triangles,0,6);

            GL.Enable(EnableCap.DepthTest);
            ts.Stop();
            var guits = new TimeSlot("Gui Render");
            _currentScene.OnGui();
            Sun.OnGui();
            guits.Stop();
            Profiler.StopFrame();
            Profiler.RenderProfilerWindow();
        }

        public static void Input()
        {
            Profiler.StartFrame();
            var ts = new TimeSlot("Input");
            _currentScene.OnInput();
            ts.Stop();
        }

        public static void Update(FrameEventArgs e)
        {
            var ts = new TimeSlot("Update");
            _currentScene.OnUpdate(e);
            ts.Stop();
        }

        public static void SwitchState(Scene nextScene)
        {
            _currentScene.OnLeave();
            _currentScene = nextScene;
            _currentScene.OnStart();
        }

        public static void Exit()
        {
            _currentScene.OnLeave();
            Context.Close();
            Context.Dispose();
        }

        public static Scene GetCurrentScene()
        {
            return _currentScene;
        }

        private static Scene _currentScene;
        public static MainWindow Context;
    }

    public class Scene
    {
        public Entity selected = new Entity();
        private Queue<Guid> removeList = new Queue<Guid>();
        private List<Entity> Entities = new List<Entity>();
        public Vector4 ClearColor = new Vector4(0.415f, 0.439f, 0.4f, 1.0f);
        public int EntitiesCount()
        {
            return Entities.Count;
        }
        public Scene()
        {

        }

        public void AddToRemoveList(Guid to_remove)
        {
            removeList.Enqueue(to_remove);
        }

        public void RemoveEntity(Guid id)
        {
            Entities.Remove(Entities.Find(x => x.ID == id));
        }

        public Guid ChangeEntity(Guid id, Entity new_entt)
        {
            Entities.Remove(Entities.Find(x => x.ID == id));
            GC.Collect();
            return AddEntity(new_entt);
        }


        public Guid AddEntity(Entity toadd)
        {
            Entities.Add(toadd);
            return toadd.ID;
        }

        public T GetEntity<T>(Guid tofind) where T : Entity
        {
            foreach (var item in Entities)
            {
                if (item.ID == tofind)
                    return (T)item;
            }

            return null;
        }

        public List<T> GetEntitiesByType<T>() where T : Entity
        {
            List<T> to_ret = new List<T>();
            foreach (var entity in Entities)
            {
                if (entity is T)
                {
                    to_ret.Add((T)entity);
                }
            }
            return to_ret;
        }


        public Entity GetEntity(Guid tofind)
        {
            return (Entities.Find(item => item.ID == tofind));
        }

        /// <summary>
        /// Gets called after the render call. add all Imgui Code in this block
        /// </summary>
        public virtual void OnGui()
        {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(800, 800));
            //ImGui.BeginMenuBar();
            //
            //ImGui.EndMenuBar();        
            //foreach (var entity in Entities)
            //{
            //    entity.OnMenuGui();
            //}

            ImGui.Begin("Entities");
            //ImGui.BeginMenuBar();
            //ImGui.EndMenuBar();

            ImGui.BeginChild("letft_pane", new System.Numerics.Vector2(150, 0), true);
            foreach (var entity in Entities)
            {
                if (ImGui.Selectable($"{entity.Name}##{entity.ID}"))
                {
                    selected = entity;
                };
            }
            ImGui.EndChild();
            ImGui.SameLine();
            ImGui.BeginGroup();
            ImGui.BeginChild($"Selected Entity:{selected.ID}", new System.Numerics.Vector2(0, -ImGui.GetFrameHeightWithSpacing()));
            ImGui.Text($"Type:{selected.Name} ID:{selected.ID}");
            ImGui.Separator();
            selected.OnGui();
            ImGui.EndChild();
            ImGui.EndGroup();

            ImGui.End();
            Camera.OnGui();
        }

        public virtual void OnInput()
        {
        }

        /// <summary>
        /// Add all update functions in this block. It is Time-managed through the opentk game window
        /// </summary>
        public virtual void OnUpdate(FrameEventArgs e)
        {
            //generate ActiveChunks;
            while (removeList.Count > 0)
                RemoveEntity(removeList.Dequeue());
            SpatialManager.OnUpdate();
            GL.ClearColor(ClearColor.X, ClearColor.Y, ClearColor.Z, ClearColor.W);
            foreach (var entity in Entities)
            {
                if (entity.Active)
                    entity.OnUpdate();
            }
        }

        /// <summary>
        /// all Rendercode should be here or you will run into gl.clear problems
        /// </summary>
        public virtual void OnRender()
        {

            foreach (var entity in Entities)
            {
                if (entity.Active)
                    entity.OnRender();
            }
        }

        /// <summary>
        /// gets called once when the state gets loaded
        /// </summary>
        public virtual void OnStart()
        {
            foreach (var entity in Entities)
            {
                entity.OnStart();
            }
        }

        /// <summary>
        /// gets called once when the state gets switched away by the statemaschine
        /// </summary>
        public virtual void OnLeave()
        {
            foreach (var entity in Entities)
            {
                entity.OnLeave();
            }
        }
    }
}