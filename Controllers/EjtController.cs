using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using judo_backend.Attributes;
using judo_backend.Models;
using judo_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace judo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EjtController : ApiController
    {
        private IEjtPersonService _ejtPersonService;
        private IEjtAdherentService _ejtAdherentService;

        public EjtController(ILog logger, IEjtPersonService ejtPersonService, IEjtAdherentService ejtAdherentService) : base(logger)
        {
            _ejtPersonService = ejtPersonService;
            _ejtAdherentService = ejtAdherentService;
        }

        [HttpGet("Person")]
        public ActionResult<List<EjtPerson>> GetAllPerson()
        {
            _logger.LogInformation($"Début - Récupération de la liste des membres du bureau et des profs");

            var persons = this._ejtPersonService.GetAll();

            _logger.LogInformation($"Fin - Récupération de la liste des membres du bureau et des profs");

            return Ok(persons);
        }

        [HttpPost("Person")]
        [Authorize]
        public ActionResult<EjtPerson> CreatePerson(EjtPerson model)
        {
            _logger.LogInformation($"Début - Création d'un membres du bureau ou des profs");

            var person = this._ejtPersonService.Create(model); 
            
            _logger.LogInformation($"Fin - Création d'un membres du bureau ou des profs");

            return Ok(person);
        }

        [HttpPut("Person")]
        [Authorize]
        public ActionResult<EjtPerson> UpdatePerson(EjtPerson model)
        {
            _logger.LogInformation($"Début - Mise à jour d'un membres du bureau ou des profs");

            var person = this._ejtPersonService.Update(model);

            _logger.LogInformation($"Fin - Mise à jour d'un membres du bureau ou des profs");

            return Ok(person);
        }

        [HttpGet("Adherent")]
        public ActionResult<List<EjtAdherent>> GetAllAdhenrent()
        {
            _logger.LogInformation($"Début - Récupération de la liste des adhérents");

            var adherents = this._ejtAdherentService.GetAll();

            _logger.LogInformation($"Fin - Récupération de la liste des adhérents");

            return Ok(adherents);
        }

        [HttpPost("Adherent")]
        [Authorize]
        public ActionResult<EjtAdherent> CreateAdhenrent(EjtAdherent model)
        {
            _logger.LogInformation($"Début - Création d'un adhérent");

            var adherent = this._ejtAdherentService.Create(model);

            _logger.LogInformation($"Fin - Création d'un adhérent");

            return Ok(adherent);
        }

        [HttpPut("Adherent")]
        [Authorize]
        public ActionResult<EjtAdherent> UpdateAdhenrent(EjtAdherent model)
        {
            _logger.LogInformation($"Début - Mise à jour d'un adhérent");

            var adherent = this._ejtAdherentService.Update(model);

            _logger.LogInformation($"Fin - Mise à jour d'un adhérent");

            return Ok(adherent);
        }

        //[HttpPost("WordDoc")]
        //public ActionResult GenerateWordDoc()
        //{
        //    var adherents = this._ejtAdherentService.GetAll();
        //    var template = "C:\\Users\\echomel\\source\\repos\\Judo\\Communication\\Plaquettes\\V2\\Inscriptions\\Template.docx";

        //    foreach (var adherent in adherents)
        //    {
        //        var adherentFilePath = $"C:\\Users\\echomel\\source\\repos\\Judo\\Communication\\Plaquettes\\V2\\Inscriptions\\Files\\{adherent.LicenceCode.Replace("*", "_")}.docx";
        //        System.IO.File.Copy(template, adherentFilePath);

        //        using (var doc = WordprocessingDocument.Open(adherentFilePath, true))
        //        {
        //            var body = doc.MainDocumentPart.Document.Body;
        //            var paras = body.Elements<Paragraph>();

        //            foreach (var para in paras)
        //            {
        //                foreach (var run in para.Elements<Run>())
        //                {
        //                    foreach (var text in run.Elements<Text>())
        //                    {
        //                        if (text.Text.Contains("_LICENCE_"))
        //                        {
        //                            text.Text = text.Text.Replace("_LICENCE_", adherent.LicenceCode);
        //                        }
        //                    }
        //                }
        //            }

        //            doc.Save();
        //            doc.Close();
        //        }
        //    }

        //    return Ok();
        //}
    }
}
