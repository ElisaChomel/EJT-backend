using judo_backend.Models;

namespace judo_backend.Services.Interfaces
{
    public interface INewService
    {
        List<New> GetAll();

        New Get(int id);

        New Upload(New n);

        void Update(New n);
    }
}
