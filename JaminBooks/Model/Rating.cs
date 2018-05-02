using JaminBooks.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static JaminBooks.Tools.SQL;

namespace JaminBooks.Model
{
    /// <summary>
    /// Models a book rating
    /// </summary>
    public class Rating
    {
        /// <summary>
        /// The unique id number that identifies the rating. -1 represents an uncreated rating.
        /// </summary>
        public int RatingID { get; private set; } = -1;

        /// <summary>
        /// The value of the rating. Between 0-5.
        /// </summary>
        public int RatingValue;

        /// <summary>
        /// The comment on the rating.
        /// </summary>
        public string Comment;

        /// <summary>
        /// The id of the book on which the rating was left.
        /// </summary>
        public int BookID;

        /// <summary>
        /// The id of the user who left the rating.
        /// </summary>
        public int UserID;

        /// <summary>
        /// Whether or not the rating should be visible.
        /// </summary>
        public bool Hidden;

        /// <summary>
        /// The date on which the rating was created.
        /// </summary>
        public DateTime CreationDate { get; private set; } = DateTime.Now;

        /// <summary>
        /// A list of the id number of users who have flagged this rating.
        /// </summary>
        private List<int> Flags = new List<int>();

        /// <summary>
        /// Get a list of the id number of the users who have flagged this rating.
        /// </summary>
        public List<int> FlagUsers
        {
            get
            {
                return Flags.ToList();
            }
        }

        /// <summary>
        /// Get the creation date of this rating formatted as "M/d/yy htt"
        /// </summary>
        public string FormatedDate
        {
            get
            {
                return CreationDate.ToString("M/d/yy htt");
            }
        }

        /// <summary>
        /// Get the name and image of the user who left this rating.
        /// </summary>
        public object[] NameAndImage
        {
            get
            {
                User u = new User(UserID);
                var username = u.FirstName + " " + u.LastName;
                return new object[] { username, u.LoadImage, u.HasBought(BookID) };
            }
        }

        /// <summary>
        /// Initialize an empty rating with default values.
        /// </summary>
        public Rating() { }

        /// <summary>
        /// Initialize a rating and set its fields equal to the rating in the database.
        /// </summary>
        /// <param name="RatingID"></param>
        public Rating(int RatingID)
        {
            DataTable dt = SQL.Execute("uspGetRatingByID", new Param("RatingID", RatingID));

            if (dt.Rows.Count > 0)
            {
                this.RatingID = RatingID;
                this.RatingValue = (int)dt.Rows[0]["Rating"];
                this.Comment = dt.Rows[0]["Comment"] != DBNull.Value ? (String)dt.Rows[0]["Comment"] : "";
                this.BookID = (int)dt.Rows[0]["BookID"];
                this.UserID = (int)dt.Rows[0]["UserID"];
                this.CreationDate = (DateTime)dt.Rows[0]["CreationDate"];
                this.Hidden = (bool)dt.Rows[0]["Hidden"];

                DataTable flags = SQL.Execute("uspGetFlagsByRating", new Param("RatingID", RatingID));
                foreach (DataRow dr in flags.Rows)
                {
                    this.Flags.Add((int)dr["UserID"]);
                }
            }
            else
            {
                throw new Exception("Invalid Rating ID");
            }
        }

        /// <summary>
        /// Initialize a rating and set its fields equal to the value of the given parameters.
        /// </summary>
        /// <param name="RatingID">the rating's id</param>
        /// <param name="bRating">The rating's star rating</param>
        /// <param name="Comment">The rating's comment</param>
        /// <param name="BookID">The id of the rating's book</param>
        /// <param name="UserID">The id of the rating's creator</param>
        /// <param name="CreationDate">The date on which the rating was created</param>
        /// <param name="Hidden">Whether or not the rating should be hidden</param>
        private Rating(int RatingID, int bRating, string Comment, int BookID, int UserID, DateTime CreationDate, bool Hidden)
        {
            this.RatingID = RatingID;
            this.RatingValue = bRating;
            this.Comment = Comment;
            this.BookID = BookID;
            this.UserID = UserID;
            this.CreationDate = CreationDate;
            this.Hidden = Hidden;

            DataTable flags = SQL.Execute("uspGetFlagsByRating", new Param("RatingID", RatingID));
            foreach (DataRow dr in flags.Rows)
            {
                this.Flags.Add((int)dr["UserID"]);
            }
        }

        /// <summary>
        /// Save the rating to the database.
        /// </summary>
        public void Save()
        {
            DataTable dt = SQL.Execute("uspSaveRating",
                new Param("RatingID", RatingID),
                new Param("Rating", RatingValue),
                new Param("Comment", Comment),
                new Param("BookID", BookID),
                new Param("UserID", UserID),
                new Param("Hidden", Hidden)
                );

            if (dt.Rows.Count > 0)
                RatingID = Convert.ToInt32(dt.Rows[0]["RatingID"]);
        }

        /// <summary>
        /// Delete the rating from the database, clear its flags, and set its id to -1.
        /// </summary>
        public void Delete()
        {
            DeleteFlags();
            DataTable dt = SQL.Execute("uspDeleteRating", new Param("RatingID", RatingID));
            RatingID = -1;
        }

        /// <summary>
        /// Add a flag to the rating.
        /// </summary>
        /// <param name="userID">The user who flagged the rating</param>
        public void AddFlag(int userID)
        {
            SQL.Execute("uspSaveFlag",
                new Param("UserID", userID),
                new Param("RatingID", RatingID));
        }

        /// <summary>
        /// Delete the flags on this rating.
        /// </summary>
        public void DeleteFlags()
        {
            SQL.Execute("uspDeleteFlags", new Param("RatingID", RatingID));
        }

        /// <summary>
        /// Check whether the given user has already flagged the rating.
        /// </summary>
        /// <param name="userID">The id of the user who flagged this rating</param>
        /// <returns>Whether or not the user has flagged the rating </returns>
        public bool hasFlagged(int userID)
        {
            return Flags.Contains(userID);
        }

        /// <summary>
        /// Get a list of ratings on the given book.
        /// </summary>
        /// <param name="BookID">The book's id</param>
        /// <returns>A list of ratings</returns>
        public static List<Rating> GetRatings(int BookID)
        {
            DataTable dt = SQL.Execute("uspGetRating", new Param("BookID", BookID));
            List<Rating> ratings = new List<Rating>();
            foreach (DataRow dr in dt.Rows)
                ratings.Add(new Rating(
                    (int)dr["RatingID"],
                    (int)dr["Rating"],
                    dr["Comment"] != DBNull.Value ? (String)dr["Comment"] : "",
                    (int)dr["BookID"],
                    (int)dr["UserID"],
                    (DateTime)dr["CreationDate"],
                    (bool)dr["Hidden"]
                    ));
            return ratings;
        }

        /// <summary>
        /// Get a list of flagged ratings.
        /// </summary>
        /// <returns>A list of ratings</returns>
        public static List<Rating> GetFlagged()
        {
            DataTable dt = SQL.Execute("uspGetFlagged");
            List<Rating> ratings = new List<Rating>();
            foreach (DataRow dr in dt.Rows)
                ratings.Add(new Rating(
                    (int)dr["RatingID"],
                    (int)dr["Rating"],
                    dr["Comment"] != DBNull.Value ? (String)dr["Comment"] : "",
                    (int)dr["BookID"],
                    (int)dr["UserID"],
                    (DateTime)dr["CreationDate"],
                    (bool)dr["Hidden"]
                    ));
            return ratings;
        }

        /// <summary>
        /// Get all ratings made by the given user.
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>A list of ratings</returns>
        public static List<Rating> GetRatingsByUser(User user)
        {
            DataTable dt = SQL.Execute("uspGetRatingByUser", new Param("UserID", user.UserID));
            List<Rating> ratings = new List<Rating>();
            foreach (DataRow dr in dt.Rows)
                ratings.Add(new Rating(
                    (int)dr["RatingID"],
                    (int)dr["Rating"],
                    dr["Comment"] != DBNull.Value ? (String)dr["Comment"] : "",
                    (int)dr["BookID"],
                    (int)dr["UserID"],
                     (DateTime)dr["CreationDate"],
                     (bool)dr["Hidden"]
                    ));
            return ratings;
        }
    }
}