

namespace YEET.Engine.ECS{
    /// <summary>
    /// Batched mesh will automaticly batch together all rendered units that have the same shader.
    /// All shaders will automaticly be loaded and then are accessable by name
    /// You can set a maximum batch size. 
    /// </summary>
    public class BatchedMesh : Component{
        public BatchedMesh(Entity owner):base(owner){

        }
        
    }
}