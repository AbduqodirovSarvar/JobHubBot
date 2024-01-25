using JobHubBot.Db.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JobHubBot.Db.Entities
{
    public class User
    {
        [Key]
        public long TelegramId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public Language Language { get; set; } = Language.en;
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        [JsonIgnore] // Add this attribute to break the circular reference
        public ICollection<UserSkill> Skills { get; set; } = new HashSet<UserSkill>();
    }
}
