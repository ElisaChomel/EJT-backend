using judo_backend.Services.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace judo_backend.Helpers.HealtCheck
{
    /// <summary>
    /// Classe pour les controles d'intégrité
    /// </summary>
    public class ApiHealthCheck : IHealthCheck
    {
        /// <summary>
        /// Service de log;
        /// </summary>
        public ILog _logger;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ApiHealthCheck"/>
        /// </summary>
        /// <param name="logger">Logger</param>
        public ApiHealthCheck(ILog logger) 
        { 
            _logger = logger;
        }

        /// <summary>
        /// Exécute la vérification de l'état. 
        /// </summary>
        /// <param name="context">Object contextuel associé à l'execution en cours.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>Token pour l'annulation.</param>
        /// <returns>A <see cref="Task{HealthCheckResult}"/>Retourne l'état du composant.</returns>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Début - Vérification du status de l'api");
           
            try
            {
                _logger.LogInformation("L'api est en ligne");
                _logger.LogInformation("Fin - Vérification du status de l'api");
                return Task.FromResult(new HealthCheckResult(status: HealthStatus.Healthy, description: "The API is up and running."));
            }
            catch (Exception)
            {
                _logger.LogInformation("L'api n'est pas en ligne");
                _logger.LogInformation("Fin - Vérification du status de l'api");
                return Task.FromResult(new HealthCheckResult(status: HealthStatus.Unhealthy, description: "The API is down."));
            }

        }
    }
}
