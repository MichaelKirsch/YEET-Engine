using System;
using System.Collections.Generic;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using Vector4 = System.Numerics.Vector4;
using YEET.Engine.ECS;


namespace YEET.Engine.Core
{
    public class Scene
    {
        public int framebuffer, texture, depth_texture, stmVAO, stmVBO;
        public Entity selected = new Entity();
        private Queue<Guid> removeList = new Queue<Guid>();
        private List<Entity> Entities = new List<Entity>();
        public Vector4 ClearColor = new Vector4(0.415f, 0.439f, 0.4f, 1.0f);
        public ShaderLoader stmLoader;
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
                    return (T) item;
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
                    to_ret.Add((T) entity);
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
                }

                ;
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
            Camera.OnGui();
            Sun.OnGui();
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

            InstanceRenderer.OnUpdate();
        }

        /// <summary>
        /// all Rendercode should be here or you will run into gl.clear problems
        /// </summary>
        public virtual void OnRender()
        {
            ClearFramebuffer();
            foreach (var entity in Entities)
            {
                if (entity.Active)
                    entity.OnRender();
            }

            GL.Enable(EnableCap.CullFace);
            //instancerenderer NEEDS to be after the Entities as they fill it up in their Rendercalls
            InstanceRenderer.OnRender();
            GL.Disable(EnableCap.CullFace);
            DisplayCurrentScene();
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
            GenerateBuffers();
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
        
        private void GenerateBuffers()
        {
            framebuffer = GL.GenFramebuffer();
            texture = GL.GenTexture();
            depth_texture = GL.GenTexture();
            stmVAO = GL.GenVertexArray();
            stmVBO = GL.GenBuffer();
            stmLoader = new ShaderLoader("default");
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
        
        private void ClearFramebuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
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

        private void DisplayCurrentScene()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            stmLoader.UseShader();
            GL.Disable(EnableCap.DepthTest);
            GL.BindVertexArray(stmVAO);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, depth_texture);
            stmLoader.SetUniformInt("colorTexture", 0);
            stmLoader.SetUniformInt("depthTexture", 1);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.Enable(EnableCap.DepthTest);
        }
        
    }
}