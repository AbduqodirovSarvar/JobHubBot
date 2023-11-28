namespace JobHubBot.Db.Domains
{
    public class UserJobs
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public long UserId { get; set; }
        public User? User { get; set; }
        public int JobId { get; set; }
        public Job? Job { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
