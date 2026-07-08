namespace FSCORD.Core
{
    /// <summary>Implemented by components that need reset hooks when pooled.</summary>
    public interface IPoolable
    {
        void OnSpawned();
        void OnDespawned();
    }
}
