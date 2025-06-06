using System.ComponentModel.DataAnnotations;

namespace AlcoHelper.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Stare hasło jest wymagane")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Nowe hasło jest wymagane")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Hasła muszą być takie same")]
        public string ConfirmPassword { get; set; }
    }
}
