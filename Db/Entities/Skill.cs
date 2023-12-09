namespace JobHubBot.Db.Entities
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        public ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}
