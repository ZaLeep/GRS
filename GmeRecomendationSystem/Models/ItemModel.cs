namespace GmeRecomendationSystem.Models
{
	public record class ItemModel
	{
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
		public string Genre { get; set; }
		public string ImageURL { get; set; }
        public System.DateTime Release { get; set; }
		public string Author { get; set; }
		public bool Checked { get; set; }
		public int Score { get; set; }
		public int MaxScore { get; set; }
	}
}