using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace AlcoHelper.Models
{
    public class Alcohol
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; } 
        public string? Country { get; set; }
        public double AlcoholPercentage { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime AddedDate { get; set; }

        public List<Review>? Reviews { get; set; }
        public List<AlcoholTag>? AlcoholTags { get; set; }
        public List<AlcoholStore>? AlcoholStores { get; set; }
        public List<FavoriteAlco>? Favorites { get; set; }
        public List<WishList>? Wishlist { get; set; }

        public bool IsApproved { get; set; } = false;
    }
}

