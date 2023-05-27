namespace GmeRecomendationSystem.Models
{
    public record ReviewModel
	{
		public decimal UserID { get; set; }
		public int ItemID { get; set; }
		public int Score { get; set; }
		public int Assists { get; set; }
		public float Weight { get; set; }
		public int UseTime { get; set; }
		public string Genre { get; set; }
        public int SAID { get; set; }
		public bool IsApp { get; set; }
    }
}
