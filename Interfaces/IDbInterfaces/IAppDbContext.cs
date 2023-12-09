using JobHubBot.Db.Entities;
using Microsoft.EntityFrameworkCore;
using User = JobHubBot.Db.Entities.User;

namespace JobHubBot.Interfaces.IDbInterfaces
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Job> Jobs { get; set; }
        DbSet<Skill> Skills { get; set; }
        DbSet<UserSkill> UserSkills { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
