using System;
using YEET.Engine.Core;

namespace YEET.Engine.ECS
{
    public class Component
    {
        protected Entity Owner { get; }
        public UInt32 CompID;
        private bool Active { get; set; }
        
        public Component(Entity owner)
        {
            Owner = owner;
            CompID = StateMaschine.GenerateId();
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

        public virtual void OnDraw()
        {
        }

        public virtual void OnUpdateAfterDraw()
        {
        }

        public virtual void OnGUI()
        {
            
        }
        
    }
}