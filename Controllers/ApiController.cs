using judo_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace judo_backend.Controllers
{
    public class ApiController : ControllerBase
    {
        /// <summary>
        /// Service de log;
        /// </summary>
        public ILog _logger;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ApiController"/>
        /// </summary>
        public ApiController(ILog logger)
        {
            _logger = logger;
        }
    }
}
