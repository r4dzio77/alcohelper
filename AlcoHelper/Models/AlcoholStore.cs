namespace AlcoHelper.Models
{
    public class AlcoholStore // Model do powiązania alkoholu ze sklepem
    {
        public int AlcoholId { get; set; }
        public int StoreId { get; set; }
        public decimal Price { get; set; }
        public string? ProductUrl { get; set; }

        public Alcohol? Alcohol { get; set; }
        public Store? Store { get; set; }
    }

}
