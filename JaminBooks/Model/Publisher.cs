using JaminBooks.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using static JaminBooks.Tools.SQL;

namespace JaminBooks.Model
{
    /// <summary>
    /// Models a book publisher
    /// </summary>
    public class Publisher
    {
        /// <summary>
        /// The unique id number that identifies the publisher. -1 represents an uncreated publisher.
        /// </summary>
        public int PublisherID { private set; get; } = -1;

        /// <summary>
        /// The name of the publisher.
        /// </summary>
        public string PublisherName;

        /// <summary>
        /// The address of the publisher.
        /// </summary>
        public Address Address;

        /// <summary>
        /// The phone of the publisher's contact.
        /// </summary>
        public Phone Phone;

        /// <summary>
        /// The first name of the publisher's contact.
        /// </summary>
        public string ContactFirstName;

        /// <summary>
        /// The last name of the publisher's contact.
        /// </summary>
        public string ContactLastName;

        /// <summary>
        /// Whether or not the publisher is deleted.
        /// </summary>
        public bool IsDeleted;

        /// <summary>
        /// The first name and last name of the publisher's contact joined with a space.
        /// </summary>
        public string FullName
        {
            get
            {
                return ContactFirstName + " " + ContactLastName;
            }
        }

        /// <summary>
        /// Initialize an empty publisher with default values.
        /// </summary>
        public Publisher() { }

        /// <summary>
        /// Initialize a publisher and set its fields equal to the publisher in the database with the given id.
        /// </summary>
        /// <param name="PublisherID">The publisher's id</param>
        public Publisher(int PublisherID)
        {
            DataTable dt = SQL.Execute("uspGetPublisherByID", new Param("PublisherID", PublisherID));

            if (dt.Rows.Count > 0)
            {
                this.PublisherID = PublisherID;
                this.PublisherName = (String)dt.Rows[0]["PublisherName"];
                this.Address = new Address((int)dt.Rows[0]["AddressID"]);
                this.Phone = new Phone((int)dt.Rows[0]["PhoneID"]);
                this.ContactFirstName = (String)dt.Rows[0]["ContactFirstName"];
                this.ContactLastName = (String)dt.Rows[0]["ContactLastName"];
                this.IsDeleted = (bool)dt.Rows[0]["IsDeleted"];
            }
            else
            {
                throw new Exception("Invalid Publisher ID");
            }
        }

        /// <summary>
        /// Initialize a publisher and set the fields equal to the given parameters
        /// </summary>
        /// <param name="PublisherID">The id of the publisher</param>
        /// <param name="PublisherName">The name of the publisher</param>
        /// <param name="AddressID">The id of the publisher's address</param>
        /// <param name="PhoneID">The id of the publisher's phone</param>
        /// <param name="ContactFirstName">The first name of the publisher's contact</param>
        /// <param name="ContactLastName">The last name of the publisher's contact</param>
        /// <param name="IsDeleted">Whether or not the publisher has been deleted</param>
        private Publisher(int PublisherID, string PublisherName, int AddressID, int PhoneID,
            string ContactFirstName, string ContactLastName, bool IsDeleted)
        {
            this.PublisherID = PublisherID;
            this.PublisherName = PublisherName;
            this.Address = new Address(AddressID);
            this.Phone = new Phone(PhoneID);
            this.ContactFirstName = ContactFirstName;
            this.ContactLastName = ContactLastName;
            this.IsDeleted = IsDeleted;
        }

        /// <summary>
        /// Save the publisher to the database.
        /// </summary>
        public void Save()
        {
            DataTable dt = SQL.Execute("uspSavePublisher",
               new Param("PublisherID", PublisherID),
               new Param("PublisherName", PublisherName),
               new Param("AddressID", Address.AddressID),
               new Param("PhoneID", Phone.PhoneID),
               new Param("ContactFirstName", ContactFirstName),
               new Param("ContactLastName", ContactLastName),
               new Param("IsDeleted", IsDeleted));

            if (dt.Rows.Count > 0)
                PublisherID = (int)dt.Rows[0]["PublisherID"];
        }

        /// <summary>
        /// Delete the publisher from the database and set its id as -1.
        /// </summary>
        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeletePublisher", new Param("PublisherID", PublisherID));
            IsDeleted = true;
        }

        /// <summary>
        /// Get the number of books published by this publisher.
        /// </summary>
        /// <returns>the number of books</returns>
        public int GetBooks()
        {
            return (int)SQL.Execute("uspGetBooksFromPublisher", new Param("PublisherID", PublisherID)).Rows[0][0];
        }

        /// <summary>
        /// Get the number of sales made by this publisher.
        /// </summary>
        /// <returns>The number of sales</returns>
        public int GetSales()
        {
            return (int)SQL.Execute("uspGetSalesFromPublisher", new Param("PublisherID", PublisherID)).Rows[0][0];
        }

        /// <summary>
        /// Get a list of publishers from the given DataTable.
        /// </summary>
        /// <param name="dt">A DataTable containing publishers</param>
        /// <returns>A list of publishers.</returns>
        public static List<Publisher> GetPublishers(DataTable dt)
        {
            List<Publisher> publishers = new List<Publisher>();
            foreach (DataRow dr in dt.Rows)
                publishers.Add(new Publisher(
                    (int)dr["PublisherID"],
                    (String)dr["PublisherName"],
                    (int)dr["AddressID"],
                    (int)dr["PhoneID"],
                    (String)dr["ContactFirstName"],
                    (String)dr["ContactLastName"],
                    (bool)dr["IsDeleted"]
                    ));
            return publishers;
        }

        /// <summary>
        /// Get all publishers.
        /// </summary>
        /// <returns>A list of all publishers</returns>
        public static List<Publisher> GetPublishers()
        {
            DataTable dt = SQL.Execute("uspGetAllPublishers");
            List<Publisher> publishers = new List<Publisher>();
            foreach (DataRow dr in dt.Rows)
                publishers.Add(new Publisher(
                    (int)dr["PublisherID"],
                    (String)dr["PublisherName"],
                    (int)dr["AddressID"],
                    (int)dr["PhoneID"],
                    (String)dr["ContactFirstName"],
                    (String)dr["ContactLastName"],
                    (bool)dr["IsDeleted"]
                    ));
            return publishers;
        }

        /// <summary>
        /// Add the publisher to given book.
        /// </summary>
        /// <param name="BookID">The book's id</param>
        public void AddPublisher(int BookID)
        {
            DataTable dt = SQL.Execute("uspBookAddPublisher",
                new Param("BookID", BookID),
                new Param("PublisherID", PublisherID));
        }
    }
}