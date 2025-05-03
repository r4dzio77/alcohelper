using AlcoHelper.Models;

namespace AlcoHelper.ViewModels
{
    public class EmailNotification //Zarzadzanie powiadomieniami e-mail
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Type { get; set; } // np. "NewReview", "Promotion" itp, itd
        public bool IsSent { get; set; }
        public DateTime CreatedAt { get; set; }

        public User? User { get; set; }
    }
}
