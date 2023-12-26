using judo_backend.Models;

namespace judo_backend.Services.Interfaces
{
    public interface ICompetitionService
    {
        List<Competition> GetAll();

        List<Competition> GetAllActive();

        List<int> GetCompetitionsInscription(int adherentId);

        List<EjtAdherent> GetInscription(int id);

        Competition Get(int id);

        List<CompetitionResult> GetResults(int id);
        
        CompetitionResult GetResult(int id);

        Competition Create(Competition n);

        CompetitionResult CreateResult(CompetitionResult n);

        Competition Update(Competition n);

        CompetitionResult UpdateResult(CompetitionResult n);
    }
}
