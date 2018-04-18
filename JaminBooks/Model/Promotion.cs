using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using static JaminBooks.Model.SQL;

namespace JaminBooks.Model
{
    public class Promotion
    {
        public int PromotionID { private set; get; } = -1;
        public DateTime StartDate;
        public DateTime EndDate;
        public int PercentDiscount;
        public decimal? Total = null;
        public string Code = null;
        public int? BookID = null;

        public Book Book
        {
            get
            {
                return BookID != null ? new Book(BookID.Value) : null;
            }
        }

        public Promotion() { }

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

        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeletePromotion", new Param("PromotionID", PromotionID));
            PromotionID = -1;

        }

        public static void DeletePromotions(int BookID)
        {
            DataTable dt = SQL.Execute("uspDeletePromotionByBook", new Param("BookID", BookID));
        }

        public static void DeleteExpiredPromotions(int BookID)
        {
            DataTable dt = SQL.Execute("uspDeleteExpiredPromotionFromBook", new Param("BookID", BookID));
        }

        public static List<Promotion> GetPromotions()
        {
            return GetPromotions(SQL.Execute("uspGetPromotions"));
        }

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
