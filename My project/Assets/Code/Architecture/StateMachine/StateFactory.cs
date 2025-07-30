using Code.Infrastructure.StateMachine;

public class StateFactory
{
    private readonly DiService _di;

    public StateFactory(DiService di)
    {
        _di = di;
    }

    public TState CreateState<TState>() where TState : IExitableState
    {
        return _di.Container.Instantiate<TState>();
    }
      
    public TState CreateState<TState>(params object[] args) where TState : IExitableState
    {
        return _di.Container.Instantiate<TState>(args);
    }
}