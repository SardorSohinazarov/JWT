namespace JWT.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string RefreshTokenValue { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
