using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static JaminBooks.Model.SQL;

namespace JaminBooks.Model
{
    public class User
    {
        public int UserID { private set; get; } = -1;
        public DateTime CreationDate { private set; get; }

        public string FirstName;
        public string LastName;
        public string Email;
        public bool IsDeleted = false;
        public bool IsAdmin = false;
        public bool IsConfirmed = false;
        public string ConfirmationCode;
        public string Password;
        public string IconLink;

        public List<Address> Addresses {
            get
            {
                return Address.GetAddresses(this.UserID);
            }
        }

        public List<Card> Cards
        {
            get
            {
                return Card.GetCards(this.UserID);
            }
        }

        public List<Phone> Phones
        {
            get
            {
                return Phone.GetPhones(this.UserID);
            }
        }

        public User() { }

        public User(int UserID)
        {
            DataTable dt = SQL.Execute("uspGetUserByID", new Param("UserID", UserID));

            if(dt.Rows.Count > 0)
            {
                this.UserID = UserID;
                this.FirstName = (String) dt.Rows[0]["FirstName"];
                this.LastName = (String) dt.Rows[0]["LastName"]; 
                this.CreationDate = (DateTime) dt.Rows[0]["CreationDate"];
                this.Password = (String) dt.Rows[0]["Password"];
                this.Email = (String) dt.Rows[0]["Email"];
                this.IsDeleted = (Boolean) dt.Rows[0]["IsDeleted"];
                this.IsAdmin = (Boolean) dt.Rows[0]["IsAdmin"];
                this.IsConfirmed = (Boolean)dt.Rows[0]["IsConfirmed"];
                this.ConfirmationCode = dt.Rows[0]["ConfirmationCode"] == DBNull.Value ? null : (String)dt.Rows[0]["ConfirmationCode"];
                this.IconLink = dt.Rows[0]["IconLink"] == DBNull.Value ? null : (String) dt.Rows[0]["IconLink"];
            }
            else
            {
                throw new Exception("Invalid User ID");
            }
        }

        private User(int UserID, string FirstName, string LastName, DateTime CreationDate, 
            string Password, string Email, bool IsDeleted, bool IsAdmin, bool IsConfirmed, 
            string ConfirmationCode, string IconLink)
        {
            this.UserID = UserID;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.CreationDate = CreationDate;
            this.Password = Password;
            this.Email = Email;
            this.IsDeleted = IsDeleted;
            this.IsAdmin = IsAdmin;
            this.IsConfirmed = IsConfirmed;
            this.ConfirmationCode = ConfirmationCode;
            this.IconLink = IconLink;
        }

        public void Save()
        {
            DataTable dt = SQL.Execute("uspSaveUser", 
                new Param("UserID", UserID),
                new Param("FirstName", FirstName),
                new Param("LastName", LastName),
                new Param("Email", Email),
                new Param("Password", Password),
                new Param("IsDeleted", IsDeleted),
                new Param("IsAdmin", IsAdmin),
                new Param("IsConfirmed", IsConfirmed),
                new Param("ConfirmationCode", ConfirmationCode),
                new Param("IconLink", (object) IconLink ?? DBNull.Value));

            if (dt.Rows.Count > 0)
                UserID = (int) dt.Rows[0]["UserID"];
        }

        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeleteUser", new Param("UserID", UserID));
            UserID = -1;
        }

        public void AddAddress(Address a)
        {
            a.AddUser(this.UserID);
        }

        public void AddCard(Card c)
        {
            c.User = this;
        }

        public void AddPhone(Phone p)
        {
            p.AddUser(this.UserID);
        }

        public static List<User> GetUsers()
        {
            DataTable dt = SQL.Execute("uspGetUsers");
            List<User> users = new List<User>();
            foreach (DataRow dr in dt.Rows)
                users.Add(new User(
                    (int)dr["UserID"],
                    (String)dr["FirstName"],
                    (String)dr["LastName"],
                    (DateTime)dr["CreationDate"],
                    (String)dr["Password"],
                    (String)dr["Email"],
                    (Boolean)dr["IsDeleted"],
                    (Boolean)dr["IsAdmin"],
                    (Boolean)dr["IsConfirmed"],
                    (String)dr["ConfirmationCode"],
                    (String)dr["IconLink"]));
            return users;
        }

        public static bool Exists(string Email, string Password, out int? id)
        {
            DataTable results = SQL.Execute("uspGetUserByEmailAndPassword",
                new Param("Email", Email),
                new Param("Password", Password));
            id = results.Rows.Count != 1 ? null : results.Rows[0]["UserID"] as int?;
            return results.Rows.Count >= 1;
        }

        public static bool Exists(string Email, string Password)
        {
            DataTable results = SQL.Execute("uspGetUserByEmailAndPassword",
                new Param("Email", Email),
                new Param("Password", Password));
            return results.Rows.Count >= 1;
        }

        public static bool Exists(string Email)
        {
            DataTable results = SQL.Execute("uspGetUserByEmail",
                new Param("Email", Email));
            return results.Rows.Count >= 1;
        }
    }
}
