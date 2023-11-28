using JobHubBot.Db.Domains;
using Microsoft.EntityFrameworkCore;

namespace JobHubBot.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Job> Jobs { get; }
        DbSet<UserJobs> UsersJobs { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken=default);
    }
}
