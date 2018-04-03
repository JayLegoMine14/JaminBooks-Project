using System;
using System.Data;
using static JaminBooks.Model.SQL;
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

        public Publisher() { }

        public Publisher(int PublisherID)
        {
            DataTable dt = SQL.Execute("uspGetPublisherByID", new Param("PublisherID", PublisherID));
            if (dt.Rows.Count > 0)
            {
                this.PublisherID = PublisherID;
                this.PublisherName = (string)dt.Rows[0]["PublisherName"]; 
                this.AddressID = (int)dt.Rows[0]["AddressID"];
                this.PhoneID = (int)dt.Rows[0]["PhoneID"];
                this.ContactFirstName = (string)dt.Rows[0]["ContactFirstName"];
                this.ContactLastName = (string)dt.Rows[0]["ContactLastName"];
            }
            else
            {
                throw new Exception("Invalid ID");
            }
        }

        public int SavePublisherName(string PublisherName)
        {
            {

                DataTable dt = SQL.Execute("uspSavePublisherName",
                new Param("PublisherName", PublisherName),
                new Param("AddressID", AddressID),
                new Param("PhoneID", PhoneID),
                new Param("ContactFirstName", ContactFirstName),
                new Param("ContactLastName", ContactLastName));

                if (dt.Rows.Count > 0)
                {
                    PublisherID = (int)dt.Rows[0]["PublisherID"];

                }
                else
                {
                    throw new Exception("Book Not Created");
                }

                return PublisherID;
            }
        }

        public int GetPublisherIDByName(string PublisherName)
        {
            DataTable dt = SQL.Execute("uspGetPublisherIDByName",
                new Param("PublisherName", PublisherName));


            if (dt.Rows.Count == 0)
            {
                PublisherID = SavePublisherName(PublisherName);
            }
            else
            {
                PublisherID = (int)dt.Rows[0]["PublisherID"];

            }


            return PublisherID;
        }

        public void delete()
        {
            DataTable dt = SQL.Execute("uspDeletePublisher",
                new Param("PublisherID", PublisherID));
        }
    }
}