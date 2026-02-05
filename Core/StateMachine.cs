using System;

namespace FootballSim.Core
{
    public interface IState
    {
        void Enter();
        void Exit();
        void Tick(float deltaTime);
        void FixedTick(float fixedDeltaTime);
    }

    /// <summary>
    /// Basit ama genişletilebilir state machine.
    /// AI ve oyuncu davranışları için kullanılabilir.
    /// </summary>
    public sealed class StateMachine
    {
        public IState CurrentState { get; private set; }

        public void ChangeState(IState newState)
        {
            if (newState == null) throw new ArgumentNullException(nameof(newState));
            if (newState == CurrentState) return;

            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }

        public void Tick(float deltaTime)
        {
            CurrentState?.Tick(deltaTime);
        }

        public void FixedTick(float fixedDeltaTime)
        {
            CurrentState?.FixedTick(fixedDeltaTime);
        }
    }
}
