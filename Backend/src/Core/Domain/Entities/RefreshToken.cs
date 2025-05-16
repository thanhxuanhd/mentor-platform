namespace Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string TokenHash { get; set; } = string.Empty;
        public string TokenSalt { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ExpiredAt { get; set; } = DateTime.Now.AddDays(7);
        public User? User { get; set; }
    }
}
