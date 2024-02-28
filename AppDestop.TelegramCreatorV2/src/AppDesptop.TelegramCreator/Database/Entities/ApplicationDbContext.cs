using Microsoft.EntityFrameworkCore;

namespace AppDesptop.TelegramCreator.Database.Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
    }
}
