using judo_backend.Attributes;
using judo_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace judo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ApiController
    {
        private IBlob _blob;

        public DocumentController(ILog logger, IBlob blob) : base(logger)
        {
            _blob = blob;
        }

        [HttpGet("file/{fileName}")]
        public ActionResult Get(string fileName)
        {
            _logger.LogInformation($"Début - Récupération du fichier {fileName}");

            var ms = this._blob.Get("Documents", fileName);
            ms.Position = 0;

            _logger.LogInformation($"Fin - Récupération du fichier {fileName}");

            return new FileStreamResult(ms, "application/pdf");
        }

        [HttpPost()]
        [Authorize]
        public IActionResult Upload(IFormFile file)
        {

            if (file.Length > 0)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                _logger.LogInformation($"Début - Upload du fichier {fileName}");

                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    this._blob.Upload("Documents", fileName, stream);
                }

                _logger.LogInformation($"Fin - Upload du fichier {fileName}");

                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
