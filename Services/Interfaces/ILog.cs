namespace judo_backend.Services.Interfaces
{
    public interface ILog
    {
        void LogInformation(string message);

        void LogError(string message);
    }
}
