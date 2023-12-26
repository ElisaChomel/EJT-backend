using ClosedXML.Excel;
using judo_backend.Models;

namespace judo_backend.Services.Interfaces
{
    public interface IExcelGeneratorService
    {
        XLWorkbook GenerateEjtAdherentList(string title, List<EjtAdherent> data);
    }
}
