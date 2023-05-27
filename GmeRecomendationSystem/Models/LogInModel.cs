using System.ComponentModel.DataAnnotations;

namespace GmeRecomendationSystem.Models
{
    public class LogInModel
    {
        [Required(ErrorMessage = "Без пошти ніяк.")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Щось не так з цією поштою.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Чогось не вистачає.")]
        [DataType(DataType.Password)]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Забув? 8-32 символи.")]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*_-]).{8,}$", ErrorMessage = "Використай великі та маленькі літері англійського алфавіту, цифри та спеціальні символи:#?!@$%^&*_-")]
        public string Password { get; set; }
    }
}
