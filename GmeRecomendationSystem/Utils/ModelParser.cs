using Microsoft.Data.SqlClient;
using GmeRecomendationSystem.Models;

namespace GmeRecomendationSystem.Utils
{
	static class ModelParser
	{
        public static List<ReviewModel> ParseReviews(SqlDataReader reader, Dictionary<int, int> checkedGames, ref Dictionary<string, double> userSim)
		{
			Dictionary<string, Dictionary<int, int>> userScores = new Dictionary<string, Dictionary<int, int>>();
            List<ReviewModel> reviewModels = new List<ReviewModel>();
			while (reader.Read())
			{
				ReviewModel model = new ReviewModel();
				model.UserID = reader.GetDecimal(0);
				model.ItemID = reader.GetInt32(1);
				model.Score = reader.GetInt32(2);
				if(!(reader.GetValue(3) is DBNull))
					model.Assists = reader.GetInt32(3);
				else
                    model.Assists = 0;
                if (!(reader.GetValue(4) is DBNull))
                    model.Weight = Convert.ToSingle(reader.GetValue(4));
				else
                    model.Weight = 0;
                if (!(reader.GetValue(5) is DBNull))
                    model.UseTime = reader.GetInt32(5);
				else
                    model.UseTime = 0;
				model.SAID = reader.GetInt32(6);
                model.IsApp = reader.GetBoolean(7);
                model.Genre = reader.GetString(8);
				reviewModels.Add(model);

				if (!checkedGames.ContainsKey(model.ItemID))
					continue;
				string key = Convert.ToString(model.UserID) + "_" + Convert.ToString(reader.GetBoolean(7));
				if(userScores.ContainsKey(key))
					userScores[key].Add(model.ItemID, model.Score);
				else
				{
					userScores.Add(key, new Dictionary<int, int>());
					userScores[key].Add(model.ItemID, model.Score);
				}
			}
			userSim = ParseSimilarity(checkedGames, userScores);
			return reviewModels;
		}
        public static PageModel ParsePage(SqlDataReader reader, int page, int size, int subjectScore)
		{
			PageModel pageModel = new PageModel(page, size);
			while (reader.Read())
			{
				ItemModel model = new ItemModel();
				model.Id = reader.GetInt32(0);
				model.Title = reader.GetString(1);
				model.Description = reader.GetString(2);
				model.Genre = reader.GetString(3);
				model.ImageURL = reader.GetString(4);
				model.Release = reader.GetDateTime(5);
				model.Author = reader.GetString(6);
				if(reader.GetValue(8) is DBNull)
                {
                    model.Score = -1;
					model.Checked = false;
                }
				else
                {
                    model.Score = reader.GetInt32(8);
                    model.Checked = true;
                }
				model.MaxScore = subjectScore;
				pageModel.Items.Add(model);
			}
			return pageModel;
		}
        public static UserModel ParseUser(SqlDataReader reader, string password)
		{
			reader.Read();
			if (((string)reader.GetValue(3)).Equals(password))
			{
				UserModel u = new UserModel();
				u.Id = reader.GetDecimal(0);
				u.UserName = reader.GetString(1);
				u.UserEmail = reader.GetString(2);
				u.Role = reader.GetString(4);
				return u;
			}
			return new UserModel();
		}
        public static UserModel ParseUser(SqlDataReader reader)
		{
			reader.Read();
			UserModel u = new UserModel();
			u.Id = reader.GetDecimal(0);
			u.UserName = reader.GetString(1);
			u.UserEmail = reader.GetString(2);
            u.Role = reader.GetString(4);
            return u;
		}
        public static List<int> ParseID(SqlDataReader reader)
		{
			List<int> IDs = new List<int>();
			while (reader.Read())
				if (!IDs.Contains(reader.GetInt32(0)))
					IDs.Add(reader.GetInt32(0));
			return IDs;
		}
        public static List<string> ParseGenre(SqlDataReader reader)
		{
			List<string> genres = new List<string>();
			while (reader.Read())
			{
				string[] genre = reader.GetString(0).Split(':');
				foreach (string gen in genre)
				{
					string[] genre_ = reader.GetString(0).Split(';');
                    foreach (string gen_ in genre_)
                        if (!genres.Contains(gen_))
                            genres.Add(gen_);
                }
			}
			return genres;
		}
        public static List<SubjectRangeModel> ParseSA(SqlDataReader reader)
		{
            List<SubjectRangeModel> list = new List<SubjectRangeModel>();
			while(reader.Read())
            {
                SubjectRangeModel model = new SubjectRangeModel();
                model.SAID = reader.GetInt32(0);
                model.Name = reader.GetString(1);
                model.Score = reader.GetInt32(2);
                model.ScoreK = Convert.ToSingle(reader.GetValue(3));
                model.AssistsK = Convert.ToSingle(reader.GetValue(4));
                model.WeightK = Convert.ToSingle(reader.GetValue(5));
                model.UseTimeK = Convert.ToSingle(reader.GetValue(6));
                model.InWork = reader.GetBoolean(7);
				list.Add(model);
            }
			return list;
        }
		private static Dictionary<string, double> ParseSimilarity(Dictionary<int, int> checkedGames, Dictionary<string, Dictionary<int, int>> userScores)
		{
            Dictionary<string, double>  userSim = new Dictionary<string, double>();
			foreach(string user in userScores.Keys)
			{
				userSim[user] = AngleCos(checkedGames, userScores[user]);
            }
			return userSim;
        }
        private static double AngleCos(Dictionary<int, int> a, Dictionary<int, int> b)
        {
            double cos = 0;
            double a_l = 0, b_l = 0;
            foreach (int dim in b.Keys)
            {
                if (a.ContainsKey(dim))
                {
                    cos += a[dim] * b[dim];
                    a_l += a[dim] * a[dim];
                    b_l += b[dim] * b[dim];
                }
            }
			double zai = (Math.Sqrt(a_l) * Math.Sqrt(b_l));
			return zai == 0 ? 0 : cos / zai;
        }
    }
}