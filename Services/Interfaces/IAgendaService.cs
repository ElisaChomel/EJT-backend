using judo_backend.Models;

namespace judo_backend.Services.Interfaces
{
    public interface IAgendaService
    {
        List<Agenda> GetAll();

        Agenda Get(int id);

        Agenda Upload(Agenda a);

        void Update(Agenda a);
    }
}
