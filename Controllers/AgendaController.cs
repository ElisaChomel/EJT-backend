using judo_backend.Attributes;
using judo_backend.Models;
using judo_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace judo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgendaController : ApiController
    {
        private IAgendaService _agendaService;
        private INewService _newService;

        public AgendaController(ILog logger, IAgendaService agendaService, INewService newService) : base(logger)
        {
            _agendaService = agendaService;
            _newService = newService;
        }

        [HttpGet()]
        public ActionResult<List<Agenda>> GetAll()
        {
            _logger.LogInformation($"Début - Récupération des dates de l'agenda");

            var list =  this._agendaService.GetAll();

            _logger.LogInformation($"Fin - Récupération des dates de l'agenda");

            return Ok(list);
        }

        [HttpPost()]
        [Authorize]
        public ActionResult<Agenda> Upload(Agenda a)
        {
            _logger.LogInformation($"Début - Ajout d'une date dans l'agenda");

            Agenda ret = this._agendaService.Upload(a);

            New n = new New()
            {
                Date = ret.Date,
                Title = ret.Title,
                Resume = ret.Resume,
                Detail = ret.Detail,
            };

            this._newService.Upload(n);

            _logger.LogInformation($"Fin - Ajout d'une date dans l'agenda");

            return Ok(ret);
        }

        [HttpPut()]
        [Authorize]
        public ActionResult<Agenda> Update(Agenda a)
        {
            _logger.LogInformation($"Début - Mise à jour d'une date dans l'agenda");

            this._agendaService.Update(a);

            _logger.LogInformation($"Fin - Mise à jour d'une date dans l'agenda");

            return Ok(a);
        }
    }
}
