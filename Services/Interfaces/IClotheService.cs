using judo_backend.Models;

namespace judo_backend.Services.Interfaces
{
    public interface IClotheService
    {
        DateTime GetDate();

        DateTime SetDate(DateTime date);

        Clothe Get(string reference);

        MemoryStream GetFile(string reference);

        List<ClotheOrder> GetAll();

        List<ClotheOrderItem> GetAllItem();

        ClotheOrder InsertOrder(ClotheOrder order);

        void UpdateOrderIsPay(int id);

        void Delete();
    }
}
