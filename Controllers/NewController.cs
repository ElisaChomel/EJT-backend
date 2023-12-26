using judo_backend.Attributes;
using judo_backend.Models;
using judo_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace judo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewController : ApiController
    {
        private INewService _newService;

        public NewController(ILog logger, INewService newService) : base(logger)
        {
            _newService = newService;
        }

        [HttpGet()]
        public ActionResult<List<New>> GetAll()
        {
            _logger.LogInformation($"Début - Récupération des news");

            var list =  this._newService.GetAll();

            _logger.LogInformation($"Fin - Récupération des news");

            return Ok(list);
        }

        [HttpPost()]
        [Authorize]
        public ActionResult<New> Upload(New n)
        {
            _logger.LogInformation($"Début - Ajout d'une new");

            New ret = this._newService.Upload(n);

            _logger.LogInformation($"Fin - Ajout d'une new");

            return Ok(ret);
        }

        [HttpPut()]
        [Authorize]
        public ActionResult<New> Update(New n)
        {
            _logger.LogInformation($"Début - Mise à jour d'une new");

            this._newService.Update(n);

            _logger.LogInformation($"Fin - Mise à jour d'une new");
            return Ok(n);
        }
    }
}
