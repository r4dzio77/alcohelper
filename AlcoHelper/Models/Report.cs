namespace AlcoHelper.Models
{
    public class Report //Zglaszanie błedów
    {
        public int Id { get; set; }
        public int AlcoholId { get; set; }
        public int UserId { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public User? User { get; set; }
        public Alcohol? Alcohol { get; set; }
    }
}
