using judo_backend.Models;

namespace judo_backend.Services.Interfaces
{
    public interface IUserService
    {
        User Authenticate(Authenticate authenticate);

        User Get(int id);
      
        User Get(string username);

        List<User> GetAll();

        List<User> GetByAdherentId(int id);

        User Create(Authenticate model);
        
        void CreateLink(int userId, int adherentId);

        User Update(User u);

        User UpdatePassword(int id, string password);

        void Delete(int id);

        void DeleteLink(int userId);

        bool CheckPassword(int id, string password);

    }
}
