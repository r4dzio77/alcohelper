using AlcoHelper.Models;
using System.Collections.Generic;

namespace AlcoHelper.ViewModels
{
    public class UserRolesViewModel
    {
        public List<User> Users { get; set; }
        public List<Role> Roles { get; set; }
    }
}