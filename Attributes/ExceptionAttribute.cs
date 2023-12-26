using judo_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace judo_backend.Attributes
{
    public class ExceptionAttribute : Attribute, IExceptionFilter
    {
        /// <summary>
        /// Service de log;
        /// </summary>
        public ILog _logger;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ExceptionAttribute"/>
        /// </summary>
        public ExceptionAttribute(ILog logger) { 
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError($"{context.Exception}");
        }
    }
}
