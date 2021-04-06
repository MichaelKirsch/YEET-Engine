using OpenTK.Mathematics;

namespace YEET.ComponentEntitySystem.Entities
{
    public class StaticOBJModel : Entity
    {
        private OBJLoader _loader;
        
        public StaticOBJModel(string path,Vector3 position)
        {
            _loader = new OBJLoader(path, new ShaderLoader("Model", "FlatShadedModelVert",
                "FlatShadedModelFrag", true));
            GetComponent<Transform>().SetPosition(position);
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override void OnDraw()
        {
            base.OnDraw();
            _loader.Position = GetComponent<Transform>().GetPosition();
            _loader.Draw();
        }
    }
}