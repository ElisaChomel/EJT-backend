using judo_backend.Models.Enum;
using judo_backend.Models;
using MySqlConnector;

namespace judo_backend.Services.Interfaces
{
    public interface IEjtPersonService
    {
        EjtPerson Get(int id);

        List<EjtPerson> GetAll();

        EjtPerson Create(EjtPerson model);

        EjtPerson Update(EjtPerson model);
    }
}
