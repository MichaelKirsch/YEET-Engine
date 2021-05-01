

namespace YEET
{
    public class AbstractCollider : Component
    {
        public AxisAllignedCube Outline;
        
        public AbstractCollider(Entity owner) : base(owner)
        {
            
        }
        
        
        public virtual bool CheckCollision(AbstractCollider other_collider)
        {
            return Outline.AABB(other_collider.Outline);
        }
        
    }
}