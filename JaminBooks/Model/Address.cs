using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using static JaminBooks.Model.SQL;

namespace JaminBooks.Model
{
    public class Address
    {
        public int AddressID { private set; get; } = -1;
        public string Line1;
        public string Line2;
        public string City;
        public string State;
        public string Country;
        public string ZIP;

        public Address() {}

        public Address(int AddressID)
        {
            DataTable dt = SQL.Execute("uspGetAddressByID", new Param("AddressID", AddressID));

            if (dt.Rows.Count > 0)
            {
                this.AddressID = AddressID;
                this.Line1 = (String)dt.Rows[0]["AddressLine1"];
                this.Line2 = dt.Rows[0]["AddressLine2"] != DBNull.Value ? (String)dt.Rows[0]["AddressLine2"] : null;
                this.City = (String)dt.Rows[0]["City"];
                this.State = dt.Rows[0]["State"] != DBNull.Value ? (String)dt.Rows[0]["State"] : null;
                this.Country = (String)dt.Rows[0]["Country"];
                this.ZIP = (String)dt.Rows[0]["ZIP"];
            }
            else
            {
                throw new Exception("Invalid Address ID");
            }
        }

        private Address (int AddressID, string Line1, string Line2, string City, string State, string Country, string ZIP)
        {
            this.AddressID = AddressID;
            this.Line1 = Line1;
            this.Line2 = Line2;
            this.City = City;
            this.State = State;
            this.Country = Country;
            this.ZIP = ZIP;
        }

        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeleteAddress", new Param("AddressID", AddressID));
            AddressID = -1;
        }

        public void Save()
        {
            DataTable dt = SQL.Execute("uspSaveAddress",
               new Param("AddressID", AddressID),
               new Param("Line1", Line1),
               new Param("Line2", Line2 ?? SqlString.Null),
               new Param("City", City),
               new Param("State", State ?? SqlString.Null),
               new Param("Country", Country),
               new Param("ZIP", ZIP));

            if (dt.Rows.Count > 0)
                AddressID = (int)dt.Rows[0]["AddressID"];
        }

        public void AddUser(int UserID)
        {
            DataTable dt = SQL.Execute("uspAddressAddUser",
                new Param("AddressID", AddressID),
                new Param("UserID", UserID));
        }

        public int GetUserID()
        {
            DataTable dt = SQL.Execute("uspGetAddressByID",
                new Param("AddressID", AddressID));
            return (int)dt.Rows[0]["UserID"];
        }

        public static List<Address> GetAddresses(int UserID)
        {
            return GetAddresses(UserID, "uspGetAddresses");
        }

        public static List<Address> GetAddressesIncludingCards(int UserID)
        {
            return GetAddresses(UserID, "uspGetAddressesIncludingCards");
        }

        private static List<Address> GetAddresses(int UserID, string proc)
        {
            DataTable dt = SQL.Execute(proc, new Param("UserID", UserID));
            List<Address> addresses = new List<Address>();
            foreach (DataRow dr in dt.Rows)
                addresses.Add(new Address(
                    (int)dr["AddressID"],
                    (String)dr["AddressLine1"],
                    dr["AddressLine2"] != DBNull.Value ? (String)dr["AddressLine2"] : null,
                    (String)dr["City"],
                    dr["State"] != DBNull.Value ? (String)dr["State"] : null,
                    (String)dr["Country"],
                    (String)dr["ZIP"]));
            return addresses;
        }
    }
}
