using judo_backend.Services.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace judo_backend.Controllers
{
    /// <summary>
    /// Controlleur pour la gestion d'intégrité
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ApiController
    {
        /// <summary>
        /// Service pour le controle d'intégrité.
        /// </summary>
        private readonly HealthCheckService _service;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="HealthCheckController"/>
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="service">Service pour le controle d'intégrité.</param>
        public HealthCheckController(ILog logger, HealthCheckService service) : base(logger)
        {
            _service = service;
        }

        /// <summary>
        /// Recupère le statut du service
        /// </summary>
        /// <returns>Retourne l"état du controle.</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Début - Vérification de l'état des services");

            var report = await _service.CheckHealthAsync();

            string json = System.Text.Json.JsonSerializer.Serialize(report);

            _logger.LogInformation("Fin - Vérification de l'état des services");

            return Ok(json);
        }
    }
}
