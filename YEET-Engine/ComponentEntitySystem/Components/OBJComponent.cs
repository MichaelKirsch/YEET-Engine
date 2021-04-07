using System.Buffers.Text;

namespace YEET
{
    public class OBJComponent : Component
    {
        public OBJComponent(Entity owner): base(owner)
        {
            Transform _transform = owner.GetComponent<Transform>();
        }
    }
}