using AlcoHelper.Models;
using System.Collections.Generic;

public class AccountSettingsViewModel
{
    public User User { get; set; }
    public List<Alcohol> FavoriteAlcohols { get; set; }
    public List<Review> UserReviews { get; set; }
}
