using judo_backend.Models.Stats;

namespace judo_backend.Services.Interfaces
{
    public interface IStatsService
    {
        Stats Get(int id);

        Stats Get(DateTime date, string pageName);

        List<string> GetPageNames(DateTime date);

        Stats Create(DateTime date, string pageName);

        Stats Update(int id);

        PieValues GetPieValues();
    }
}
