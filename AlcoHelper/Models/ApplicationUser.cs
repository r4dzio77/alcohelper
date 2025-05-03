using Microsoft.AspNetCore.Identity;

namespace AlcoHelper.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsAdmin { get; set; } = false;
    }
}
