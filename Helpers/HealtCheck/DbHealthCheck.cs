using judo_backend.Services.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MySqlConnector;
using System.Data;

namespace judo_backend.Helpers.HealtCheck
{
    /// <summary>
    /// Classe pour les controles d'intégrité de la base de donnée
    /// </summary>
    public class DbHealthCheck : IHealthCheck
    {
        /// <summary>
        /// Information pour la chaine de connexion.
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// Service de log;
        /// </summary>
        public ILog _logger;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="DbHealthCheck"/>
        /// </summary>
        /// <param name="configuration">Information de la config.</param>
        /// <param name="logger">Logger</param>
        public DbHealthCheck(IConfiguration configuration, ILog logger)
        {
            _connectionString = configuration.GetSection("ConnectionString").Value;
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
            _logger.LogInformation("Début - Vérification du status de la base de donnée");

            try
            {
                using (var mysqlconnection = new MySqlConnection(_connectionString))
                {                    
                    if (mysqlconnection.State != ConnectionState.Open)
                    {
                        _logger.LogInformation("Tentative de connexion à la base de données");
                        mysqlconnection.Open();
                        _logger.LogInformation("Succès de la tentative de connexion à la base de données");
                    }

                    if (mysqlconnection.State == ConnectionState.Open)
                    {
                        _logger.LogInformation("Tentative de déconnexion à la base de données");
                        mysqlconnection.Close();
                        _logger.LogInformation("Succès de la tentative de déconnexion à la base de données");

                        _logger.LogInformation("Fin - Vérification du status de la base de donnée");
                        return Task.FromResult(HealthCheckResult.Healthy("The database is up and running."));
                    }
                }

                _logger.LogInformation("Fin - Vérification du status de la base de donnée");
                return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, "The database is down."));
            }
            catch (Exception e)
            {
                _logger.LogError($"Echec de la vérification du status de la base de donnée. {e}");
                _logger.LogInformation("Fin - Vérification du status de la base de donnée");
                return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, "The database is down."));
            }
        }
    }
}
