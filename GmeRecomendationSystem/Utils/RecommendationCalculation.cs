using GmeRecomendationSystem.Models;

namespace GmeRecomendationSystem.Utils
{
	internal static class RecomendationCalculation
	{
		public async static Task CreateRecommendation(int userID, int said)
		{
			Dictionary<string, double> userSimilarity = new Dictionary<string, double>();
			PageModel pageModel = await DBWork.GetCheckedItems(userID, said);
			SubjectRangeModel SAModel = await DBWork.GetSubjectRange(said);
			Dictionary<int, int> checkedGames = new Dictionary<int, int>();
			foreach(ItemModel itemModel in pageModel.Items)
			{
				checkedGames[itemModel.Id] = itemModel.Score;
			}
			List<ReviewModel> reviews = DBWork.GetReviewsByUsers(userID, checkedGames, ref userSimilarity, said);

			List<string> genres = new List<string>();
			Dictionary<int, float> game_score = new Dictionary<int, float>();
            genres = DBWork.GetGenres(userID, said);
			foreach (ReviewModel rev in reviews)
			{
				if (checkedGames.ContainsKey(rev.ItemID))
					continue;

				string user = Convert.ToString(rev.UserID) + "_" + Convert.ToString(rev.IsApp);
                double us = userSimilarity.ContainsKey(user) ? userSimilarity[user] : 0;
				if (game_score.ContainsKey(rev.ItemID))
					game_score[rev.ItemID] += await ReviewScore(rev, us, genres, SAModel);
				else
					game_score[rev.ItemID] = await ReviewScore(rev, us, genres, SAModel);
			}

			if (game_score.Count < 5)
				await FullGenreRecommendation(game_score, userID, checkedGames, said);

			List<KeyValuePair<int, float>> l_game_score = game_score.ToList();
			l_game_score.Sort((x, y) => y.Value.CompareTo(x.Value));
			await DBWork.SetRecommendations(userID, l_game_score, said);
		}
		
		private async static Task FullGenreRecommendation(Dictionary<int, float> game_score, int userID, Dictionary<int, int> checkedGames, int said)
        {
            Dictionary<string, double> userSimilarity = new Dictionary<string, double>();
            List<ReviewModel> reviews = DBWork.GetReviewsByGenre(userID, checkedGames, ref userSimilarity, said);
            SubjectRangeModel SAModel = await DBWork.GetSubjectRange(said);

            foreach (ReviewModel rev in reviews)
			{
				if (checkedGames.ContainsKey(rev.ItemID))
					continue;

                string user = Convert.ToString(rev.UserID) + "_" + Convert.ToString(rev.IsApp);
                double us = userSimilarity.ContainsKey(user) ? userSimilarity[user] : 1;
				if (game_score.ContainsKey(rev.ItemID))
					game_score[rev.ItemID] += await ReviewScore(rev, us, SAModel);
				else
					game_score[rev.ItemID] = await ReviewScore(rev, us, SAModel);
			}

		}

        private async static Task<float> ReviewScore(ReviewModel rev, double userSim, List<string> genres, SubjectRangeModel SAModel)
		{
			double mainK = userSim;
			mainK += userSim * (rev.Assists * SAModel.AssistsK);
			mainK += userSim * (rev.Weight * SAModel.WeightK);
			mainK += userSim * (rev.UseTime * SAModel.UseTimeK);
			mainK += await FirstInSecondCount(rev.Genre.Split(new Char[] {':', ';', ','}, StringSplitOptions.RemoveEmptyEntries), genres) / 100;
			return (float)mainK * (rev.Score * SAModel.ScoreK);
        }
        private async static Task<float> ReviewScore(ReviewModel rev, double userSim, SubjectRangeModel SAModel)
        {
            double mainK = 1 + userSim;
            mainK += userSim * (rev.Assists * SAModel.AssistsK);
            mainK += userSim * (rev.Weight * SAModel.WeightK);
			mainK += userSim * (rev.UseTime * SAModel.UseTimeK);
            return (float)mainK * (rev.Score * SAModel.ScoreK);
        }

        private async static Task<int> FirstInSecondCount(IEnumerable<string> first, IEnumerable<string> second)
		{
			int count = 0;
			foreach (string f in first)
				if (second.Contains(f)) count++;
			return count;
		}
	}
}