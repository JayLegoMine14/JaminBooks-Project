using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using static JaminBooks.Model.SQL;

namespace JaminBooks.Model
{
    public class Order
    {
        public int OrderID { private set; get; } = -1;
        public DateTime OrderDate { private set; get; }
        public DateTime? RefundDate = null;
        public DateTime? FulfilledDate =  null;
        public int PercentDiscount;
        public Card Card;
        public Address Address;
        public Dictionary<Book, dynamic> Books = new Dictionary<Book, dynamic>();

        public bool IsRefunded
        {
            get
            {
                return RefundDate != null;
            }
        }

        public bool IsFulfilled
        {
            get
            {
                return FulfilledDate != null;
            }
        }

        public decimal Total
        {
            get
            {
                decimal BookTotal = 0;

                foreach (KeyValuePair<Book, dynamic> item in Books)
                    BookTotal += item.Value.Quantity * item.Value.Price;

                return BookTotal - (BookTotal * (PercentDiscount / 100m));
            }
        }

        public Order() { }

        public Order(int OrderID) {
            DataTable dt = SQL.Execute("uspGetOrderByID", new Param("OrderID", OrderID));

            if (dt.Rows.Count > 0)
            {
                this.OrderID = OrderID;
                this.OrderDate = (DateTime)dt.Rows[0]["OrderDate"];
                this.Card = new Card((int)dt.Rows[0]["CardID"]);
                this.Address = new Address((int)dt.Rows[0]["AddressID"]);
                this.PercentDiscount = (int)dt.Rows[0]["PercentDiscount"];
                this.RefundDate = dt.Rows[0]["RefundDate"] == DBNull.Value ? null : (DateTime?)dt.Rows[0]["RefundDate"];
                this.FulfilledDate = dt.Rows[0]["FulfilledDate"] == DBNull.Value ? null : (DateTime?)dt.Rows[0]["FulfilledDate"];

                DataTable books = SQL.Execute("uspGetOrderBooksByID", new Param("OrderID", OrderID));
                foreach(DataRow book in books.Rows)
                {
                    Books.Add(new Book((int)book["BookID"]),
                        new { Price = (decimal)book["Price"], Quantity = (int)book["Quantity"], Cost = (decimal)book["Cost"] });
                }
            }
            else
            {
                throw new Exception("Invalid Order ID");
            }
        }

        public Order (int OrderID, DateTime OrderDate, int CardID, int AddressID, int PercentDiscount, DateTime? RefundDate, DateTime? FulfilledDate)
        {
            this.OrderID = OrderID;
            this.OrderDate = OrderDate;
            this.Card = new Card(CardID);
            this.Address = new Address(AddressID);
            this.PercentDiscount = PercentDiscount;
            this.FulfilledDate = FulfilledDate;
            this.RefundDate = RefundDate;

            DataTable books = SQL.Execute("uspGetOrderBooksByID", new Param("OrderID", OrderID));
            foreach (DataRow book in books.Rows)
            {
                Books.Add(new Book((int)book["BookID"]),
                    new { Price = (decimal)book["Price"], Quantity = (int)book["Quantity"], Cost = (decimal)book["Cost"] });
            }
        }

        public void Save()
        {
            DataTable dt = SQL.Execute("uspSaveOrder",
               new Param("OrderID", OrderID),
               new Param("PercentDiscount", PercentDiscount),
               new Param("CardID", Card.CardID),
               new Param("AddressID", Address.AddressID),
               new Param("FulfilledDate", FulfilledDate ?? SqlDateTime.Null),
               new Param("RefundDate", RefundDate ?? SqlDateTime.Null));

            var oldID = OrderID;
            if (dt.Rows.Count > 0)
                OrderID = (int)dt.Rows[0]["OrderID"];

            if(oldID != OrderID)
                foreach(Book b in Books.Keys)
                   SQL.Execute("uspSaveOrderBooks",
                   new Param("OrderID", OrderID),
                   new Param("BookID", b.BookID),
                   new Param("Price", Books[b].Price),
                   new Param("Quantity", Books[b].Quantity),
                   new Param("Cost", Books[b].Cost));
        }

        public override bool Equals(object obj)
        {
            if(obj is Order)
                return this.OrderID == ((Order)obj).OrderID;
            else
                return false;
        }

        public static List<Order> GetAll()
        {
            return GetOrders(SQL.Execute("uspGetOrders"));
        }

        public static List<Order> GetAllByUser(int UserID)
        {
            return GetOrders(SQL.Execute("uspGetOrdersByUser", new Param("UserID", UserID)));
        }

        public static List<Order> GetOrders(DataTable dt)
        {
            List<Order> orders = new List<Order>();
            foreach (DataRow dr in dt.Rows)
                orders.Add(new Order(
                    (int)dr["OrderID"],
                    (DateTime)dr["OrderDate"],
                    (int)dr["CardID"],
                    (int)dr["AddressID"],
                    (int)dr["PercentDiscount"],
                    dr["RefundDate"] == DBNull.Value ? null : (DateTime?)dr["RefundDate"],
                    dr["FulfilledDate"] == DBNull.Value ? null : (DateTime?)dr["FulfilledDate"]
                    ));
            return orders;
        }
    }
}
