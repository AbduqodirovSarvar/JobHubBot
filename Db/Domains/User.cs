using JobHubBot.Db.Enums;

namespace JobHubBot.Db.Domains
{
    public class User
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; }  = string.Empty;
        public Language Language { get; set; } = Language.en;
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        public ICollection<Job> Jobs { get; set; } = new HashSet<Job>();
    }
}
