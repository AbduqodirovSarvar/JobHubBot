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
        public DbSet<UserSkill> UserSkills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(x => x.Phone).IsUnique();
            modelBuilder.Entity<Skill>().HasIndex(x => x.Name).IsUnique();

            modelBuilder.Entity<UserSkill>()
                .HasKey(us => new { us.UserId, us.SkillId });

            modelBuilder.Entity<UserSkill>()
                .HasOne(us => us.User)
                .WithMany(u => u.Skills)
                .HasForeignKey(us => us.UserId);

            modelBuilder.Entity<UserSkill>()
                .HasOne(us => us.Skill)
                .WithMany(s => s.Users)
                .HasForeignKey(us => us.SkillId);
        }
    }
}
