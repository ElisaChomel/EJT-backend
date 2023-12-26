using judo_backend.Models;

namespace judo_backend.Services.Interfaces
{
    public interface IEjtAdherentService
    {
        EjtAdherent Get(int id);

        EjtAdherent Get(string licenceCode);

        List<EjtAdherent> GetAll();

        List<EjtAdherent> GetAllByUserId(int userId);

        EjtAdherent Create(EjtAdherent model);

        EjtAdherent Update(EjtAdherent model);
    }
}
