namespace YEET
{
    public class Component
    {
        private Entity Owner { get; }

        private bool Active { get; set; }
        
        public Component(Entity owner)
        {
            Owner = owner;
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
        
        
    }
}