using judo_backend.Models;

namespace judo_backend.Services.Interfaces
{
    public interface IStageService
    {
        List<Stage> GetAll();

        List<Stage> GetAllActive();

        List<int> GetStagesInscription(int adherentId);

        List<EjtAdherent> GetAdherentsInscription(int id);

        Stage Get(int id);

        Stage Create(Stage s);

        void CreateStageInscription(int adherentId, int stageId);

        Stage Update(Stage s);
    }
}
