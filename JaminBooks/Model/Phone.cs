using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static JaminBooks.Model.SQL;

namespace JaminBooks.Model
{
    public class Phone
    {
        public int PhoneID { private set; get; } = -1;
        public string Number;
        public string Category;

        public Phone() { }

        public Phone(int PhoneID)
        {
            DataTable dt = SQL.Execute("uspGetPhoneByID", new Param("UserID", PhoneID));

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

        private Phone(int PhoneID, string Number, string Category)
        {
            this.PhoneID = PhoneID;
            this.Number = Number;
            this.Category = Category;
        }

        public void Save()
        {
            DataTable dt = SQL.Execute("uspSavePhone",
               new Param("PhoneID", PhoneID),
               new Param("PhoneNumber", Number),
               new Param("PhoneCategory", Category));

            if (dt.Rows.Count > 0)
                PhoneID = (int)dt.Rows[0]["PhoneID"];
        }

        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeletePhone", new Param("PhoneID", PhoneID));
            PhoneID = -1;
        }

        public void AddUser(int UserID)
        {
            DataTable dt = SQL.Execute("uspPhoneAddUser",
                new Param("PhoneID", PhoneID),
                new Param("UserID", UserID));
        }

        public static List<Phone> GetPhones(int UserID)
        {
            DataTable dt = SQL.Execute("uspGetPhones", new Param("UserID", UserID));
            List<Phone> phones = new List<Phone>();
            foreach(DataRow dr in dt.Rows)
                phones.Add(new Phone(
                    (int) dr["PhoneID"],
                    (String)dr["PhoneNumber"],
                    (String)dr["PhoneCategory"]
                    ));
            return phones;
        }
    }
}
