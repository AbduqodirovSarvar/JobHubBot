namespace JobHubBot.Db.Entities
{
    public class UserSkill
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public User? User { get; set; }
        public int SkillId { get; set; }
        public Skill? Skill { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    }
}
