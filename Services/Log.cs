using judo_backend.Services.Interfaces;

namespace judo_backend.Services
{
    public class Log : ILog
    {
        /// <summary>
        /// Service de log;
        /// </summary>
        ILogger<Log> _logger;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="HealthCheckController"/>
        /// </summary>
        /// <param name="service">Service pour le controle d'intégrité.</param>
        public Log(ILogger<Log> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Affichage d'un log d'erreur
        /// </summary>
        /// <param name="message"Message du log></param>
        public void LogError(string message)
        {
            _logger.LogError($"{DateTime.Now.ToString("yy-MM-dd HH:mm:ss")} - {message}");
        }

        /// <summary>
        /// Affichage d'un log d'information
        /// </summary>
        /// <param name="message">Message du log</param>
        public void LogInformation(string message)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("yy-MM-dd HH:mm:ss")} - {message}");
        }


    }
}
