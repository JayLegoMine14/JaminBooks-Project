using System;
using System.Data;
using static JaminBooks.Model.SQL;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JaminBooks.Model
{
    public class Rating
    {
        public int RatingID { private get; set; } = -1;

        public int bRating;
        public string Comment;
        public int BookID;
        public int UserID;

        public Rating() { }

        public Rating(int RatingID)
        {
            DataTable dt = SQL.Execute("uspGetRatingByID", new Param("RatingID", RatingID));

            if (dt.Rows.Count > 0)
            {
                this.RatingID = RatingID;
                this.bRating = (int)dt.Rows[0]["Rating"];
                this.Comment = (String)dt.Rows[0]["Comment"];
                this.BookID = (int)dt.Rows[0]["BookID"];
                this.UserID = (int)dt.Rows[0]["UserID"];
            }
            else
            {
                throw new Exception("Invalid Rating ID");
            }

        }

        private Rating(int RatingID, int bRating, string Comment, int BookID, int UserID)
        {
            this.RatingID = RatingID;
            this.bRating = bRating;
            this.Comment = Comment;
            this.BookID = BookID;
            this.UserID = UserID;
        }

        public void Save()
        {
            DataTable dt = SQL.Execute("uspSaveRating",
                new Param("AuthorID", RatingID),
                new Param("FirstName", bRating),
                new Param("LastName", Comment),
                new Param("IsDeleted", BookID),
                new Param("IsDeleted", UserID)
                );
        }

        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeleteRating", new Param("RatingID", RatingID));
            RatingID = -1;
        }

        public static List<Rating> GetRatings(int BookID)
        {
            DataTable dt = SQL.Execute("uspGetRating", new Param("BookID", BookID));
            List<Rating> ratings = new List<Rating>();
            foreach (DataRow dr in dt.Rows)
                ratings.Add(new Rating(
                    (int)dr["RatingID"],
                    (int)dr["bRating"],
                    (String)dr["Comment"],
                    (int)dr["BookID"],
                    (int)dr["UserID"]
                    ));
            return ratings;
        }
    }
}