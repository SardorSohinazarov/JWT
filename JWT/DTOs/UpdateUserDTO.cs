namespace JWT.DTOs
{
    public class UpdateUserDTO
    {
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public List<int> Roles { get; set; }
    }
}
