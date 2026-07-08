namespace FSCORD.Core
{
    /// <summary>
    /// Tiny finite state machine. Match flow uses this instead of the dozens of
    /// interdependent boolean flags that lived on the 2015 Game.cs.
    /// </summary>
    public sealed class StateMachine
    {
        public IState Current { get; private set; }

        public void Change(IState next)
        {
            Current?.Exit();
            Current = next;
            Current?.Enter();
        }

        public void Tick(float deltaTime) => Current?.Tick(deltaTime);
    }
}
