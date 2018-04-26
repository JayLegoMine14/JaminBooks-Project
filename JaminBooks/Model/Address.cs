using JaminBooks.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using static JaminBooks.Tools.SQL;
using JaminBooks.Tools;

namespace JaminBooks.Model
{
    /// <summary>
    /// Models an address, shipping or billing
    /// </summary>
    public class Address
    {
        /// <summary>
        /// A unique number representing this address. -1 represents an uncreated address.
        /// </summary>
        public int AddressID { private set; get; } = -1;
        /// <summary>
        /// The first line of the street address.
        /// </summary>
        public string Line1;
        /// <summary>
        /// The second line of the street address.
        /// </summary>
        public string Line2;
        /// <summary>
        /// The city of the address.
        /// </summary>
        public string City;
        /// <summary>
        /// The state of the address. This will be a two letter code.
        /// </summary>
        public string State;
        /// <summary>
        /// The country of the address. This will be a two letter code.
        /// </summary>
        public string Country;
        /// <summary>
        /// The ZIP code of the address.
        /// </summary>
        public string ZIP;

        /// <summary>
        /// Instantiates an empty address with all the default values.
        /// </summary>
        public Address() { }

        /// <summary>
        /// Instantiates an address and fills the fields with the address in the database with the given id number.
        /// </summary>
        /// <param name="AddressID">The addresses id number</param>
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

        /// <summary>
        /// Instantiates an address with the given its fields set to the given parameters.
        /// </summary>
        /// <param name="AddressID">The address's id</param>
        /// <param name="Line1">Street address line 1</param>
        /// <param name="Line2">Street address line 2</param>
        /// <param name="City">City</param>
        /// <param name="State">State</param>
        /// <param name="Country">Country</param>
        /// <param name="ZIP">ZIP</param>
        private Address(int AddressID, string Line1, string Line2, string City, string State, string Country, string ZIP)
        {
            this.AddressID = AddressID;
            this.Line1 = Line1;
            this.Line2 = Line2;
            this.City = City;
            this.State = State;
            this.Country = Country;
            this.ZIP = ZIP;
        }

        /// <summary>
        /// Set the address isdeleted true in the database and set its id to -1.
        /// </summary>
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

        /// <summary>
        /// Adds this address to the given user.
        /// </summary>
        /// <param name="UserID"></param>
        public void AddUser(int UserID)
        {
            DataTable dt = SQL.Execute("uspAddressAddUser",
                new Param("AddressID", AddressID),
                new Param("UserID", UserID));
        }

        /// <summary>
        /// Gets the id number of the user who owns this address.
        /// </summary>
        /// <returns>The is number of the user who owns this address.</returns>
        public int GetUserID()
        {
            DataTable dt = SQL.Execute("uspGetAddressByID",
                new Param("AddressID", AddressID));
            return (int)dt.Rows[0]["UserID"];
        }

        /// <summary>
        /// Gets a list of address that belong to the given user.
        /// </summary>
        /// <param name="UserID">A user's id number.</param>
        /// <returns>A list of addresses.</returns>
        public static List<Address> GetAddresses(int UserID)
        {
            return GetAddresses(UserID, "uspGetAddresses");
        }

        /// <summary>
        /// Gets a list of address that belong to the given user or one of the user's credit cards.
        /// </summary>
        /// <param name="UserID">A user's id number.</param>
        /// <returns>A list of addresses.</returns>
        public static List<Address> GetAddressesIncludingCards(int UserID)
        {
            return GetAddresses(UserID, "uspGetAddressesIncludingCards");
        }

        /// <summary>
        /// Gets a list of address that belong to the given user using the specified stored procedure.
        /// </summary>
        /// <param name="UserID">A user's id number.</param>
        /// <param name="proc">The stored procedure to use to get the desired addresses.</param>
        /// <returns>A list of addresses.</returns>
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
