using System;
using System.Data;
using static JaminBooks.Model.SQL;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JaminBooks.Model
{
    public class Publisher
    {
        public int PublisherID { private set; get; } = -1;
        public string PublisherName;
        public Address Address;
        public Phone Phone;
        public string ContactFirstName;
        public string ContactLastName;
        public bool IsDeleted;

        public string FullName
        {
            get
            {
                return ContactFirstName + " " + ContactLastName;
            }
        }

        public Publisher() { }

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

        private Publisher(int PublisherID, string PublisherName)
        {
            this.PublisherID = PublisherID;
            this.PublisherName = PublisherName;
        }

        public void Save()
        {
            DataTable dt = SQL.Execute("uspSavePublisher",
               new Param("PublisherID", PublisherID),
               new Param("PublisherName", PublisherName),
               new Param("AddressID", Address.AddressID),
               new Param("PhoneID", Phone.PhoneID),
               new Param("ContactFirstName", ContactFirstName),
               new Param("ContactFirstName", ContactFirstName),
               new Param("IsDeleted", IsDeleted));



            if (dt.Rows.Count > 0)
                PublisherID = (int)dt.Rows[0]["PublisherID"];
        }

        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeletePublisher", new Param("PublisherID", PublisherID));
            IsDeleted = true;
        }

        public int GetBookID()
        {
            DataTable dt = SQL.Execute("uspGetPublisherByID",
                new Param("PublisherID", PublisherID));
            return (int)dt.Rows[0]["PublisherID"];
        }

        public static List<Publisher> GetPublishers(int BookID)
        {
            DataTable dt = SQL.Execute("uspGetPublishers", new Param("BookID", BookID));
            List<Publisher> publishers = new List<Publisher>();
            foreach (DataRow dr in dt.Rows)
                publishers.Add(new Publisher(
                    (int)dr["PublisherID"],
                    (String)dr["PublisherName"]
                    ));
            return publishers;
        }

        public static List<Publisher> GetPublishers()
        {
            DataTable dt = SQL.Execute("uspAllGetPublishers");
            List<Publisher> publishers = new List<Publisher>();
            foreach (DataRow dr in dt.Rows)
                publishers.Add(new Publisher(
                    (int)dr["PublisherID"],
                    (String)dr["PulisherName"]
                    ));
            return publishers;
        }


        public void AddPublisher(int BookID)
        {
            DataTable dt = SQL.Execute("uspBookAddPublisher",
                new Param("BookID", BookID),
                new Param("PublisherID", PublisherID));
        }
    }
}
