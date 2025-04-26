namespace AlcoHelper.Models
{
    public class Comment //Komentarze do recenzji innych
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ReviewId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public User? User { get; set; }
        public Review? Review { get; set; }
    }
}
