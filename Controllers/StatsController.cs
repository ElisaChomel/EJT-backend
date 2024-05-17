using DocumentFormat.OpenXml.Office.Word;
using judo_backend.Attributes;
using judo_backend.Models.Stats;
using judo_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace judo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ApiController
    {
        private IStatsService _statsService;

        public StatsController(ILog logger, IStatsService statsService) : base(logger)
        {
            _statsService = statsService;
        }

        [HttpPost("pageName/{pageName}")]
        public ActionResult<Stats> CreateOrUpdate(string pageName)
        {
            _logger.LogInformation($"Début - Création ou mise à jour d'une stat");

            DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            Stats ret = this._statsService.Get(date, pageName);

            if (ret == null)
            {
                ret = this._statsService.Create(date, pageName);
            }
            else
            {
                ret = this._statsService.Update(ret.Id);
            }

            _logger.LogInformation($"Fin - Création ou mise à jour d'une stat");

            return Ok(ret);
        }

        [HttpGet("pieValues")]
        public ActionResult<PieValues> GetPieValues()
        {
            _logger.LogInformation($"Début - Récupération des valeurs pour le graphe pie");

            PieValues values = this._statsService.GetPieValues();

            _logger.LogInformation($"Fin - Récupération des valeurs pour le graphe pie");

            return Ok(values);
        }

        [HttpGet("barValues")]
        public ActionResult<BarValues> GetBarValues()
        {
            var values = new BarValues();
            
            _logger.LogInformation($"Début - Récupération des valeurs pour le graphe bar");

            var dates = new List<DateTime>();

            for(int i = 4; i >= 0; i--)
            {
                var date = new DateTime(DateTime.Now.AddMonths(-i).Year, DateTime.Now.AddMonths(-i).Month, 1);
                dates.Add(date);
                values.Labels.Add(date.ToString("MMMM yy", new CultureInfo("fr-FR")));
            }
            
            var names = new List<string>();
            foreach (var date in dates)
            {
                var temp = this._statsService.GetPageNames(date);    
                
                foreach (var name in temp)
                {
                    if (!values.Labels.Contains(name))
                    {
                        names.Add(name);
                    }
                }
            }
            foreach (var name in names)
            {
                var dataset = new BarValueDataset();
                dataset.Label = name;

                foreach (var date in dates)
                {                
                    var stat = this._statsService.Get(date, name);

                    if(stat == null)
                    {
                        dataset.Data.Add(0);
                    }
                    else
                    {
                        dataset.Data.Add(stat.CountView);
                    }
                }

                values.Values.Add(dataset);
            }

            _logger.LogInformation($"Fin - Récupération des valeurs pour le graphe bar");

            return Ok(values);
        }
    }
}
