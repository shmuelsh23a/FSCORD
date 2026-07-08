namespace FSCORD.Core
{
    /// <summary>A single state in a StateMachine.</summary>
    public interface IState
    {
        void Enter();
        void Tick(float deltaTime);
        void Exit();
    }
}
