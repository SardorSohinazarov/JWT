namespace JWT.DTOs
{
    public class UpdateRoleDTO
    {
        public string Name { get; set; }
        public List<int> Permissions { get; set; }
    }
}
