using JaminBooks.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using static JaminBooks.Tools.SQL;

namespace JaminBooks.Model
{
    /// <summary>
    /// Models an order
    /// </summary>
    public class Order
    {
        /// <summary>
        /// the unique id number that identifies this order. -1 represents an unsaved Order.
        /// </summary>
        public int OrderID { private set; get; } = -1;

        /// <summary>
        /// The date on which the order was made.
        /// </summary>
        public DateTime OrderDate { private set; get; }

        /// <summary>
        /// The date on which the order was refunded.
        /// </summary>
        public DateTime? RefundDate = null;

        /// <summary>
        /// The date on which the order was fulfilled.
        /// </summary>
        public DateTime? FulfilledDate = null;

        /// <summary>
        /// The percent discount on the order.
        /// </summary>
        public int PercentDiscount;

        /// <summary>
        /// The card used to pay for the order.
        /// </summary>
        public Card Card;

        /// <summary>
        /// The shipping address of the order.
        /// </summary>
        public Address Address;

        /// <summary>
        /// The parent id of a reshipped order.
        /// </summary>
        public int? ParentOrderID;

        /// <summary>
        /// A list of books on the order.
        /// </summary>
        public Dictionary<Book, dynamic> Books = new Dictionary<Book, dynamic>();

        /// <summary>
        /// Whether or not the order has been refunded
        /// </summary>
        public bool IsRefunded
        {
            get
            {
                return RefundDate != null;
            }
        }

        /// <summary>
        /// Whether or not the order has been refunded.
        /// </summary>
        public bool IsFulfilled
        {
            get
            {
                return FulfilledDate != null;
            }
        }

        /// <summary>
        /// The total price of the order including the price of all books on the order and the discount on the order.
        /// </summary>
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

        /// <summary>
        /// A list of the id numbers of the orders which were reshipments of this order.
        /// </summary>
        public List<int> Children
        {
            get
            {
                List<int> children = new List<int>();
                foreach (DataRow dr in SQL.Execute("uspGetChildren", new Param("OrderID", this.OrderID)).Rows)
                {
                    children.Add((int)dr["OrderID"]);
                }
                return children;
            }
        }

        /// <summary>
        /// Instantiate an empty order with default values.
        /// </summary>
        public Order() { }

        /// <summary>
        /// Instantiates an order and sets the fields equal to the order in the database with the given id number.
        /// </summary>
        /// <param name="OrderID">The order's id number</param>
        public Order(int OrderID)
        {
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
                this.ParentOrderID = dt.Rows[0]["ParentOrderID"] == DBNull.Value ? null : (int?)dt.Rows[0]["ParentOrderID"];

                DataTable books = SQL.Execute("uspGetOrderBooksByID", new Param("OrderID", OrderID));
                foreach (DataRow book in books.Rows)
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

        /// <summary>
        /// Instantiate an order and set the fields equal to the given parameters.
        /// </summary>
        /// <param name="OrderID">The id number of the order</param>
        /// <param name="OrderDate">The date the order was made</param>
        /// <param name="CardID">The card used to pay for the user</param>
        /// <param name="AddressID">The id number of the shipping address</param>
        /// <param name="PercentDiscount">The percent discount on the order</param>
        /// <param name="RefundDate">The date the order was refunded</param>
        /// <param name="FulfilledDate">The date the order was fulfilled</param>
        /// <param name="ParentOrderID">The id of the parent of this order</param>
        private Order(int OrderID, DateTime OrderDate, int CardID, int AddressID, int PercentDiscount, DateTime? RefundDate, DateTime? FulfilledDate, int? ParentOrderID)
        {
            this.OrderID = OrderID;
            this.OrderDate = OrderDate;
            this.Card = new Card(CardID);
            this.Address = new Address(AddressID);
            this.PercentDiscount = PercentDiscount;
            this.FulfilledDate = FulfilledDate;
            this.RefundDate = RefundDate;
            this.ParentOrderID = ParentOrderID;

            DataTable books = SQL.Execute("uspGetOrderBooksByID", new Param("OrderID", OrderID));
            foreach (DataRow book in books.Rows)
            {
                Books.Add(new Book((int)book["BookID"]),
                    new { Price = (decimal)book["Price"], Quantity = (int)book["Quantity"], Cost = (decimal)book["Cost"] });
            }
        }

        /// <summary>
        /// Clear the id of this order to allow for recreation.
        /// </summary>
        public void ClearID()
        {
            OrderID = -1;
        }

        /// <summary>
        /// Save this order to the database.
        /// </summary>
        public void Save()
        {
            DataTable dt = SQL.Execute("uspSaveOrder",
               new Param("OrderID", OrderID),
               new Param("PercentDiscount", PercentDiscount),
               new Param("CardID", Card.CardID),
               new Param("AddressID", Address.AddressID),
               new Param("FulfilledDate", FulfilledDate ?? SqlDateTime.Null),
               new Param("RefundDate", RefundDate ?? SqlDateTime.Null),
               new Param("ParentOrderID", ParentOrderID ?? SqlInt32.Null));

            var oldID = OrderID;
            if (dt.Rows.Count > 0)
                OrderID = (int)dt.Rows[0]["OrderID"];

            if (oldID != OrderID)
                foreach (Book b in Books.Keys)
                    SQL.Execute("uspSaveOrderBooks",
                    new Param("OrderID", OrderID),
                    new Param("BookID", b.BookID),
                    new Param("Price", Books[b].Price),
                    new Param("Quantity", Books[b].Quantity),
                    new Param("Cost", Books[b].Cost));
        }

        /// <summary>
        /// Get all orders.
        /// </summary>
        /// <returns>A list of all orders</returns>
        public static List<Order> GetAll()
        {
            return GetOrders(SQL.Execute("uspGetOrders"));
        }

        /// <summary>
        /// Get all orders made by the give user
        /// </summary>
        /// <param name="UserID">The user's id</param>
        /// <returns>A list of orders</returns>
        public static List<Order> GetAllByUser(int UserID)
        {
            return GetOrders(SQL.Execute("uspGetOrdersByUser", new Param("UserID", UserID)));
        }

        /// <summary>
        /// Get a list of order from the given DataTable.
        /// </summary>
        /// <param name="dt">A DataTable containing orders</param>
        /// <returns>A list of orders</returns>
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
                    dr["FulfilledDate"] == DBNull.Value ? null : (DateTime?)dr["FulfilledDate"],
                    dr["ParentOrderID"] == DBNull.Value ? null : (int?)dr["ParentOrderID"]
                    ));
            return orders;
        }
    }
}