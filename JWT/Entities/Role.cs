using System.Text.Json.Serialization;

namespace JWT.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public List<User> Users { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}
