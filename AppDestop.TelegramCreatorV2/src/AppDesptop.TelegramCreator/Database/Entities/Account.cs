namespace AppDesptop.TelegramCreator.Database.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password{ get; set; }
        public string? UserName { get; set; }
        public string? Proxy { get; set; }
        public string? FullName { get; set; }
        public string? ApId { get; set; }
        public string? ApHash { get; set; }
        public string? Avatar { get; set; }
        public string? Session { get; set; }
        public DateTime? CreateDate { get; set; }

    }
}
