using FiniteStateMachine.Utility;

namespace FiniteStateMachine.Core;

public class TransitionBuilder
{
    private readonly StateMachine _stateMachine;
    private readonly IState _from;

    public TransitionBuilder(IState from, StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        _from = from;
    }

    public DestinationBuilder To(IState to)
    {
        return new DestinationBuilder(_from, to, _stateMachine);
    }

    public class DestinationBuilder : TransitionBuilder
    {
        private readonly IState _to;

        public DestinationBuilder(IState from, IState to, StateMachine stateMachine) : base(from, stateMachine)
        {
            _to = to;
        }

        public TransitionBuilder If(IPredicate condition)
        {
            _stateMachine.At(_from, _to, condition);
            return new TransitionBuilder(_from, _stateMachine);
        }
    }
}

public class Transition : ITransition
{
    public Transition(IState to, IPredicate condition)
    {
        To = to;
        Condition = condition;
    }

    public IState To { get; }
    public IPredicate Condition { get; }
}