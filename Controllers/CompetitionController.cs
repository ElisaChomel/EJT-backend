using judo_backend.Attributes;
using judo_backend.Models;
using judo_backend.Services;
using judo_backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace judo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompetitionController : ApiController
    {
        private ICompetitionService _competitionService;
        private IUserService _userService;
        private IEmailService _emailService;
        private IExcelGeneratorService _excelService;

        public CompetitionController(
            ILog logger,
            ICompetitionService competitionService,
            IUserService userService,
            IEmailService emailService,
            IExcelGeneratorService excelService) : base(logger)
        {
            _competitionService = competitionService;
            _userService = userService;
            _emailService = emailService;
            _excelService = excelService;
        }

        [HttpGet()]
        [Authorize]
        public ActionResult<List<Competition>> GetAll()
        {
            _logger.LogInformation($"Début - Récupération de la liste des compétitions");

            var list = this._competitionService.GetAll();

            _logger.LogInformation($"Fin - Récupération de la liste des compétitions");

            return Ok(list);
        }

        [HttpGet("active")]
        [Authorize]
        public ActionResult<List<Competition>> GetAllActive()
        {
            _logger.LogInformation($"Début - Récupération de la liste des compétitions actives");

            var list = this._competitionService.GetAllActive();

            _logger.LogInformation($"Fin - Récupération de la liste des compétitions actives");

            return Ok(list);
        }

        [HttpGet("adherent/{id}")]
        [Authorize]
        public ActionResult<List<int>> GetCompetitionsInscription(int id)
        {
            _logger.LogInformation($"Début - Récupération de la liste des compétitions ou un adhérent est inscrit");

            var list = this._competitionService.GetCompetitionsInscription(id);

            _logger.LogInformation($"Fin - Récupération de la liste des compétitions ou un adhérent est inscrit");

            return Ok(list);
        }

        [HttpGet("{id}/result")]
        [Authorize]
        public ActionResult<List<CompetitionResult>> GetResults(int id)
        {
            _logger.LogInformation($"Début - Récupération de la liste des resultats d'une compétition");

            var list = this._competitionService.GetResults(id);

            _logger.LogInformation($"Fin - Récupération de la liste des resultats d'une compétition");

            return Ok(list);
        }

        [HttpPost()]
        [Authorize]
        public ActionResult<Competition> Create(Competition c)
        {
            _logger.LogInformation($"Début - Création d'une compétition");

            Competition ret = this._competitionService.Create(c);

            _logger.LogInformation($"Fin - Création d'une compétition");

            return Ok(ret);
        }

        [HttpPost("{id}/result")]
        [Authorize]
        public ActionResult<CompetitionResult> CreateResult(int id, CompetitionResult c)
        {
            _logger.LogInformation($"Début - Ajout d'un adhérent à une compétition");

            if (id != c.Competition_ID)
            {
                return Conflict();
            }

            Competition competition = this._competitionService.Get(c.Competition_ID);

            if (competition == null)
            {
                return Conflict();
            }

            CompetitionResult ret = this._competitionService.CreateResult(c);

            var users = this._userService.GetByAdherentId(ret.Adherent_ID);

            _logger.LogInformation($"Fin - Ajout d'un adhérent à une compétition");

            _logger.LogInformation($"Début - Envoi d'un email pour informer de l'inscription");

            foreach (var user in users)
            {
                this._emailService.SendCompetitionRegistred(user.Email, user.Username, $"{ret.Firstname} {ret.Name}", competition.Name);
            }

            _logger.LogInformation($"Fin - Envoi d'un email pour informer de l'inscription");

            return Ok(ret);
        }

        [HttpPut()]
        [Authorize]
        public ActionResult<Competition> Update(Competition c)
        {
            _logger.LogInformation($"Début - Mise à jour d'une compétition");

            Competition ret = this._competitionService.Update(c);

            _logger.LogInformation($"Fin - Mise à jour d'une compétition");

            return Ok(ret);
        }

        [HttpPut("{id}/result")]
        [Authorize]
        public ActionResult<CompetitionResult> UpdateResult(CompetitionResult c)
        {
            _logger.LogInformation($"Début - Mise à jour d'un résultat d'une compétition");

            CompetitionResult ret = this._competitionService.UpdateResult(c);

            _logger.LogInformation($"Début - Mise à jour d'un résultat d'une compétition");

            return Ok(ret);
        }

        [HttpGet("{id}/export")]
        [Authorize]
        public ActionResult GetAllInscription(int id)
        {
            _logger.LogInformation($"Début - Export des inscripts pour la compétition");

            var competition = this._competitionService.Get(id);

            var list = this._competitionService.GetInscription(id);

            var workbook = _excelService.GenerateEjtAdherentList(competition.Name, list);
            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Seek(0, SeekOrigin.Begin);            

            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, bytes.Length);

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var donwloadFile = string.Format("{0}.xlsx", Guid.NewGuid().ToString());

            _logger.LogInformation($"Fin - Export des inscripts pour la compétition");

            return File(bytes, contentType, donwloadFile);
        }

    }
}
