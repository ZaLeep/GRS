using System.ComponentModel.DataAnnotations;

namespace GmeRecomendationSystem.Models
{
    public class SubjectRangeModel
    {
        public int SAID { get; set; }
        [Required(ErrorMessage ="Введіть назву предметної області.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введіть максимальне значення оцінки для предметної області.")]
        [Range(1, 100, ErrorMessage = "Максимальне значення може бути від 1 до 100.")]
        public int Score { get; set; }

        [Required(ErrorMessage = "Введіть коефіцієнт значимості оцінки відгуку.")]
        [Range(0, 1, ErrorMessage = "Коефіцієнт обмежений значенням від 0 до 1.")]
        public float ScoreK { get; set; }

        [Required(ErrorMessage = "Введіть коефіцієнт значимості кількості допомог відгуку.")]
        [Range(0, 1, ErrorMessage = "Коефіцієнт обмежений значенням від 0 до 1.")]
        public float AssistsK { get; set; }

        [Required(ErrorMessage = "Введіть коефіцієнт значимості ваги відгуку.")]
        [Range(0, 1, ErrorMessage = "Коефіцієнт обмежений значенням від 0 до 1.")]
        public float WeightK { get; set; }

        [Required(ErrorMessage = "Введіть коефіцієнт значимості часу використання елементу на момент відгуку.")]
        [Range(0, 1, ErrorMessage = "Коефіцієнт обмежений значенням від 0 до 1.")]
        public float UseTimeK { get; set; }
        public bool InWork { get; set; }
    }
}
