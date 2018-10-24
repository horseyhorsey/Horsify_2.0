using Microsoft.Extensions.Logging;
using Prism.Logging;

namespace Horsesoft.Music.Horsify.Base.Logging
{
    /// <summary>
    /// This logger uses the Microsoft.Logging....with added file provider. <para/>
    /// LoggerFacade is from Prism
    /// </summary>
    /// <seealso cref="Prism.Logging.ILoggerFacade" />
    public class Logger : ILoggerFacade
    {
        private ILogger _logger;

        public Logger(string name = null)
        {
            if (_logger == null)
                CreateLogger(name);        
        }

        private void CreateLogger(string name = null)
        {
            var fileName = string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                fileName = "HorsifyJukebox.log";
            }
            else
            {
                fileName = $"{name}.log";
            }
#if DEBUG
                ILoggerFactory logFactory = new LoggerFactory()
                .AddConsole(LogLevel.Trace)
                .AddFile(@"C:\ProgramData\Horsify\Logs\" + fileName, LogLevel.Debug);
#else
            ILoggerFactory logFactory = new LoggerFactory()
                .AddFile(@"C:\ProgramData\Horsify\Logs\" + fileName, (int)LogLevel.Warn);
#endif
            //Create the logger from incoming name.
            if (!string.IsNullOrWhiteSpace(name))
                _logger = logFactory.CreateLogger(name);
            else
                _logger = logFactory.CreateLogger("Horsify Base Logger");
        }

        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:                    
                    _logger?.LogDebug($"{message}", category, priority);
                    break;
                case Category.Exception:
                    _logger?.LogError($"{message}", category, priority);
                    break;
                case Category.Info:
                    _logger?.LogInformation($"{message}", category, priority);
                    break;
                case Category.Warn:
                    _logger?.LogWarning($"{message}", (int)category, priority);
                    break;
                default:
                    break;
            }            
        }
    }
}
