namespace GmeRecomendationSystem.Models
{
    public record RecommendationModel
	{
		public int ItemId { get; set; }
		public float Score { get; set; }
	}
}
