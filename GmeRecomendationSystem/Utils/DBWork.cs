using Microsoft.Data.SqlClient;
using System.Globalization;
using GmeRecomendationSystem.Models;

namespace GmeRecomendationSystem.Utils
{
	internal static class DBWork
	{
		private const int PAGE_SIZE = 12;
		private static SqlConnection DBconn = new SqlConnection("Data Source=SQL6031.site4now.net;Initial Catalog=db_a99fa3_urs;User Id=db_a99fa3_urs_admin;Password=35Jey_XL35");

        private static SqlDataReader? ExecReader(string query)
        {
            try
            {
                if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
                SqlCommand command = new SqlCommand(query, DBconn);
                return command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
            return null;
        }
        private static object? ExecScalar(string query)
        {
            try
            {
                if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
                SqlCommand command = new SqlCommand(query, DBconn);
                return command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
            return null;
        }
        private static void ExecNonQuery(string query)
        {
            try
            {
                if (DBconn.State == System.Data.ConnectionState.Closed)
                    DBconn.Open();
                SqlCommand command = new SqlCommand(query, DBconn);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
            }
        }

        public  static SubjectRangeModel GetSubjectRange()
		{
            string query = string.Format("SELECT TOP 1 * FROM SubjectRange ORDER BY SAID");
            SqlDataReader reader = ExecReader(query);
            if(reader is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return new SubjectRangeModel();
            }
            List<SubjectRangeModel> SA = ModelParser.ParseSA(reader);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
            return SA[0];
        }
		public  static SubjectRangeModel GetSubjectRange(int said)
        {
            string query = string.Format("SELECT * FROM SubjectRange WHERE SAID = {0}", said);
            SqlDataReader reader = ExecReader(query);
            if (reader is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return new SubjectRangeModel();
            }
            List<SubjectRangeModel> SA = ModelParser.ParseSA(reader);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
            return SA[0];
        }
        public  static List<SubjectRangeModel> GetSubjectRanges(bool admin = false)
        {
            string query;
            if(admin)
                query = string.Format("SELECT * FROM SubjectRange");
            else
                query = string.Format("SELECT * FROM SubjectRange WHERE InWork = 1");
            SqlDataReader reader = ExecReader(query);
            if (reader is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return new List<SubjectRangeModel>();
            }
            List<SubjectRangeModel> SA = ModelParser.ParseSA(reader);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
            return SA;
        }
        public  static int GetSubjectRangeScore(int said)
		{
            string query = string.Format("SELECT ScoreRange FROM SubjectRange WHERE SAID = {0}", said);
            SqlCommand command = new SqlCommand(query, DBconn);
            SqlDataReader reader = ExecReader(query);
            if (reader is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return -1;
            }
            reader.Read();
			int score = reader.GetInt32(0);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
            return score;
        }
        public  static void AddSubjectRange(SubjectRangeModel model)
        {
            string query = string.Format("INSERT INTO SubjectRange(SubjectName, ScoreRange, ScoreK, AssistsK, WeightK, UseTimeK, InWork) " +
										"VALUES('{0}', {1}, {2}, {3}, {4}, {5}, 0)", model.Name, model.Score, model.ScoreK.ToString(CultureInfo.InvariantCulture), model.AssistsK.ToString(CultureInfo.InvariantCulture), model.WeightK.ToString(CultureInfo.InvariantCulture), model.UseTimeK.ToString(CultureInfo.InvariantCulture));
            SqlCommand command = new SqlCommand(query, DBconn);
            ExecNonQuery(query);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
        }
        public  static void SetSubjectRangeUse(int said, int set)
        {
            string query = string.Format("UPDATE SubjectRange SET InWork = {0} WHERE SAID = {1}", set, said);
            ExecNonQuery(query);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
        }

        public  static ItemModel GetItem(int itemID, int said, int userID)
		{
			int score =  GetSubjectRangeScore(said);
			string query  = string.Format("SELECT I.*, Ch.Score, Ch.ScoreRange FROM Item AS I " +
						"LEFT JOIN (SELECT R.*, SR.ScoreRange FROM Review AS R LEFT JOIN SubjectRange AS SR ON R.SAID = SR.SAID " +
						"WHERE UserID = {1} AND R.SAID = {2} AND IsApp = 1) AS Ch ON I.ItemID = Ch.ItemID " +
                           "WHERE I.ItemID = {0} AND I.SAID = {2}", itemID, userID, said);
			SqlDataReader reader = ExecReader(query);
            if (reader is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return new ItemModel();
            }
            PageModel page = ModelParser.ParsePage(reader, 0, 0, score);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
			return page.Items[0];
		}

        public  static PageModel? GetLibraryItems(int page, int userID, int said)
		{
            int score =  GetSubjectRangeScore(said);
            string query = string.Format("SELECT I.*, Ch.Score FROM Item AS I " +
						"LEFT JOIN (SELECT * FROM Review WHERE UserID = {2} AND SAID = {3} AND IsApp = 1) AS Ch ON I.ItemID = Ch.ItemID " +
                        "WHERE I.SAID = {3} " +
						"ORDER BY ItemID OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", PAGE_SIZE * (page - 1), PAGE_SIZE + 1, userID, said);
			SqlDataReader reader = ExecReader(query);
            if (reader is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return null;
            }
            PageModel pageModel = ModelParser.ParsePage(reader, page, PAGE_SIZE, score);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
			return pageModel;
		}
        public  static PageModel? GetLibraryItems(string search, int page, int userID, int said)
		{
            int score =  GetSubjectRangeScore(said);
            string searchLow = search.ToLower();
			string query = string.Format("SELECT I.*, Ch.Score FROM Item AS I " +
						"LEFT JOIN (SELECT * FROM Review WHERE UserID = {3} AND SAID = {4} AND IsApp = 1) AS Ch ON I.ItemID = Ch.ItemID " +
						"WHERE LOWER(Title) LIKE '%{2}%' AND I.SAID = {4}" +
                        "ORDER BY ItemID OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", PAGE_SIZE * (page - 1), PAGE_SIZE + 1, searchLow, userID, said);
			SqlDataReader reader = ExecReader(query);
            if (reader is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return null;
            }
            PageModel pageModel = ModelParser.ParsePage(reader, page, PAGE_SIZE, score);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
			return pageModel;

		}

        public  static UserModel? LogIn(string email, string password)
		{
			string query = string.Format("SELECT * FROM AppAccount WHERE Email = '{0}'", email);
			SqlDataReader reader = ExecReader(query);
            if (reader is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return null;
            }
            UserModel userModel = ModelParser.ParseUser(reader, password);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
			return userModel;
		}
        public  static UserModel Registrate(string username, string email, string password)
		{
			string query = string.Format("INSERT INTO AppAccount(Nickname, Email, UserPassword, UserRole) VALUES('{0}', '{1}', '{2}', 'user')", username, email, password);
			ExecNonQuery(query);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
			query = string.Format("SELECT * FROM AppAccount WHERE Email = '{0}'", email);
			SqlDataReader reader = ExecReader(query);
            if (reader is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return new UserModel();
            }
            UserModel userModel = ModelParser.ParseUser(reader);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
			return userModel;
		}
        public  static int? CheckEmailExistence(string email)
		{
			string query = string.Format("SELECT COUNT(UserID) FROM AppAccount WHERE Email = '{0}'", email);
			int? count = (int?)ExecScalar(query);
            if (count is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return -1;
            }
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
			return count;
		}

		public  static PageModel? GetCheckedItems(int userID, int page, int said)
        {
            int score =  GetSubjectRangeScore(said);
            string query = string.Format("SELECT I.*, Ch.Score FROM Item AS I " +
                           "LEFT JOIN (SELECT * FROM Review WHERE UserID = {2} AND SAID = {3} AND IsApp = 1) AS Ch ON I.ItemID = Ch.ItemID " +
                           "WHERE I.ItemID IN (SELECT ItemID FROM Review WHERE UserID = {2} AND SAID = {3} AND IsApp = 1) AND I.SAID = {3}" +
                           "ORDER BY ItemID OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", PAGE_SIZE * (page - 1), PAGE_SIZE + 1, userID, said);
            SqlDataReader reader = ExecReader(query);
            if (reader is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return null;
            }
            PageModel pageModel = ModelParser.ParsePage(reader, page, PAGE_SIZE, score);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
            return pageModel;
        }
        public  static PageModel? GetCheckedItems(int userID, string search, int page, int said)
        {
            int score =  GetSubjectRangeScore(said);
            string searchLow = search.ToLower();
            string query = string.Format("SELECT I.*, Ch.Score FROM Item AS I " +
			   "LEFT JOIN (SELECT * FROM Review WHERE UserID = {3} AND SAID = {4} AND IsApp = 1) AS Ch ON I.ItemID = Ch.ItemID " +
                           "WHERE LOWER(Title) LIKE '%{2}%' AND I.SAID = {4} AND " +
                           "I.ItemID IN(SELECT ItemID FROM Review WHERE UserID = {3} AND SAID = {4} AND IsApp = 1) " +
                           "ORDER BY ItemID OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", PAGE_SIZE * (page - 1), PAGE_SIZE + 1, searchLow, userID, said);
            SqlDataReader reader = ExecReader(query);
            if (reader is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return null;
            }
            PageModel pageModel = ModelParser.ParsePage(reader, page, PAGE_SIZE, score);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
            return pageModel;
        }

        public  static PageModel? GetCheckedItems(int userID, int said)
        {
            string query = string.Format("SELECT I.*, Ch.Score FROM Item AS I " +
                           "LEFT JOIN (SELECT * FROM Review WHERE UserID = {0} AND SAID = {1} AND IsApp = 1) AS Ch ON I.ItemID = Ch.ItemID " +
                           "WHERE I.ItemID IN (SELECT ItemID FROM Review WHERE UserID = {0} AND SAID = {1} AND IsApp = 1) AND I.SAID = {1}" +
                           "ORDER BY ItemID", userID, said);
            SqlDataReader reader = ExecReader(query);
            if (reader is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return null;
            }
            PageModel pageModel = ModelParser.ParsePage(reader, -1, -1, said);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
            return pageModel;
        }
        public  static PageModel? GetRecomendedItems(int userID, int page, int said)
		{
            int score =  GetSubjectRangeScore(said);
            string query = string.Format("SELECT I.*, Ch.Score, Rec.Score AS RecScore FROM Item AS I " +
			   "LEFT JOIN (SELECT * FROM Review WHERE UserID = {2} AND SAID = {3} AND IsApp = 1) AS Ch ON I.ItemID = Ch.ItemID AND I.SAID = Ch.SAID " +
			   "LEFT JOIN (SELECT * FROM Recommendation WHERE UserID = {2}) AS Rec ON I.ItemID = Rec.ItemID AND I.SAID = Rec.SAID " +
			   "WHERE I.ItemID IN (SELECT ItemID FROM Recommendation WHERE UserID = {2} AND SAID = {3}) " +
                           "ORDER BY RecScore DESC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", PAGE_SIZE * (page - 1), PAGE_SIZE + 1, userID, said);
			SqlDataReader reader = ExecReader(query);
            if (reader is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return null;
            }
            PageModel pageModel = ModelParser.ParsePage(reader, page, PAGE_SIZE, said);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
			return pageModel;
		}
        public  static PageModel? GetRecomendedItems(int userID, string search, int page, int said)
		{
            int score =  GetSubjectRangeScore(said);
            string searchLow = search.ToLower();
            string query = string.Format("SELECT I.*, Ch.Score FROM Item AS I " +
					   "LEFT JOIN (SELECT * FROM Review WHERE UserID = {3} AND SAID = {4} AND IsApp = 1) AS Ch ON I.ItemID = Ch.ItemID " +
					   "WHERE LOWER(Title) LIKE '%{2}%' AND " +
					   "I.ItemID IN(SELECT ItemID FROM Recommendation WHERE UserID = {3} AND SAID = {4}) " +
					   "ORDER BY ItemID OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", PAGE_SIZE * (page - 1), PAGE_SIZE + 1, searchLow, userID, said);
			SqlDataReader reader = ExecReader(query);
            if (reader is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return null;
            }
            PageModel pageModel = ModelParser.ParsePage(reader, page, PAGE_SIZE, score);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
			return pageModel;
		}

        public  static void SetCheckedItem(int gameID, int userID, int said, int score)
		{
            UnsetCheckedItem(gameID, userID, said);
			string query = string.Format("INSERT INTO Review(UserID, ItemID, Score, SAID, IsApp) VALUES({0}, {1}, {2}, {3}, 1)", userID, gameID, score, said);
			ExecNonQuery(query);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
		}
        public  static void UnsetCheckedItem(int gameID, int userID, int said)
		{
			string query = string.Format("DELETE FROM Review WHERE ItemID = {0} AND UserID = {1} AND SAID = {2} AND IsApp = 1", gameID, userID, said);
			ExecNonQuery(query);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
		}

        public  static int? GetCheckedItemsCount(int userID, int said)
		{
			string query = string.Format("SELECT COUNT(ItemID) FROM Review WHERE UserID = {0} AND SAID = {1} AND IsApp = 1", userID, said);
			int? gamesCount = (int?)ExecScalar(query);
            if (gamesCount is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return 0;
            }
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
			return gamesCount;
		}

        public static List<ReviewModel> GetReviewsByUsers(int userID, Dictionary<int, int> checkedGames, ref Dictionary<string, double> userSim, int said)
		{
			string query = string.Format("SELECT R.*, I.Genre FROM Review AS R " +
				"LEFT JOIN (SELECT * FROM Item WHERE SAID = {1}) AS I ON R.ItemID = I.ItemID " +
				"WHERE UserID IN(SELECT UserID FROM Review " +
				"WHERE ItemID IN (SELECT ItemID FROM Review WHERE UserID = {0} AND SAID = {1}) AND SAID = {1}) AND R.SAID = {1}", userID, said);
			SqlDataReader reader = ExecReader(query);
            if (reader is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return new List<ReviewModel>();
            }
            List<ReviewModel> reviewModels = ModelParser.ParseReviews(reader, checkedGames, ref userSim);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
			return reviewModels;
		}
        public static List<ReviewModel> GetReviewsByGenre(int userID, Dictionary<int, int> checkedGames, ref Dictionary<string, double> userSim, int said)
		{
			List<string> genres = GetGenres(userID, said);
			string query = string.Format("SELECT TOP 50000 R.*, I.Genre " +
						"FROM Review AS R LEFT JOIN Item AS I ON R.ItemID = I.ItemID " +
						"WHERE ItemID IN (SELECT ItemID FROM Item WHERE (Genre LIKE '%{0}%'", genres[0]);
			for (int i = 1; i < genres.Count; i++)
				query += string.Format(" OR Genre LIKE '%{0}%'", genres[i]);
			query += string.Format(")) AND SAID = {0} ORDER BY Assists DESC", said);
			SqlDataReader reader = ExecReader(query);
            if (reader is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return new List<ReviewModel>();
            }
            List<ReviewModel> reviewModels = ModelParser.ParseReviews(reader, checkedGames, ref userSim);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
			return reviewModels;
		}

        public static List<string> GetGenres(int userID, int said)
		{
			string query = string.Format("SELECT Genre FROM Item " +
				"WHERE ItemID IN (SELECT ItemID FROM Review WHERE UserID = {0} AND SAID = {1} AND IsApp = 1)", userID, said);
			SqlDataReader reader = ExecReader(query);
            if (reader is null)
            {
                if (DBconn.State == System.Data.ConnectionState.Open)
                    DBconn.Close();
                return new List<string>();
            }
            List<string> genres = ModelParser.ParseGenre(reader);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
			return genres;
		}
        public  static void SetRecommendations(int userID, List<KeyValuePair<int, float>> game_score, int said)
		{
			string query = string.Format("DELETE FROM Recommendation WHERE UserID = {0} AND SAID = {1}", userID, said);
			ExecNonQuery(query);
            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
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
			ExecNonQuery(query);

            if (DBconn.State == System.Data.ConnectionState.Open)
                DBconn.Close();
		}
	}
}
