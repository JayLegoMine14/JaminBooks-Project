﻿using System;
using System.Data;
using static JaminBooks.Model.SQL;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JaminBooks.Model
{
    public class Rating
    {
        public int RatingID { get; private set; } = -1;

        public int RatingValue;
        public string Comment;
        public int BookID;
        public int UserID;
        public DateTime CreationDate { get; private set; } = DateTime.Now;

        public string FormatedDate
        {
            get
            {
                return CreationDate.ToString("M/d/yy htt");
            }
        }

        public string[] NameAndImage
        {
            get
            {
                User u = new User(UserID);
                var username = u.FirstName + " " + u.LastName;
                return new string[] { username, u.LoadImage };
            }
        }

        public Rating() { }

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
            }
            else
            {
                throw new Exception("Invalid Rating ID");
            }

        }

        private Rating(int RatingID, int bRating, string Comment, int BookID, int UserID, DateTime CreationDate)
        {
            this.RatingID = RatingID;
            this.RatingValue = bRating;
            this.Comment = Comment;
            this.BookID = BookID;
            this.UserID = UserID;
            this.CreationDate = CreationDate;
        }

        public void Save()
        {
            DataTable dt = SQL.Execute("uspSaveRating",
                new Param("RatingID", RatingID),
                new Param("Rating", RatingValue),
                new Param("Comment", Comment),
                new Param("BookID", BookID),
                new Param("UserID", UserID)
                );

            if (dt.Rows.Count > 0)
                RatingID = Convert.ToInt32(dt.Rows[0]["RatingID"]);
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
                    (int)dr["Rating"],
                    dr["Comment"] != DBNull.Value ? (String)dr["Comment"] : "",
                    (int)dr["BookID"],
                    (int)dr["UserID"],
                    (DateTime)dr["CreationDate"]
                    ));
            return ratings;
        }

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
                     (DateTime)dr["CreationDate"]
                    ));
            return ratings;
        }
    }
}