using JaminBooks.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static JaminBooks.Model.SQL;

namespace JaminBooks.Model
{
    public class Card
    {
        public int CardID { private set; get; } = -1;

        public string Number;
        public string ExpMonth;
        public string ExpYear;
        public string Name;
        public Address Address;
        public User User {
            get {
                return new User(UserID);
            }
            set
            {
                UserID = value.UserID;
            }
        }

        public string CCV {
            set {
                CCV = Authentication.Hash(value);
            }
            private get { return CCV; }
        }

        private int UserID = -1;

        public Card() { }

        public Card(int CardID) {
            DataTable dt = SQL.Execute("uspGetCardByID", new Param("CardID", CardID));

            if (dt.Rows.Count > 0)
            {
                this.CardID = CardID;
                this.UserID = (int)dt.Rows[0]["UserID"];
                this.CCV = (String)dt.Rows[0]["CCV"];
                this.Number = Encryption.Decrypt((String)dt.Rows[0]["Number"], CCV);
                this.ExpMonth = (String)dt.Rows[0]["ExpMonth"];
                this.ExpYear = (String)dt.Rows[0]["ExpYear"];
                this.Name = (String)dt.Rows[0]["Name"];
                this.Address = new Address((int)dt.Rows[0]["AddressID"]);
            }
            else
            {
                throw new Exception("Invalid Card ID");
            }
        }

        private Card(int CardID, int UserID, string Number, string CCV, string ExpMonth, string ExpYear, string Name, int AddressID)
        {
            this.CardID = CardID;
            this.UserID = UserID;
            this.CCV = CCV;
            this.Number = Encryption.Decrypt(Number, CCV);
            this.ExpMonth = ExpMonth;
            this.ExpYear = ExpYear;
            this.Name = Name;
            this.Address = new Address(AddressID);
        }

        public void Save()
        {
            this.Address.Save();
            DataTable dt = SQL.Execute("uspSaveCard",
               new Param("CardID", CardID),
               new Param("UserID", UserID),
               new Param("Number", Encryption.Encrypt(Number, CCV)),
               new Param("CCV", CCV),
               new Param("ExpMonth", ExpMonth),
               new Param("ExpYear", ExpYear),
               new Param("Name", Name),
               new Param("AddressID", Address.AddressID));

            if (dt.Rows.Count > 0)
                CardID = (int)dt.Rows[0]["CardID"];
        }

        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeleteCard", new Param("CardID", CardID));
            CardID = -1;
            Address.Delete();
        }

        public static List<Card> GetCards(int UserID)
        {
            DataTable dt = SQL.Execute("uspGetCards", new Param("UserID", UserID));
            List<Card> cards = new List<Card>();
            foreach (DataRow dr in dt.Rows)
                cards.Add(new Card(
                    (int)dr["CardID"],
                    (int)dr["UserID"],
                    (String)dr["Number"],
                    (String)dr["CCV"],
                    (String)dr["ExpMonth"],
                    (String)dr["ExpYear"],
                    (String)dr["Name"],
                    (int)dr["AddressID"]
                    ));
            return cards;
        }

    }
}
