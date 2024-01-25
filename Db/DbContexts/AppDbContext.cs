using JobHubBot.Db.Entities;
using JobHubBot.Interfaces.IDbInterfaces;
using Microsoft.EntityFrameworkCore;
using User = JobHubBot.Db.Entities.User;

namespace JobHubBot.Db.DbContexts
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Skill> Skills { get; set; }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(@"Data Source=C:\Users\Sarvar\OneDrive\Ishchi stol\JobHubBot\Db\JoboHubBot.db");
        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(x => x.Phone).IsUnique();
            modelBuilder.Entity<Skill>().HasIndex(x => x.Name).IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(x => x.Skills)
                .WithOne(y => y.User).HasForeignKey(x => x.UserId);

            modelBuilder.Entity<Skill>()
                .HasMany(x => x.Users)
                .WithOne(y => y.Skill).HasForeignKey(x => x.SkillId);
        }
    }
}
