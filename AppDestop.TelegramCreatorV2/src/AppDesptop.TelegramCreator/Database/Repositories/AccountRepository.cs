using AppDesptop.TelegramCreator.Database.Entities;
using AppDesptop.TelegramCreator.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AppDesptop.TelegramCreator.Database.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(Account account)
        {
            try
            {
                account.CreateDate = DateTime.Now;
                _dbContext.Accounts.Add(account);
                _dbContext.SaveChanges();
            }catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
           
        }

        public void Delete(Guid id)
        {
            try
            {
                var delete = _dbContext.Accounts.FirstOrDefault(x => x.Id == id);
                if (delete != null)
                {
                    _dbContext.Accounts.Remove(delete);
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
          
        }

        public bool DeleteRange(List<string> deleteList)
        {
            try
            {
                foreach (var id in deleteList)
                {
                    var idAccount = new Guid(id);
                    var delete = _dbContext.Accounts.FirstOrDefault(x => x.Id == idAccount);
                    if (delete != null)
                    {
                        _dbContext.Accounts.Remove(delete);
                    }
                }
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }

        public List<Account> GetAll()
        {
            try
            {
                return _dbContext.Accounts.ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
            return null;
        }

        public Account GetById(Guid id)
        {
            try
            {
                return _dbContext.Accounts.FirstOrDefault(s => s.Id == id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
            return null;
           
        }
        public void Update(Account account)
        {
            try
            {
                if (_dbContext.Entry(account).State == EntityState.Unchanged)
                {
                    _dbContext.SaveChanges();
                }
                _dbContext.Accounts.Attach(account);
                _dbContext.Entry(account).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }
        public void DeleteAll()
        {
            try
            {
                var accounts = _dbContext.Accounts;
                _dbContext.Accounts.RemoveRange(accounts);
                _dbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        
        }
    }
}
