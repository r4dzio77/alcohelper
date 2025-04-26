namespace AlcoHelper.Models
{
    public class FavoriteAlco
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AlcoholId { get; set; }

        public User? User { get; set; }
        public Alcohol? Alcohol { get; set; }
    }
}
