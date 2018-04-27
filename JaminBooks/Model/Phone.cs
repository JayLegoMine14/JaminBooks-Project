using JaminBooks.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using static JaminBooks.Tools.SQL;

namespace JaminBooks.Model
{
    /// <summary>
    /// Models a phone
    /// </summary>
    public class Phone
    {
        /// <summary>
        /// The unique id number identifying this phone. -1 represents an uncreated phone.
        /// </summary>
        public int PhoneID { private set; get; } = -1;

        /// <summary>
        /// The number of this phone.
        /// </summary>
        public string Number;

        /// <summary>
        /// The category of phone.
        /// </summary>
        public string Category;

        /// <summary>
        /// Instantiate an empty phone with default values.
        /// </summary>
        public Phone() { }

        /// <summary>
        /// Instantiate a phone and set its fields equal to the phone in the database with the given id.
        /// </summary>
        /// <param name="PhoneID">The phone's id</param>
        public Phone(int PhoneID)
        {
            DataTable dt = SQL.Execute("uspGetPhoneByID", new Param("PhoneID", PhoneID));

            if (dt.Rows.Count > 0)
            {
                this.PhoneID = PhoneID;
                this.Number = (String)dt.Rows[0]["PhoneNumber"];
                this.Category = (String)dt.Rows[0]["PhoneCategory"];
            }
            else
            {
                throw new Exception("Invalid Phone ID");
            }
        }

        /// <summary>
        /// Initialize a phone and set the fields equal to the given parameters.
        /// </summary>
        /// <param name="PhoneID">The phone's id</param>
        /// <param name="Number">The phone's number</param>
        /// <param name="Category">The phone's category</param>
        private Phone(int PhoneID, string Number, string Category)
        {
            this.PhoneID = PhoneID;
            this.Number = Number;
            this.Category = Category;
        }

        /// <summary>
        /// Save the phone to the database.
        /// </summary>
        public void Save()
        {
            DataTable dt = SQL.Execute("uspSavePhone",
               new Param("PhoneID", PhoneID),
               new Param("PhoneNumber", Number),
               new Param("PhoneCategory", Category));

            if (dt.Rows.Count > 0)
                PhoneID = (int)dt.Rows[0]["PhoneID"];
        }

        /// <summary>
        /// Delete the phone from the database and set its id to -1.
        /// </summary>
        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeletePhone", new Param("PhoneID", PhoneID));
            PhoneID = -1;
        }

        /// <summary>
        /// Add the phone to the given user.
        /// </summary>
        /// <param name="UserID">The user's id</param>
        public void AddUser(int UserID)
        {
            DataTable dt = SQL.Execute("uspPhoneAddUser",
                new Param("PhoneID", PhoneID),
                new Param("UserID", UserID));
        }

        /// <summary>
        /// Get the user who owns the phone
        /// </summary>
        /// <returns>The user's id</returns>
        public int GetUserID()
        {
            DataTable dt = SQL.Execute("uspGetPhoneByID",
                new Param("PhoneID", PhoneID));
            return (int)dt.Rows[0]["UserID"];
        }

        /// <summary>
        /// Gets a list of phones owned by the given user.
        /// </summary>
        /// <param name="UserID">The user's id</param>
        /// <returns>A list of phones.</returns>
        public static List<Phone> GetPhones(int UserID)
        {
            DataTable dt = SQL.Execute("uspGetPhones", new Param("UserID", UserID));
            List<Phone> phones = new List<Phone>();
            foreach (DataRow dr in dt.Rows)
                phones.Add(new Phone(
                    (int)dr["PhoneID"],
                    (String)dr["PhoneNumber"],
                    (String)dr["PhoneCategory"]
                    ));
            return phones;
        }

        /// <summary>
        /// Get a list of all phone categories.
        /// </summary>
        /// <returns>A dictionary that represents all phone categories.</returns>
        public static Dictionary<int, string> GetPhoneCategories()
        {
            DataTable dt = SQL.Execute("uspGetPhoneCategories");
            Dictionary<int, string> cats = new Dictionary<int, string>();
            foreach (DataRow dr in dt.Rows)
                cats.Add((int)dr["PhoneCategoryID"], (String)dr["PhoneCategory"]);
            return cats;
        }
    }
}