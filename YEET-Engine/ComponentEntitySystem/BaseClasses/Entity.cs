using System;
using System.Collections.Generic;

namespace YEET
{
    public class Entity
    {
        public string Name = "default";

        private List<Component> _components;
        public Guid ID { get; }
        public bool ShowGUI = true;
        public Entity()
        {
            _components = new List<Component>();
            _components.Add(new Transform(this));
            ID = new Guid();
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnReset()
        {
        }

        public virtual void OnRender()
        {
        }

        public virtual void OnUpdateAfterDraw()
        {
        }

        public virtual void OnGui()
        {
            
        }

        public virtual void OnLeave()
        {
            
        }
        
        public void AddComponent(Component toadd)
        {
           _components.Add(toadd); 
        }
        
        
        public bool RemoveComponent<T>()
        {
            foreach (Component component in _components)
            {
                if (component.GetType().Equals(typeof(T)))
                {
                    _components.Remove(component);
                    return true;
                }
            }
            return false;
        }

        public T GetComponent<T>() where T : Component//get a derivative of Component
        {
            foreach (var item in _components)
            {
                if (item is T)
                {
                    return (T)item;
                }
            }
            return null;
        }
        
    }
}