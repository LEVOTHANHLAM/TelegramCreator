using AppDesptop.TelegramCreator.Database.Entities;

namespace AppDesptop.TelegramCreator.Database.Repositories
{
    public interface IAccountRepository
    {
        void Add(Account account);
        List<Account> GetAll();
        void Update(Account account);
        Account GetById(Guid id);
        void Delete(Guid id);
        bool DeleteRange(List<string> deleteList);
        void DeleteAll();
    }
}
