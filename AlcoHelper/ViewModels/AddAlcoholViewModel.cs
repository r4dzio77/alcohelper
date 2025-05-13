using AlcoHelper.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace AlcoHelper.ViewModels
{
    public class AddAlcoholViewModel
    {
        [Required(ErrorMessage = "Nazwa jest wymagana!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Typ jest wymagany!")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Kraj pochodzenia jest wymagany!")]
        public string Country { get; set; }

        [Range(0.1, 100, ErrorMessage = "Zawartość alkoholu musi być pomiędzy 0.1% a 100%!")]
        public double AlcoholPercentage { get; set; }

        [StringLength(500, ErrorMessage = "Opis może mieć maksymalnie 500 znaków!")]
        public string Description { get; set; }

        [Url(ErrorMessage = "Podaj poprawny URL zdjęcia!")]
        public string? ImageUrl { get; set; }

        public List<int> TagIds { get; set; }  // Lista wybranych tagów
    }
}