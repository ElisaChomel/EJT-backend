using judo_backend.Attributes;
using judo_backend.Models;
using judo_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace judo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StageController : ApiController
    {
        private IStageService _stageService;
        private IUserService _userService;
        private IEjtAdherentService _ejtAdherentService;
        private IEmailService _emailService;

        public StageController(ILog logger, IStageService stageService, IUserService userService, IEjtAdherentService ejtAdherentService, IEmailService emailService) : base(logger)
        {
            _stageService = stageService;
            _userService = userService;
            _ejtAdherentService = ejtAdherentService;
            _emailService = emailService;
        }

        [HttpGet()]
        [Authorize]
        public ActionResult<List<Stage>> GetAll()
        {
            _logger.LogInformation($"Début - Récupération de la liste des stages");

            var list = this._stageService.GetAll();

            _logger.LogInformation($"Fin - Récupération de la liste des stages");

            return Ok(list);
        }

        [HttpGet("active")]
        [Authorize]
        public ActionResult<List<Stage>> GetAllActive()
        {
            _logger.LogInformation($"Début - Récupération de la liste des stages actifs");

            var list = this._stageService.GetAllActive();

            _logger.LogInformation($"Fin - Récupération de la liste des stages actifs");

            return Ok(list);
        }

        [HttpGet("adherent/{id}")]
        [Authorize]
        public ActionResult<List<int>> GetStagesInscription(int id)
        {
            _logger.LogInformation($"Début - Récupération de la liste des stages ou un adhérent est inscrit");

            var list = this._stageService.GetStagesInscription(id);

            _logger.LogInformation($"Fin - Récupération de la liste des stages ou un adhérent est inscrit");

            return Ok(list);
        }

        [HttpGet("{id}/adherents")]
        [Authorize]
        public ActionResult<List<EjtAdherent>> GetAdherentsInscription(int id)
        {
            _logger.LogInformation($"Début - Récupération de la liste des adhérent inscrit au stage");

            var list = this._stageService.GetAdherentsInscription(id);

            _logger.LogInformation($"Fin - Récupération de la liste des adhérent inscrit au stage");

            return Ok(list);
        }

        [HttpPost()]
        [Authorize]
        public ActionResult<Stage> Create(Stage s)
        {
            _logger.LogInformation($"Début - Création d'un stage");

            Stage ret = this._stageService.Create(s);

            _logger.LogInformation($"Fin - Création d'un stage");

            return Ok(ret);
        }

        [HttpPost("{id}/adherent/{adherentId}")]
        [Authorize]
        public ActionResult CreateStageInscription(int id, int adherentId)
        {
            _logger.LogInformation($"Début - Ajout d'un adhérent à un stage");

            Stage stage = this._stageService.Get(id);

            if (stage == null)
            {
                return Conflict();
            }

            this._stageService.CreateStageInscription(adherentId, id);

            var users = this._userService.GetByAdherentId(adherentId);

            var adherent = this._ejtAdherentService.Get(adherentId);

            _logger.LogInformation($"Fin - Ajout d'un adhérent à un stage");

            _logger.LogInformation($"Début - Envoi d'un email pour informer de l'inscription au stage");

            foreach (var user in users)
            {
                this._emailService.SendStageRegistred(user.Email, user.Username, $"{adherent.Firstname} {adherent.Lastname}", stage.Name, stage.Start.ToString("dd/MM/yyyy"), stage.End.ToString("dd/MM/yyyy"));
            }

            _logger.LogInformation($"Fin - Envoi d'un email pour informer de l'inscription au stage");

            return Ok();
        }


        [HttpPut()]
        [Authorize]
        public ActionResult<Stage> Update(Stage s)
        {
            _logger.LogInformation($"Début - Mise à jour d'un stage");

            Stage ret = this._stageService.Update(s);

            _logger.LogInformation($"Fin - Mise à jour d'un stage");

            return Ok(ret);
        }
    }
}
