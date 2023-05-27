using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GmeRecomendationSystem.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Анонімів тут не люблять")]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "Ви маєте цілих 32 символи довжини для свого ім'я на сайті, тож введіть хоча б 2 з них.")]
        [RegularExpression(@"^[A-Za-z0-9]+([A-Za-z0-9]*|[._-]?[A-Za-z0-9]+)*$", ErrorMessage = "Невже важко ввести адекватний нік?")]
        public string NickName { get; set; }

        [Required(ErrorMessage = "А як же спам на пошті?")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "А тепер, спробуй надіслати листа на цю пошту...")]
        [Remote(action: "CheckRegisterEmail", controller: "Home", HttpMethod = "Post", ErrorMessage = "У базі вже існує користувач з данною поштою.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Чогось не вистачає.")]
        [DataType(DataType.Password)]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Пароль має бути хоча б 8 символів, але не більше 32.")]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*_-s]).{8,}$", ErrorMessage = "Використай великі та маленькі літері англійського алфавіту, цифри та спеціальні символи:#?!@$%^&*_-")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Пароль слід повторити.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Погано, коли не можеш повторити тільки введений пароль.")]
        public string PasswordConfirm { get; set; }
    }
}
