using FiniteStateMachine.Utility;

namespace FiniteStateMachine.Core;

public class TransitionBuilder(IState from, StateMachine stateMachine)
{
    private readonly StateMachine _stateMachine = stateMachine;
    private readonly IState _from = from;

    public DestinationBuilder To(IState to)
    {
        return new DestinationBuilder(_from, to, _stateMachine);
    }

    public class DestinationBuilder(IState from, IState to, StateMachine stateMachine)
        : TransitionBuilder(from, stateMachine)
    {
        public TransitionBuilder If(IPredicate condition)
        {
            _stateMachine.At(_from, to, condition);
            return new TransitionBuilder(_from, _stateMachine);
        }
    }
}

public class Transition(IState to, IPredicate condition) : ITransition
{
    public IState To { get; } = to;
    public IPredicate Condition { get; } = condition;
}