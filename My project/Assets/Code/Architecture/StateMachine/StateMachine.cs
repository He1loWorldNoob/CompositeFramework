using System;
using System.Collections.Generic;

namespace Code.Infrastructure.StateMachine
{
    public class StateMachine
    {
        private readonly Dictionary<Type, IExitableState> _states;
        private IExitableState _activeState;

        public StateMachine(StateFactory stateFactory)
        {
            _states = new Dictionary<Type, IExitableState>
            {
                [typeof(BootStrapState)] = stateFactory.CreateState<BootStrapState>(this),
                [typeof(TestState)] = stateFactory.CreateState<TestState>(this)
                
            };
        }
        
        public void Enter<TState>() where TState : class, IState
        {
            IState state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayLoad>(TPayLoad payload) where TState :class, IPayloadedState<TPayLoad>
        {
            TState state = ChangeState<TState>();
            state.Enter(payload);
        }


        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();

            TState state = GetState<TState>();
            _activeState = state;

            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState
        {
            return _states[typeof(TState)] as TState;
        }
        public void Update()
        {
            if(_activeState is IUpdateble state)
                state.OnUpdate();
        }
    }
}
