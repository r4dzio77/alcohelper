namespace AlcoHelper.Models
{
    public class PasswordResetToken
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
