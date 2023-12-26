namespace judo_backend.Services.Interfaces
{
    public interface IEmailService
    {
        void SendWelcome(string to, string username);

        void SendPasswordChanged(string to, string username);

        void SendPasswordForgot(int userId, string to, string username, string token, string code);

        void SendGoodBye(string to, string username);

        void SendCompetitionRegistred(string to, string username, string adherentName, string competitionName);

        void SendStageRegistred(string to, string username, string adherentName, string stageName, string start, string end);
    }
}
