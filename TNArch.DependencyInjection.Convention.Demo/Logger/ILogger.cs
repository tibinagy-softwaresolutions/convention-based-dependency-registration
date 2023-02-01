namespace TNArch.DependencyInjection.Convention.Demo.Logger
{
    public interface ILogger<TLogger>
    {
        void LogException(Exception exception, string message = null);
        void LogInfo(string message);
        void LogWarning(string message);
    }
}