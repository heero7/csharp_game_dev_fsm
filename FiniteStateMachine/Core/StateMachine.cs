namespace FiniteStateMachine.Core;

public class StateMachine
{
    public IState Current => _current.State;
    public IState Previous => _previous.State;

    private readonly Dictionary<Type, StateNode> _stateNodes = new();
    private readonly HashSet<ITransition> _anyTransitions = new();
    
    private StateNode _current;
    private StateNode _previous;

    public void Update()
    {
        var transition = GetTransition();

        if (transition != null)
        {
            ChangeState(transition.To);
        }
        
        _current.State.OnUpdate();
    }

    public void SetState(IState state)
    {
        _current = _stateNodes[state.GetType()];
        _current.State.OnEnter();
    }
    
    public void SetInitialState(IState state)
    {
        _current = _stateNodes[state.GetType()];
        _previous ??= _current;
        _current.State.OnEnter();
    }

    private void ChangeState(IState state)
    {
        if (state == _current.State)
        {
            return;
        }

        var nextStateNode = _stateNodes[state.GetType()];

        var previousStateNode = _current;
        var previousState = _current.State;
        var nextState = nextStateNode.State;
        
        previousState.OnExit();
        nextState.OnEnter();

        _current = nextStateNode;
        _previous = previousStateNode;
    }

    public void AddTransition(IState from, IState to, IPredicate condition)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    }

    public void AddAnyTransition(IState to, IPredicate condition)
    {
        _anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
    }
    
    private ITransition GetTransition()
    {
        foreach (var anyTransition in _anyTransitions)
        {
            if (anyTransition.Condition.Evaluate())
                return anyTransition;
        }

        foreach (var transition in _current.Transitions)
        {
            if (transition.Condition.Evaluate())
                return transition;
        }

        return null;
    }

    private StateNode GetOrAddNode(IState state)
    {
        var node = _stateNodes.GetValueOrDefault(state.GetType());

        if (node != null)
        {
            return node;
        }

        node = new StateNode(state);
        _stateNodes.Add(state.GetType(), node);

        return node;
    }

    private class StateNode
    {
        public StateNode(IState state)
        {
            State = state;
        }

        public IState State { get; }
        public HashSet<ITransition> Transitions { get; } = new();

        public void AddTransition(IState to, IPredicate condition) => Transitions.Add(new Transition(to, condition));
    }
}