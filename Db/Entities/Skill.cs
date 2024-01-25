namespace JobHubBot.Db.Entities
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        public ICollection<UserSkill> Users { get; set; } = new HashSet<UserSkill>();
        public ICollection<Job> Jobs { get; set; } = new HashSet<Job>();
    }
}
