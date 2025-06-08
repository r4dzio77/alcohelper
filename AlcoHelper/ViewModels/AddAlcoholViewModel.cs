using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AlcoHelper.ViewModels
{
    public class AddAlcoholViewModel
    {
        public int Id { get; set; }

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

        public string? ExistingImageUrl { get; set; }

        public List<int> TagIds { get; set; } = new();

        [DataType(DataType.Upload)]
        public IFormFile? ImageFile { get; set; } // ← dodane poprawnie
    }
}
