using ClosedXML.Excel;
using judo_backend.Models;

namespace judo_backend.Services.Interfaces
{
    public interface IExcelGeneratorService
    {
        XLWorkbook GenerateCommandeList(List<ClotheOrderItem> items, List<ClotheOrder> orders);

        XLWorkbook GenerateEjtAdherentList(string title, List<EjtAdherent> data);
    }
}
