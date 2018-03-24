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
        public DateTime CreationDate { get; }

        public string FirstName;
        public string LastName;
        public string Email;
        public bool IsDeleted = false;
        public bool IsAdmin =  false;

        private string Password;

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
            }
            else
            {
                throw new Exception("Invalid User ID");
            }
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
                new Param("IsAdmin", IsAdmin));
        }

        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeleteUser", new Param("UserID", UserID));
            UserID = -1;
        }
    }
}
