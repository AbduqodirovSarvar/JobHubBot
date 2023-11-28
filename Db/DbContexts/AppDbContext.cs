using JobHubBot.Db.Domains;
using JobHubBot.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobHubBot.Db.DbContexts
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<UserJobs> UsersJobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(x => x.Phone).IsUnique();
            modelBuilder.Entity<Job>().HasIndex(x => x.Name).IsUnique();

            modelBuilder.Entity<UserJobs>()
                .HasKey(uj => new { uj.UserId, uj.JobId });
        }
    }
}
