using Microsoft.EntityFrameworkCore;

namespace WpfApp
{
    internal class UserDataContext:DbContext
    {

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data source = Users.db");
        }
    }
}
