using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataContext
{
    public class DatabaseContext : DbContext
    {
        public class OptionsBuild
        {
            public OptionsBuild()
            {
                settings = new AppConfiguration();
                opsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
                opsBuilder.UseSqlServer(settings.sqlConnectionString);
                dbOptions = opsBuilder.Options;
            }
            public DbContextOptionsBuilder<DatabaseContext> opsBuilder { get; set; }
            public DbContextOptions<DatabaseContext> dbOptions { get; set; }
            private AppConfiguration settings { get; set; }
        }
        public static OptionsBuild ops = new OptionsBuild();
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<MTTransaction> MTTransactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasIndex(p => new { p.UserName })
                .IsUnique(true);
        }
    }
}
