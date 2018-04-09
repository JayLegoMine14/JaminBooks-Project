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
        public int AddressID;
        public int PhoneID;       
        public string ContactFirstName;
        public string ContactLastName;
        public bool IsDeleted;

        public Publisher() { }

        public Publisher(int PublisherID)
        {
            DataTable dt = SQL.Execute("uspGetPublisherByID", new Param("PublisherID", PublisherID));

            if (dt.Rows.Count > 0)
            {
                this.PublisherID = PublisherID;
                this.PublisherName = (String)dt.Rows[0]["PublisherName"];
                this.AddressID = (int)dt.Rows[0]["AddressID"];
                this.PhoneID = (int)dt.Rows[0]["PhoneID"];
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
            this.AddressID = AddressID;
            this.PhoneID = PhoneID;
            this.ContactFirstName = ContactFirstName;
            this.ContactLastName = ContactLastName;
            this.IsDeleted = IsDeleted;
        }

        public void Save()
        {
            DataTable dt = SQL.Execute("uspSavePublisher",
               new Param("PublisherID", PublisherID),
               new Param("PublisherName", PublisherName),
               new Param("AddressID", AddressID),
               new Param("PhoneID", PhoneID),
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

        //These two need to be looked at
        public void AddAddress(Address a)
        {
            a.AddUser(this.PublisherID);
        }
        
        public void AddPhone(Phone p)
        {
            p.AddUser(this.PublisherID);
        }
        //these two need to be looked at


        public void AddPublisher(int BookID)
        {
            DataTable dt = SQL.Execute("uspBookAddPublisher",
                new Param("BookID", BookID),
                new Param("PublisherID", PublisherID));
        }
    }
}
