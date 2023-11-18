namespace JWT.DTOs
{
    public class RegisterDTO
    {
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<int> Roles { get; set; }
    }
}
