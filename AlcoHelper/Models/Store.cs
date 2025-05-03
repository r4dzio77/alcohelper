namespace AlcoHelper.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WebsiteUrl { get; set; }
        public List<AlcoholStore> AlcoholStores { get; set; }
    }

}
