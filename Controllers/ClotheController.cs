using judo_backend.Attributes;
using judo_backend.Models;
using judo_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace judo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClotheController : ApiController
    {
        private IClotheService _clotheService;
        private IEmailService _emailService;
        private IExcelGeneratorService _excelService;

        public ClotheController(ILog logger, 
                                IClotheService clotheService, 
                                IEmailService emailService,
                                IExcelGeneratorService excelService) : base(logger)
        {
            _clotheService = clotheService;
            _emailService = emailService;
            _excelService = excelService;
        }

        [HttpGet("date")]
        public ActionResult<DateTime> GetDate()
        {
            _logger.LogInformation($"Début - Récupération de la date pour les commande");

            var date = this._clotheService.GetDate();

            _logger.LogInformation($"Fin - Récupération de la date pour les commande");

            return Ok(date);
        }

        [HttpPut("date/{date}")]
        [Authorize]
        public ActionResult<DateTime> SetDate(DateTime date)
        {
            _logger.LogInformation($"Début - Mise à jour de la date de la prochaine commande : '{date.ToString("yyyy-MM-dd")}'");

            var d = this._clotheService.SetDate(date);

            this._clotheService.Delete();

            _logger.LogInformation($"Fin - Mise à jour de la date de la prochaine commande : '{date.ToString("yyyy-MM-dd")}'");

            return Ok(d);
        }

        [HttpPost("order/confirm")]
        [Authorize]
        public ActionResult<DateTime> SetOrderConfirmReceived()
        {
            _logger.LogInformation($"Début - Envoi du mail pour informé de la reception de la commande");

            var orders = this._clotheService.GetAll();

            foreach ( var order in orders )
            {
                this._emailService.SendOrderReceived(order);
            }

            _logger.LogInformation($"Fin - Envoi du mail pour informé de la reception de la commande");

            return Ok();
        }

        [HttpGet("ref/{reference}")]
        public ActionResult<Clothe> Get(string reference)
        {
            _logger.LogInformation($"Début - Récupération du vêtement avec la ref '{reference}'");

            var clothe = this._clotheService.Get(reference);

            _logger.LogInformation($"Fin - Récupération du vêtement avec la ref '{reference}'");

            return Ok(clothe);
        }

        [HttpGet("ref/{reference}/file")]
        public ActionResult GetFile(string reference)
        {
            _logger.LogInformation($"Début - Récupération du fichier du vêtement avec la ref '{reference}");

            var ms = this._clotheService.GetFile(reference);
            ms.Position = 0;

            _logger.LogInformation($"Fin - Récupération du fichier du vêtement avec la ref '{reference}");

            return new FileStreamResult(ms, "application/png");
        }

        [HttpGet("all")]
        public ActionResult<List<ClotheOrder>> GetAll()
        {
            _logger.LogInformation($"Début - Récupération de la liste des commandes");

            var orders = this._clotheService.GetAll();

            _logger.LogInformation($"Fin - Récupération de la liste des commandes");

            return Ok(orders);
        }

        [HttpPost()]
        public ActionResult<ClotheOrder> InsertOrder(ClotheOrder order)
        {
            _logger.LogInformation($"Début - Ajout de la commande de vêtement");

            ClotheOrder ret = this._clotheService.InsertOrder(order);

            _logger.LogInformation($"Fin - Ajout de la commande de vêtement");

            _logger.LogInformation($"Début - Envoi du mail de confirmation pour la commande");

            string date = this._clotheService.GetDate().ToString("dd/MM/yyyy");

            this._emailService.SendOrderConfirmation(date, ret);

            _logger.LogInformation($"Fin - Envoi du mail de confirmation pour la commande");

            return Ok(ret);
        }

        [HttpPut("id/{id}/isPay")]
        [Authorize]
        public ActionResult UpdateOrder(int id)
        {
            _logger.LogInformation($"Début - Modification de la commande de vêtement");

            this._clotheService.UpdateOrderIsPay(id);

            _logger.LogInformation($"Fin - Modification de la commande de vêtement");

            return Ok();
        }

        [HttpGet("export")]
        [Authorize]
        public ActionResult GetOrderExcel()
        {
            _logger.LogInformation($"Début - Export des commandes");

            var orders = this._clotheService.GetAll();

            var items = this._clotheService.GetAllItem();

            var workbook = _excelService.GenerateCommandeList(items, orders);
            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Seek(0, SeekOrigin.Begin);

            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, bytes.Length);

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var donwloadFile = string.Format("{0}.xlsx", Guid.NewGuid().ToString());

            _logger.LogInformation($"Fin - Export des commandes");

            return File(bytes, contentType, donwloadFile);
        }
    }
}
