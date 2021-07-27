using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImGuiNET;
using YEET.Engine.Core;

namespace YEET.Engine.ECS
{
    public class Entity
    {
        public string Name = "default";

        private Dictionary<Guid, Component> _components;
        private Dictionary<Guid, Entity> _childEntities;
        public Guid ID { get; }
        public bool Active = true;
        public bool ShowGUI = true;
        public bool OpenInInspector = false;
        public Entity(bool GuiVisible = false)
        {
            ShowGUI = GuiVisible;
            _components = new Dictionary<Guid, Component>();
            _childEntities = new Dictionary<Guid, Entity>();
            _components.Add(new Guid(), new Transform(this));
            ID = Guid.NewGuid();
        }

        public virtual void OnStart()
        {
            foreach (var child in _childEntities)
            {
                child.Value.OnStart();
            }
        }

        public virtual void OnUpdate()
        {
            //if(Math.Abs((GetComponent<Transform>().Position-Camera.Position).Length)>Camera.RenderingDistance){
            //  return;  
            //}  
            foreach (var component in _components)
            {
                component.Value.OnUpdate();
            }
            foreach (var child in _childEntities)
            {
                child.Value.OnUpdate();
            }
        }

        public virtual void OnReset()
        {
        }

        public virtual void OnRender()
        {
            
            foreach (var child in _childEntities)
            {
                child.Value.OnRender();
            }
        }

        public virtual void OnUpdateAfterDraw()
        {
            foreach (var child in _childEntities)
            {
                child.Value.OnUpdateAfterDraw();
            }
        }

        public virtual void OnGui()
        {
            int counter=0;
            foreach (var component in _components)
            {
                if (ImGui.TreeNode(component.Value.ToString()+$"##{counter}"))
                {
                    component.Value.OnGUI();
                    ImGui.Separator();
                }
                counter++;
            }
            if (ImGui.Button("Delete"))
            {
                StateMaschine.GetCurrentScene().AddToRemoveList(this.ID);
                StateMaschine.GetCurrentScene().selected = new Entity();
            }
            if (_childEntities.Count > 0)
            {
                ImGui.Separator();
                ImGui.BeginChild("Child Entities");
                ImGui.Text("Child Entities");
                foreach (var child in _childEntities)
                {
                    if (ImGui.Selectable($"{child.Value.ID}"))
                    {
                        StateMaschine.GetCurrentScene().selected = child.Value;
                    }
                }
                ImGui.EndChild();
            }
        }

        /// <summary>
        /// attachement point for the fixed menu boxes on top
        /// add new tabs here if you got some complex stuff
        /// </summary>
        public virtual void OnMenuGui()
        {

        }

        /// <summary>
        /// if the entity contains settings that are relevant for all other it can go into the global scene settings
        /// </summary>
        public virtual void OnMenuSettingsGui()
        {

        }


        public virtual void OnLeave()
        {
            foreach (var child in _childEntities)
            {
                child.Value.OnLeave();

            }

        }

        public Guid AddComponent(Component toadd)
        {
            var id = Guid.NewGuid();
            _components.Add(id, toadd);
            return id;
        }

        public bool RemoveComponent(Guid to_remove)
        {
            return _components.Remove(to_remove);
        }

        public T GetComponent<T>() where T : Component//get a derivative of Component
        {
            foreach (var x in _components)
            {
                if (x.Value is T)
                    return (T)x.Value;
            }
            return null;
        }

        public T GetComponent<T>(Guid to_find) where T : Component//get a derivative of Component
        {
            if (_components.ContainsKey(to_find))
                return (T)_components[to_find];
            return null;
        }

        public List<T> GetComponents<T>() where T : Component
        {
            List<T> to_ret = new List<T>();
            foreach (var component in _components)
            {
                if (component.Value is T)
                {
                    to_ret.Add((T)component.Value);
                }
            }
            return to_ret;
        }


        public Guid AddChildEntity(Entity toAdd)
        {
            var id = Guid.NewGuid();
            _childEntities.Add(id, toAdd);
            return id;
        }


        public T GetChildEntity<T>(Guid to_find) where T : Entity//get a derivative of Component
        {
            if (_childEntities.ContainsKey(to_find))
                return (T)_childEntities[to_find];
            return null;
        }

    }
}