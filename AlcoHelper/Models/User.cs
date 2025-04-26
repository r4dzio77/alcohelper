using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace AlcoHelper.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Role { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<Review>? Reviews { get; set; }
        public List<FavoriteAlco>? Favorites { get; set; }
        public List<WishList>? Wishlist { get; set; }
    }

}
