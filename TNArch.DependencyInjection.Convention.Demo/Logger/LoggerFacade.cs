using Microsoft.Extensions.Logging;
using TNArch.DependencyInjection.Convention.Abstractions;

namespace TNArch.DependencyInjection.Convention.Demo.Logger
{
    [Dependency(typeof(ILogger<>), DependencyLifeStyle.Singleton)]
    public class LoggerFacade<TLogger> : ILogger<TLogger>
    {
        private readonly Microsoft.Extensions.Logging.ILogger<TLogger> _logger;

        public LoggerFacade(Microsoft.Extensions.Logging.ILogger<TLogger> logger)
        {
            _logger = logger;
        }

        public void LogException(Exception exception, string message = null)
        {
            _logger.LogError(exception, message);
        }

        public void LogInfo(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }
    }
}