using System;
using System.Collections.Generic;
using System.Data;
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
        public int PercentDiscount;
        public Card Card;
        public Address Address;
        public Dictionary<Book, dynamic> Books = new Dictionary<Book, dynamic>();

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

                DataTable books = SQL.Execute("uspGetOrderBooksByID", new Param("OrderID", OrderID));
                foreach(DataRow book in books.Rows)
                {
                    Books.Add(new Book((int)book["BookID"]),
                        new { Price = (decimal)book["Price"], Quantity = (int)book["Quantity"] });
                }
            }
            else
            {
                throw new Exception("Invalid Order ID");
            }
        }

        public Order (int OrderID, DateTime OrderDate, int CardID, int AddressID, int PercentDiscount)
        {
            this.OrderID = OrderID;
            this.OrderDate = OrderDate;
            this.Card = new Card(CardID);
            this.Address = new Address(AddressID);
            this.PercentDiscount = PercentDiscount;

            DataTable books = SQL.Execute("uspGetOrderBooksByID", new Param("OrderID", OrderID));
            foreach (DataRow book in books.Rows)
            {
                Books.Add(new Book((int)book["BookID"]),
                    new { Price = (decimal)book["Price"], Quantity = (int)book["Quantity"] });
            }
        }

        public void Save()
        {
            DataTable dt = SQL.Execute("uspSaveOrder",
               new Param("PercentDiscount", PercentDiscount),
               new Param("CardID", Card.CardID),
               new Param("AddressID", Address.AddressID));

            if (dt.Rows.Count > 0)
                OrderID = (int)dt.Rows[0]["OrderID"];

            foreach(Book b in Books.Keys)
               SQL.Execute("uspSaveOrderBooks",
               new Param("OrderID", OrderID),
               new Param("BookID", b.BookID),
               new Param("Price", Books[b].Price),
               new Param("Quantity", Books[b].Quantity));
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
                    (int)dr["PercentDiscount"]
                    ));
            return orders;
        }
    }
}
