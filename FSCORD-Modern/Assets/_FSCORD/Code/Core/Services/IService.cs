namespace FSCORD.Core
{
    /// <summary>A long-lived system created once at boot and shut down on teardown.</summary>
    public interface IService
    {
        void Initialize();
        void Shutdown();
    }
}
