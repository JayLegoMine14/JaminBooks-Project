//This is a copy of book but I will use it for rating

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
        public int RatingID { private set; get; } = -1;
        public int RatingVal;
        public string Comment;
        public int BookID;
        public int UserID;

        public Rating() { }

        public Rating(int BookID)
        {
            DataTable dt = SQL.Execute("uspGetRating", new Param("Rating", RatingVal));
            if (dt.Rows.Count > 0)
            {
                this.BookID = BookID;
                this.RatingVal = (int)dt.Rows[0]["Rating"];
                this.Comment = (string)dt.Rows[0]["Comment"];
                this.UserID = (int)dt.Rows[0]["UserID"];
                            }
            else
            {
                throw new Exception("Invalid User ID");
            }
        }

        public void Save()
        {
            DataTable dt = SQL.Execute("uspSaveRating",
                new Param("BookID", BookID),
                new Param("Rating", RatingVal),
                new Param("Comment", Comment),
                new Param("UserID", UserID));

            if (dt.Rows.Count > 0)
            {
                BookID = (int)dt.Rows[0]["BookID"];
            }
        }

        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeleteRating", new Param("RatingID", RatingID));
            RatingID = -1;
        }
    }
}