using judo_backend.Attributes;
using judo_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace judo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotoController : ApiController
    {
        private IBlob _blob;

        public PhotoController(ILog logger, IBlob blob) : base(logger)
        {
            _blob = blob;
        }

        [HttpGet("folder/{folderName}")]
        public ActionResult<IList<string>> Get(string folderName)
        {
            _logger.LogInformation($"Début - Récupération des noms des fichier s du dossier {folderName}");

            var list = this._blob.Get($"Photos/{folderName}");

            _logger.LogInformation($"Fin - Récupération des noms des fichier s du dossier {folderName}");

            return Ok(list);
        }

        [HttpGet("folder/{folderName}/file/{fileName}")]
        public ActionResult Get(string folderName, string fileName)
        {
            _logger.LogInformation($"Début - Récupération du fichier  {fileName} du dossier {folderName}");

            var ms = this._blob.Get($"Photos/{folderName}", fileName);

            if (ms == null)
            {
                ms = this._blob.Get($"Photos/default", "default.jpg");
            }

            ms.Position = 0;

            _logger.LogInformation($"Fin - Récupération du fichier  {fileName} du dossier {folderName}");

            return new FileStreamResult(ms, "application/octet-stream");
        }

        [HttpPost("folder/{folderName}/file/{fileName1}/{fileName2}")]
        [Authorize]
        public IActionResult Change(string folderName, string fileName1, string fileName2)
        {
            _logger.LogInformation($"Début - Changement de noms pour le fichier du dossier {folderName} ({fileName1} <=> {fileName2})");

            this._blob.Change($"Photos/{folderName}", fileName1, fileName2);
           
            _logger.LogInformation($"Fin - Changement de noms pour le fichier du dossier {folderName} ({fileName1} <=> {fileName2})");
            
            return Ok();
        }

        [HttpPost("folder/{folderName}")]
        [Authorize]
        public IActionResult Upload(string folderName, IFormFile file)
        {
            if (file.Length > 0)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                _logger.LogInformation($"Début - Upload du fichier {fileName} dans dossier {folderName}");

                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    this._blob.Upload($"Photos/{folderName}", fileName, stream);
                }
                _logger.LogInformation($"Fin - Upload du fichier {fileName} dans dossier {folderName}");

                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete("folder/{folderName}/file/{fileName}")]
        [Authorize]
        public IActionResult Delete(string folderName, string fileName)
        {
            _logger.LogInformation($"Début - Suppression du fichier {fileName} du dossier {folderName}");

            this._blob.Delete($"Photos/{folderName}", fileName);

            _logger.LogInformation($"Fin - Suppression du fichier {fileName} du dossier {folderName}");

            return Ok();
        }
    }
}
