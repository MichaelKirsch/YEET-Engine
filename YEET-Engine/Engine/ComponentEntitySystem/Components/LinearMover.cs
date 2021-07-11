using System.Net;
using OpenTK.Graphics.ES20;
using OpenTK.Mathematics;


namespace YEET.Engine.ECS
{
    public class LinearMover : Component
    {
        private bool _Direction=true;
        private Vector3 _DirectionVector;
        private float _LengthVector,_TraveledDistance;
        public float Speed;
        private Vector3 _Endpoint, _Startpoint;
        
        public LinearMover(Entity owner, Vector3 startpoint, Vector3 entpoint, float speed) : base(owner)
        {
            _Startpoint = startpoint;
            _Endpoint = entpoint;
            Speed = speed;
            Reset();
        }

        public override void OnUpdate()
        {
            if (_Direction)
            {
                Owner.GetComponent<Transform>().SetPosition(Owner.GetComponent<Transform>().GetPosition()+_DirectionVector*Speed);
                _TraveledDistance = new Vector3(Owner.GetComponent<Transform>().GetPosition()-_Startpoint).Length;
                if (_TraveledDistance > _LengthVector)
                    Reset();
            }
            else
            {
                Owner.GetComponent<Transform>().SetPosition(Owner.GetComponent<Transform>().GetPosition()-_DirectionVector*Speed);
                _TraveledDistance = new Vector3(Owner.GetComponent<Transform>().GetPosition()-_Startpoint).Length;
                if (_TraveledDistance <=0.1f) //this is shit but it works for now
                    _Direction = !_Direction;
            }
            base.OnUpdate();
        }

        public void SetStartpoint(Vector3 newstart)
        {
            _Startpoint = newstart;
            Reset();
        }
        
        public void SetEndpoint(Vector3 newstart)
        {
            _Endpoint = newstart;
            Reset();
        }
        
        public void SetSpeed(float speed)
        {
            Speed = speed;
        }
        
        
        private void Reset()
        {
            Owner.GetComponent<Transform>().SetPosition(_Startpoint);
            var x = new Vector3(_Endpoint - _Startpoint);
            _LengthVector = x.Length;
            _DirectionVector = x.Normalized();
            _Direction = true;
        }
        
        
    }
}