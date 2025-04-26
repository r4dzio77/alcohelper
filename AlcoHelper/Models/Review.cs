namespace AlcoHelper.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AlcoholId { get; set; }
        public double Rating { get; set; } // Ocena 
        public string Comment { get; set; }
        public string ImageUrl { get; set; } // Adres url do Zdjecia 
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public Alcohol Alcohol { get; set; }
    }

}
