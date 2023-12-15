using System.ComponentModel.DataAnnotations;

namespace JobHubBot.Db.Entities
{
    public class Job
    {
        [Key]
        public int MessageId { get; set; }
        public long FromId { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        public ICollection<Skill> Skills { get; set; } = new HashSet<Skill>();
    }
}
