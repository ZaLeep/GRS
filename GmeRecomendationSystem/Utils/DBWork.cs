using Microsoft.Data.SqlClient;
using System.Globalization;
using GmeRecomendationSystem.Models;

namespace GmeRecomendationSystem.Utils
{
	internal static class DBWork
	{
		private const int PAGE_SIZE = 12;
		private static SqlConnection DBconn = new SqlConnection("Data Source=SQL6031.site4now.net;Initial Catalog=db_a99fa3_urs;User Id=db_a99fa3_urs_admin;Password=35Jey_XL35");

		public  static SubjectRangeModel GetSubjectRange()
		{
            try
            {
                string query;
                query = string.Format("SELECT TOP 1 * FROM SubjectRange ORDER BY SAID");
                if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
                SqlCommand command = new SqlCommand(query, DBconn);
                SqlDataReader reader = command.ExecuteReader();
                List<SubjectRangeModel> SA = ModelParser.ParseSA(reader);
                DBconn.Close();
                return SA[0];

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
            return new SubjectRangeModel();
        }
		public  static SubjectRangeModel GetSubjectRange(int said)
        {
            try
            {
                string query;
                query = string.Format("SELECT * FROM SubjectRange WHERE SAID = {0}", said);
                if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
                SqlCommand command = new SqlCommand(query, DBconn);
                SqlDataReader reader = command.ExecuteReader();
                List<SubjectRangeModel> SA = ModelParser.ParseSA(reader);
                DBconn.Close();
                return SA[0];

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
            return new SubjectRangeModel();
        }
        public  static List<SubjectRangeModel> GetSubjectRanges(bool admin = false)
        {
            try
            {
                string query;
                if(admin)
                    query = string.Format("SELECT * FROM SubjectRange");
                else
                    query = string.Format("SELECT * FROM SubjectRange WHERE InWork = 1");
                if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
                SqlCommand command = new SqlCommand(query, DBconn);
                SqlDataReader reader = command.ExecuteReader();
                List<SubjectRangeModel> SA = ModelParser.ParseSA(reader);
                DBconn.Close();
                return SA;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
            return new List<SubjectRangeModel>();
        }
        public  static int GetSubjectRangeScore(int said)
		{
            try
            {
                string query;
                query = string.Format("SELECT ScoreRange FROM SubjectRange WHERE SAID = {0}", said);
                if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
                SqlCommand command = new SqlCommand(query, DBconn);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
				int score = reader.GetInt32(0);
                DBconn.Close();
                return score;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
            return -1;
        }
        public  static void AddSubjectRange(SubjectRangeModel model)
        {
            try
            {
                if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
                string query = string.Format("INSERT INTO SubjectRange(SubjectName, ScoreRange, ScoreK, AssistsK, WeightK, UseTimeK, InWork) " +
											"VALUES('{0}', {1}, {2}, {3}, {4}, {5}, 0)", model.Name, model.Score, model.ScoreK.ToString(CultureInfo.InvariantCulture), model.AssistsK.ToString(CultureInfo.InvariantCulture), model.WeightK.ToString(CultureInfo.InvariantCulture), model.UseTimeK.ToString(CultureInfo.InvariantCulture));
                SqlCommand command = new SqlCommand(query, DBconn);
                command.ExecuteNonQuery();
                DBconn.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
        }
        public  static void SetSubjectRangeUse(int said, int set)
        {
            try
            {
                if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
                string query = string.Format("UPDATE SubjectRange SET InWork = {0} WHERE SAID = {1}", set, said);
                SqlCommand command = new SqlCommand(query, DBconn);
                command.ExecuteNonQuery();
                DBconn.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
        }

        public  static ItemModel GetItem(int itemID, int said, int userID)
		{
			try
			{
				int score =  GetSubjectRangeScore(said);
				string query;
					query = string.Format("SELECT I.*, Ch.Score, Ch.ScoreRange FROM Item AS I " +
							"LEFT JOIN (SELECT R.*, SR.ScoreRange FROM Review AS R LEFT JOIN SubjectRange AS SR ON R.SAID = SR.SAID " +
							"WHERE UserID = {1} AND R.SAID = {2} AND IsApp = 1) AS Ch ON I.ItemID = Ch.ItemID " +
                            "WHERE I.ItemID = {0} AND I.SAID = {2}", itemID, userID, said);
                if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
                SqlCommand command = new SqlCommand(query, DBconn);
				SqlDataReader reader = command.ExecuteReader();
				PageModel page = ModelParser.ParsePage(reader, 0, 0, score);
				DBconn.Close();
				return page.Items[0];

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
			return new ItemModel();
		}

        public  static PageModel? GetLibraryItems(int page, int userID, int said)
		{
			try
            {
                int score =  GetSubjectRangeScore(said);
                string query;
				query = string.Format("SELECT I.*, Ch.Score FROM Item AS I " +
							"LEFT JOIN (SELECT * FROM Review WHERE UserID = {2} AND SAID = {3} AND IsApp = 1) AS Ch ON I.ItemID = Ch.ItemID " +
                            "WHERE I.SAID = {3} " +
							"ORDER BY ItemID OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", PAGE_SIZE * (page - 1), PAGE_SIZE + 1, userID, said);
				if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
				SqlCommand command = new SqlCommand(query, DBconn);
				SqlDataReader reader = command.ExecuteReader();
				PageModel pageModel = ModelParser.ParsePage(reader, page, PAGE_SIZE, score);
				DBconn.Close();
				return pageModel;

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
			return null;
		}
        public  static PageModel? GetLibraryItems(string search, int page, int userID, int said)
		{
			try
            {
                int score =  GetSubjectRangeScore(said);
                string searchLow = search.ToLower();
				string query;
				query = string.Format("SELECT I.*, Ch.Score FROM Item AS I " +
							"LEFT JOIN (SELECT * FROM Review WHERE UserID = {3} AND SAID = {4} AND IsApp = 1) AS Ch ON I.ItemID = Ch.ItemID " +
							"WHERE LOWER(Title) LIKE '%{2}%' AND I.SAID = {4}" +
                            "ORDER BY ItemID OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", PAGE_SIZE * (page - 1), PAGE_SIZE + 1, searchLow, userID, said);
                if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
                SqlCommand command = new SqlCommand(query, DBconn);
				SqlDataReader reader = command.ExecuteReader();
				PageModel pageModel = ModelParser.ParsePage(reader, page, PAGE_SIZE, score);
				DBconn.Close();
				return pageModel;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
			return null;

		}

        public  static UserModel? LogIn(string email, string password)
		{
			try
			{
				if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
				string query = string.Format("SELECT * FROM AppAccount WHERE Email = '{0}'", email);
				SqlCommand command = new SqlCommand(query, DBconn);
				SqlDataReader reader = command.ExecuteReader();
				UserModel userModel = ModelParser.ParseUser(reader, password);
				DBconn.Close();
				return userModel;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
			return null;
		}
        public  static UserModel Registrate(string username, string email, string password)
		{
			try
			{
				if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
				string query = string.Format("INSERT INTO AppAccount(Nickname, Email, UserPassword, UserRole) VALUES('{0}', '{1}', '{2}', 'user')", username, email, password);
				SqlCommand command = new SqlCommand(query, DBconn);
				command.ExecuteNonQuery();
				DBconn.Close();

				if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
				query = string.Format("SELECT * FROM AppAccount WHERE Email = '{0}'", email);
				command = new SqlCommand(query, DBconn);
				SqlDataReader reader = command.ExecuteReader();
				UserModel userModel = ModelParser.ParseUser(reader);
				DBconn.Close();
				return userModel;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				if (DBconn.State == System.Data.ConnectionState.Open)
					DBconn.Close();
			}
			return new UserModel();
		}
        public  static int CheckEmailExistence(string email)
		{
			try
			{
				if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
				string query = string.Format("SELECT COUNT(UserID) FROM AppAccount WHERE Email = '{0}'", email);
				SqlCommand command = new SqlCommand(query, DBconn);
				int count = (int)command.ExecuteScalar();
                DBconn.Close();
				return count;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
			return -1;
		}

		public  static PageModel? GetCheckedItems(int userID, int page, int said)
        {
            try
            {
                int score =  GetSubjectRangeScore(said);
                string query = string.Format("SELECT I.*, Ch.Score FROM Item AS I " +
                               "LEFT JOIN (SELECT * FROM Review WHERE UserID = {2} AND SAID = {3} AND IsApp = 1) AS Ch ON I.ItemID = Ch.ItemID " +
                               "WHERE I.ItemID IN (SELECT ItemID FROM Review WHERE UserID = {2} AND SAID = {3} AND IsApp = 1) AND I.SAID = {3}" +
                               "ORDER BY ItemID OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", PAGE_SIZE * (page - 1), PAGE_SIZE + 1, userID, said);
                if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
                SqlCommand command = new SqlCommand(query, DBconn);
                SqlDataReader reader = command.ExecuteReader();
                PageModel pageModel = ModelParser.ParsePage(reader, page, PAGE_SIZE, score);
                DBconn.Close();
                return pageModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
            return null;
        }
        public  static PageModel? GetCheckedItems(int userID, string search, int page, int said)
        {
            try
            {
                int score =  GetSubjectRangeScore(said);
                string searchLow = search.ToLower();
                string query = string.Format("SELECT I.*, Ch.Score FROM Item AS I " +
							   "LEFT JOIN (SELECT * FROM Review WHERE UserID = {3} AND SAID = {4} AND IsApp = 1) AS Ch ON I.ItemID = Ch.ItemID " +
                               "WHERE LOWER(Title) LIKE '%{2}%' AND I.SAID = {4} AND " +
                               "I.ItemID IN(SELECT ItemID FROM Review WHERE UserID = {3} AND SAID = {4} AND IsApp = 1) " +
                               "ORDER BY ItemID OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", PAGE_SIZE * (page - 1), PAGE_SIZE + 1, searchLow, userID, said);
                if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
                SqlCommand command = new SqlCommand(query, DBconn);
                SqlDataReader reader = command.ExecuteReader();
                PageModel pageModel = ModelParser.ParsePage(reader, page, PAGE_SIZE, score);
                DBconn.Close();
                return pageModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
            return null;
        }

        public  static PageModel? GetCheckedItems(int userID, int said)
        {
            try
            {
                string query = string.Format("SELECT I.*, Ch.Score FROM Item AS I " +
                               "LEFT JOIN (SELECT * FROM Review WHERE UserID = {0} AND SAID = {1} AND IsApp = 1) AS Ch ON I.ItemID = Ch.ItemID " +
                               "WHERE I.ItemID IN (SELECT ItemID FROM Review WHERE UserID = {0} AND SAID = {1} AND IsApp = 1) AND I.SAID = {1}" +
                               "ORDER BY ItemID", userID, said);
                if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
                SqlCommand command = new SqlCommand(query, DBconn);
                SqlDataReader reader = command.ExecuteReader();
				PageModel pageModel;
				pageModel = ModelParser.ParsePage(reader, -1, -1, said);
                DBconn.Close();
                return pageModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
            return null;
        }
        public  static PageModel? GetRecomendedItems(int userID, int page, int said)
		{
			try
            {
                int score =  GetSubjectRangeScore(said);
                string query = string.Format("SELECT I.*, Ch.Score, Rec.Score AS RecScore FROM Item AS I " +
							   "LEFT JOIN (SELECT * FROM Review WHERE UserID = {2} AND SAID = {3} AND IsApp = 1) AS Ch ON I.ItemID = Ch.ItemID AND I.SAID = Ch.SAID " +
							   "LEFT JOIN (SELECT * FROM Recommendation WHERE UserID = {2}) AS Rec ON I.ItemID = Rec.ItemID AND I.SAID = Rec.SAID " +
							   "WHERE I.ItemID IN (SELECT ItemID FROM Recommendation WHERE UserID = {2} AND SAID = {3}) " +
                               "ORDER BY RecScore DESC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", PAGE_SIZE * (page - 1), PAGE_SIZE + 1, userID, said);
                if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
				SqlCommand command = new SqlCommand(query, DBconn);
				SqlDataReader reader = command.ExecuteReader();
				PageModel pageModel = ModelParser.ParsePage(reader, page, PAGE_SIZE, said);
				DBconn.Close();
				return pageModel;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
			return null;
		}
        public  static PageModel? GetRecomendedItems(int userID, string search, int page, int said)
		{
			try
            {
                int score =  GetSubjectRangeScore(said);
                string searchLow = search.ToLower();
                string query = string.Format("SELECT I.*, Ch.Score FROM Item AS I " +
							   "LEFT JOIN (SELECT * FROM Review WHERE UserID = {3} AND SAID = {4} AND IsApp = 1) AS Ch ON I.ItemID = Ch.ItemID " +
							   "WHERE LOWER(Title) LIKE '%{2}%' AND " +
							   "I.ItemID IN(SELECT ItemID FROM Recommendation WHERE UserID = {3} AND SAID = {4}) " +
							   "ORDER BY ItemID OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", PAGE_SIZE * (page - 1), PAGE_SIZE + 1, searchLow, userID, said);
				if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
				SqlCommand command = new SqlCommand(query, DBconn);
				SqlDataReader reader = command.ExecuteReader();
				PageModel pageModel = ModelParser.ParsePage(reader, page, PAGE_SIZE, score);
				DBconn.Close();
				return pageModel;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
			return null;
		}

        public  static void SetCheckedItem(int gameID, int userID, int said, int score)
		{
			try
			{
                UnsetCheckedItem(gameID, userID, said);
                if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
				string query = string.Format("INSERT INTO Review(UserID, ItemID, Score, SAID, IsApp) VALUES({0}, {1}, {2}, {3}, 1)", userID, gameID, score, said);
				SqlCommand command = new SqlCommand(query, DBconn);
				command.ExecuteNonQuery();
				DBconn.Close();
			}
			catch (SqlException ex)
			{
				Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
		}
        public  static void UnsetCheckedItem(int gameID, int userID, int said)
		{
			try
			{
				if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
				string query = string.Format("DELETE FROM Review WHERE ItemID = {0} AND UserID = {1} AND SAID = {2} AND IsApp = 1", gameID, userID, said);
				SqlCommand command = new SqlCommand(query, DBconn);
				command.ExecuteNonQuery();
				DBconn.Close();
			}
			catch (SqlException ex)
			{
				Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
		}

        public  static int GetCheckedItemsCount(int userID, int said)
		{
			try
			{
				if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
				string query = string.Format("SELECT COUNT(ItemID) FROM Review WHERE UserID = {0} AND SAID = {1} AND IsApp = 1", userID, said);
				SqlCommand command = new SqlCommand(query, DBconn);
				int gamesCount = (int)command.ExecuteScalar();
				DBconn.Close();
				return gamesCount;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
			return 0;
		}

        public static List<ReviewModel> GetReviewsByUsers(int userID, Dictionary<int, int> checkedGames, ref Dictionary<string, double> userSim, int said)
		{
			try
			{
				if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
				string query = string.Format("SELECT R.*, I.Genre FROM Review AS R " +
					"LEFT JOIN (SELECT * FROM Item WHERE SAID = {1}) AS I ON R.ItemID = I.ItemID " +
					"WHERE UserID IN(SELECT UserID FROM Review " +
					"WHERE ItemID IN (SELECT ItemID FROM Review WHERE UserID = {0} AND SAID = {1}) AND SAID = {1}) AND R.SAID = {1}", userID, said);
				SqlCommand command = new SqlCommand(query, DBconn);
				SqlDataReader reader = command.ExecuteReader();
				List<ReviewModel> reviewModels = ModelParser.ParseReviews(reader, checkedGames, ref userSim);
				DBconn.Close();
				return reviewModels;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
			return new List<ReviewModel>();
		}
        public static List<ReviewModel> GetReviewsByGenre(int userID, Dictionary<int, int> checkedGames, ref Dictionary<string, double> userSim, int said)
		{
			try
			{
				List<string> genres = GetGenres(userID, said);
				if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
				string query = string.Format("SELECT TOP 50000 R.*, I.Genre " +
							"FROM Review AS R LEFT JOIN Item AS I ON R.ItemID = I.ItemID " +
							"WHERE ItemID IN (SELECT ItemID FROM Item WHERE (Genre LIKE '%{0}%'", genres[0]);
				for (int i = 1; i < genres.Count; i++)
					query += string.Format(" OR Genre LIKE '%{0}%'", genres[i]);
				query += string.Format(")) AND SAID = {0} ORDER BY Assists DESC", said);
				SqlCommand command = new SqlCommand(query, DBconn);
				SqlDataReader reader = command.ExecuteReader();
				List<ReviewModel> reviewModels = ModelParser.ParseReviews(reader, checkedGames, ref userSim);
				DBconn.Close();
				return reviewModels;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
			return new List<ReviewModel>();
		}

        public static List<string> GetGenres(int userID, int said)
		{
			try
			{
				if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
				string query = string.Format("SELECT Genre FROM Item " +
					"WHERE ItemID IN (SELECT ItemID FROM Review WHERE UserID = {0} AND SAID = {1} AND IsApp = 1)", userID, said);
				SqlCommand command = new SqlCommand(query, DBconn);
				SqlDataReader reader = command.ExecuteReader();
				List<string> genres = ModelParser.ParseGenre(reader);
				DBconn.Close();
				return genres;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
			return new List<string>();
		}
        public  static void SetRecommendations(int userID, List<KeyValuePair<int, float>> game_score, int said)
		{
			try
			{
				if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
				string query = string.Format("DELETE FROM Recommendation WHERE UserID = {0} AND SAID = {1}", userID, said);
				SqlCommand command = new SqlCommand(query, DBconn);
				command.ExecuteNonQuery();
				DBconn.Close();

				DBconn.Open();
				int count = game_score.Count - 1, limit = 120;
				query = "INSERT INTO Recommendation(UserID, ItemID, Score, SAID) VALUES";
				for (int i = 0; i < count; i++)
				{
					query += string.Format(" ({0}, {1}, {2}, {3}),", userID, game_score[i].Key, game_score[i].Value.ToString(CultureInfo.InvariantCulture), said);
					limit--;
                    if (limit == 1)
                    {
                        break;
                    }
				}
				query += string.Format(" ({0}, {1}, {2}, {3})", userID, game_score[count].Key, game_score[count].Value.ToString(CultureInfo.InvariantCulture), said);
				command = new SqlCommand(query, DBconn);
				command.ExecuteNonQuery();

				DBconn.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
		}
	}
}
