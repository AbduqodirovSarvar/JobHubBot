using System.ComponentModel.DataAnnotations;

namespace JobHubBot.Db.Domains
{
    public class Job
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? Location { get; set; }
        public ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}
