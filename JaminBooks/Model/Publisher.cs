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
        //Publisher fields
        public int PublisherID { private set; get; } = -1;
        public string PublisherName;
        public int AddressID;
        public int PhoneID;
        public string ContactFirstName;
        public string ContactLastName;

        //Address fields
        public string Line1;
        public string Line2;
        public string City;
        public string State;
        public string Country;
        public string ZIP;

        //Phone fields
        public string Number;
        public string Category;

        public Phone Phone = new Phone();
        public Address Address = new Address();

        public Publisher() { }


        public Publisher(int PublisherID)
        {
            DataTable dt = SQL.Execute("uspGetPublisherByID", new Param("PublisherID", PublisherID));
            if (dt.Rows.Count > 0)
            {
                this.PublisherID = PublisherID;
                this.PublisherName = (string)dt.Rows[0]["PublisherName"]; 
                this.AddressID = (int)dt.Rows[0]["AddressID"];
                this.ContactFirstName = (string)dt.Rows[0]["ContactFirstName"];
                this.ContactLastName = (string)dt.Rows[0]["ContactLastName"];
                this.Phone = new Phone((int)dt.Rows[0]["PhoneID"]);
                this.AddressID = ((int)dt.Rows[0]["AddressID"]);
            }
            else
            {
                throw new Exception("Invalid ID");
            }
        }


        public void Save()
        {
            {
                Phone.Number = Number;
                Phone.Category = Category;

                Address.Line1 = Line1;
                Address.Line2 = Line2;
                Address.City = City;
                Address.State = State;
                Address.Country = Country;
                Address.ZIP = ZIP;

                Phone.Save();
                Address.Save();
                DataTable dt = SQL.Execute("uspSavePublisher",
                new Param("PublisherName", PublisherName),
                new Param("AddressID", Address.AddressID),
                new Param("PhoneID", Phone.PhoneID),
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

             }
        }

        public int GetPublisherIDByName()
        {
            DataTable dt = SQL.Execute("uspGetPublisherIDByName",
                new Param("PublisherName", PublisherName));


            if (dt.Rows.Count == 0)
            {
                Save();
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