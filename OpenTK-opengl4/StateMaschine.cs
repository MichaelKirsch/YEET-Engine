namespace OpenTK_opengl4
{
    public class State
    {
        public State(StateMaschine parentMaschine)
        {
            _Parent = parentMaschine;
        }

        public virtual void OnUpdate(){}
        public virtual void OnStart(){}
        public virtual void OnLeave(){}
        protected StateMaschine _Parent;
    }
    
    public class StateMaschine
    {
        public StateMaschine()
        {
            
        }
        public bool ShouldClose { get; set; }
        public void Run(State startState)
        {
            _currentState = startState;
            startState.OnStart();
            while (!ShouldClose)
            {
                _currentState.OnUpdate();
            }
        }

        public void SwitchState(State nextState)
        {
            _currentState.OnLeave();
            _currentState = nextState;
            _currentState.OnStart();
        }
        
        private State _currentState;
    }
}