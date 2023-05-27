namespace GmeRecomendationSystem.Models
{
    public record UserModel
	{
		public decimal Id { get; set; }
		public string UserName { get; set; }
		public string UserEmail { get; set; }
		public string Role { get; set; }
	}
}
