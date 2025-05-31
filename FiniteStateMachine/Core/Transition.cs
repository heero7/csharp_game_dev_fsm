namespace FiniteStateMachine.Core;

public class Transition(IState to, IPredicate condition) : ITransition
{
    public IState To { get; } = to;
    public IPredicate Condition { get; } = condition;
}