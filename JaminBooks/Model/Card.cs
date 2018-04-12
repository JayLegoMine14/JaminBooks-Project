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

        public string ExpMonth;
        public string ExpYear;
        public string Name;
        public Address Address;
        public string LastFourDigits { private set; get; }

        private bool IsEncrypted = true;
        private bool IsHashed = true;

        public String CCV
        {
            set
            {
                _CCV = value;
                IsHashed = false;
            }
        }

        public String Number
        {
            set
            {
                _Number = value;
                LastFourDigits = _Number.Substring(_Number.Length - 4);
                _CCV = "";
                IsHashed = false;
                IsEncrypted = false;
            }
            get
            {
                return _Number;
            }
        }

        private string _CCV;
        private string _Number;

        public User User {
            get {
                return new User(UserID);
            }
            set
            {
                UserID = value.UserID;
            }
        }

        private int UserID = -1;

        public Card() {
            IsEncrypted = false;
            IsHashed = false;
        }

        public Card(int CardID) {
            DataTable dt = SQL.Execute("uspGetCardByID", new Param("CardID", CardID));

            if (dt.Rows.Count > 0)
            {
                this.CardID = CardID;
                this.UserID = (int)dt.Rows[0]["UserID"];
                this._CCV = (String)dt.Rows[0]["CCV"];
                this._Number = (String)dt.Rows[0]["Number"];
                this.ExpMonth = (String)dt.Rows[0]["ExpMonth"];
                this.ExpYear = (String)dt.Rows[0]["ExpYear"];
                this.Name = (String)dt.Rows[0]["Name"];
                this.Address = new Address((int)dt.Rows[0]["AddressID"]);
                this.LastFourDigits = (String)dt.Rows[0]["LastFourDigits"];
            }
            else
            {
                throw new Exception("Invalid Card ID");
            }
        }

        private Card(int CardID, int UserID, string Number, string CCV, string ExpMonth, string ExpYear, string Name, int AddressID, string LastFourDigits)
        {
            this.CardID = CardID;
            this.UserID = UserID;
            this._CCV = CCV;
            this._Number = Number;
            this.ExpMonth = ExpMonth;
            this.ExpYear = ExpYear;
            this.Name = Name;
            this.Address = new Address(AddressID);
            this.LastFourDigits = LastFourDigits;
        }

        public void Save()
        {
            this.Address.Save();
            DataTable dt = SQL.Execute("uspSaveCard",
               new Param("CardID", CardID),
               new Param("UserID", UserID),
               new Param("Number", IsEncrypted ? _Number : Encryption.Encrypt(_Number, _CCV)),
               new Param("CCV", IsHashed ? _CCV : Authentication.Hash(_CCV)),
               new Param("ExpMonth", ExpMonth),
               new Param("ExpYear", ExpYear),
               new Param("Name", Name),
               new Param("AddressID", Address.AddressID),
               new Param("LastFourDigits", LastFourDigits));

            if (dt.Rows.Count > 0)
                CardID = (int)dt.Rows[0]["CardID"];
        }

        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeleteCard", new Param("CardID", CardID));
            CardID = -1;
            Address.Delete();
        }

        public bool DecryptNumber(string CCV)
        {
            if (Authentication.Hash(CCV) == this._CCV.Trim() && IsEncrypted == true)
            {
                Number = Encryption.Decrypt(Number, CCV);
                IsEncrypted = false;
                return true;
            }
            else return false;
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
                    (int)dr["AddressID"],
                    (String)dr["LastFourDigits"]
                    ));
            return cards;
        }
    }
}
