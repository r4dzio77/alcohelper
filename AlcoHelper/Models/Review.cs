using System;
using System.ComponentModel.DataAnnotations;

namespace AlcoHelper.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public int AlcoholId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Ocena musi być między 1 a 5.")]
        public double Rating { get; set; } // Ocena 

        [Required(ErrorMessage = "Komentarz jest wymagany.")]
        [MinLength(10, ErrorMessage = "Komentarz musi mieć co najmniej 10 znaków.")]
        public string Comment { get; set; }

        public string? ImageUrl { get; set; } // Adres url do Zdjecia 

        public DateTime CreatedAt { get; set; }

        public User? User { get; set; }
        public Alcohol? Alcohol { get; set; }
    }
}
