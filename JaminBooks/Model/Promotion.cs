using JaminBooks.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using static JaminBooks.Tools.SQL;

namespace JaminBooks.Model
{
    /// <summary>
    /// Models a book, discount code, or order total promotion
    /// </summary>
    public class Promotion
    {
        /// <summary>
        /// The unique id number that identifies the promotion. -1 represent an uncreated promotion.
        /// </summary>
        public int PromotionID { private set; get; } = -1;

        /// <summary>
        /// The start date of the promotion.
        /// </summary>
        public DateTime StartDate;

        /// <summary>
        /// The end date of the promotion.
        /// </summary>
        public DateTime EndDate;

        /// <summary>
        /// The percent discount of the promotion.
        /// </summary>
        public int PercentDiscount;

        /// <summary>
        /// The total order value needed to earn the promotion. (Optional)
        /// </summary>
        public decimal? Total = null;

        /// <summary>
        /// The coupon code that acquires the promotion (Optional)
        /// </summary>
        public string Code = null;

        /// <summary>
        /// The id of the book with the promotion (Optional)
        /// </summary>
        public int? BookID = null;

        /// <summary>
        /// Get the book with the promotion. returns null if the promotion is not a book promotion.
        /// </summary>
        public Book Book
        {
            get
            {
                return BookID != null ? new Book(BookID.Value) : null;
            }
        }

        /// <summary>
        /// Initialize an empty promotion with default values.
        /// </summary>
        public Promotion() { }

        /// <summary>
        /// Initialize a promotion and set the equal to the promotion in the database with the given id.
        /// </summary>
        /// <param name="PromotionID">The promotion's id</param>
        public Promotion(int PromotionID)
        {
            DataTable dt = SQL.Execute("uspGetPromotionByID", new Param("PromotionID", PromotionID));

            if (dt.Rows.Count > 0)
            {
                this.PromotionID = PromotionID;
                this.StartDate = (DateTime)dt.Rows[0]["StartDate"];
                this.EndDate = (DateTime)dt.Rows[0]["EndDate"];
                this.PercentDiscount = (int)dt.Rows[0]["PercentDiscount"];
                this.Total = dt.Rows[0]["Total"] == DBNull.Value ? null : (decimal?)dt.Rows[0]["Total"];
                this.BookID = dt.Rows[0]["BookID"] == DBNull.Value ? null : (int?)dt.Rows[0]["BookID"];
                this.Code = dt.Rows[0]["Code"] == DBNull.Value ? null : (string)dt.Rows[0]["Code"];
            }
            else
            {
                throw new Exception("Invalid Promotion ID");
            }
        }

        /// <summary>
        /// Initialize a promotion and set its fields equal to the given parameters.
        /// </summary>
        /// <param name="PromotionID">The promotion's id</param>
        /// <param name="StartDate">The promotion's start date</param>
        /// <param name="EndDate">The promotion's end date</param>
        /// <param name="PercentDiscount">The promotion's discount</param>
        /// <param name="Total">The necessary total</param>
        /// <param name="Code">The coupon code</param>
        /// <param name="BookID">The id of the book with the promotion</param>
        private Promotion(int PromotionID, DateTime StartDate, DateTime EndDate, int PercentDiscount, decimal? Total, string Code, int? BookID)
        {
            this.PromotionID = PromotionID;
            this.StartDate = StartDate;
            this.EndDate = EndDate;
            this.PercentDiscount = PercentDiscount;
            this.Total = Total;
            this.Code = Code;
            this.BookID = BookID;
        }

        /// <summary>
        /// Save the promotion to the database.
        /// </summary>
        public void Save()
        {
            DataTable dt = SQL.Execute("uspSavePromotion",
                new Param("PromotionID", PromotionID),
                new Param("StartDate", StartDate),
                new Param("EndDate", EndDate),
                new Param("PercentDiscount", PercentDiscount),
                new Param("Total", Total ?? SqlDecimal.Null),
                new Param("Code", Code ?? SqlString.Null),
                new Param("BookID", BookID ?? SqlInt32.Null));

            if (dt.Rows.Count > 0)
                PromotionID = (int)dt.Rows[0]["PromotionID"];
        }

        /// <summary>
        /// Delete the promotion from the database and set its id to -1.
        /// </summary>
        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeletePromotion", new Param("PromotionID", PromotionID));
            PromotionID = -1;
        }

        /// <summary>
        /// Delete any promotions on the given book.
        /// </summary>
        /// <param name="BookID">The book's id</param>
        public static void DeletePromotions(int BookID)
        {
            DataTable dt = SQL.Execute("uspDeletePromotionByBook", new Param("BookID", BookID));
        }

        /// <summary>
        /// Delete an promotions past their end date from the given book.
        /// </summary>
        /// <param name="BookID">The book's id</param>
        public static void DeleteExpiredPromotions(int BookID)
        {
            DataTable dt = SQL.Execute("uspDeleteExpiredPromotionFromBook", new Param("BookID", BookID));
        }

        /// <summary>
        /// Get a list of all promotions
        /// </summary>
        /// <returns>A list of all promotions</returns>
        public static List<Promotion> GetPromotions()
        {
            return GetPromotions(SQL.Execute("uspGetPromotions"));
        }

        /// <summary>
        /// Get a list of all promotions from the given DataTable.
        /// </summary>
        /// <param name="dt">A DataTable containing promotions</param>
        /// <returns>A list of promotions</returns>
        public static List<Promotion> GetPromotions(DataTable dt)
        {
            List<Promotion> promos = new List<Promotion>();
            foreach (DataRow dr in dt.Rows)
                promos.Add(new Promotion(
                    (int)dr["PromotionID"],
                    (DateTime)dr["StartDate"],
                    (DateTime)dr["EndDate"],
                    (int)dr["PercentDiscount"],
                    dr["Total"] == DBNull.Value ? null : (decimal?)dr["Total"],
                    dr["Code"] == DBNull.Value ? null : (string)dr["Code"],
                    dr["BookID"] == DBNull.Value ? null : (int?)dr["BookID"]));
            return promos;
        }
    }
}