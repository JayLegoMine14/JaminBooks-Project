using JaminBooks.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static JaminBooks.Tools.SQL;
using JaminBooks.Tools;

namespace JaminBooks.Model
{
    /// <summary>
    /// Models a debit or credit card
    /// </summary>
    public class Card
    {
        /// <summary>
        /// The unique id number that identifies this card. -1 represents a uncreated card.
        /// </summary>
        public int CardID { private set; get; } = -1;

        /// <summary>
        /// The expiration month as a two digits.
        /// </summary>
        public string ExpMonth;
        /// <summary>
        /// The expiration year as two digits.
        /// </summary>
        public string ExpYear;
        /// <summary>
        /// The full name on the card.
        /// </summary>
        public string Name;
        /// <summary>
        /// The address associated with this card.
        /// </summary>
        public Address Address;
        /// <summary>
        /// The last four digits of the card number.
        /// </summary>
        public string LastFourDigits { private set; get; }

        /// <summary>
        /// Whether or not the card number is currently encrypted.
        /// </summary>
        private bool IsEncrypted = true;

        /// <summary>
        /// Whether or not the CVC is hashed.
        /// </summary>
        private bool IsHashed = true;

        /// <summary>
        /// The CVC number of the card.
        /// </summary>
        public String CVC
        {
            set
            {
                _CVC = value;
                //If the CVC value is set, it has not yet been hashed.
                IsHashed = false;
            }
        }

        /// <summary>
        /// The card number.
        /// </summary>
        public String Number
        {
            set
            {
                _Number = value;
                //Set the last for digits
                LastFourDigits = _Number.Substring(_Number.Length - 4);
                //Clear the CVC
                _CVC = "";
                IsHashed = false;
                //If the number has just been set, it is not encrypted
                IsEncrypted = false;
            }
            get
            {
                return _Number;
            }
        }

        /// <summary>
        /// A private copy of the CVC
        /// </summary>
        private string _CVC;
        /// <summary>
        /// A private copy of the CVC
        /// </summary>
        private string _Number;

        /// <summary>
        /// The user who owns the card
        /// </summary>
        public User User
        {
            get
            {
                return new User(UserID);
            }
            set
            {
                UserID = value.UserID;
            }
        }

        /// <summary>
        /// Private id number of the user
        /// </summary>
        private int UserID = -1;

        /// <summary>
        /// Instantiate an empty card with all of the default values
        /// </summary>
        public Card()
        {
            IsEncrypted = false;
            IsHashed = false;
        }

        /// <summary>
        /// Instantiate a card and set the fields equal to the card in the database with the given id number
        /// </summary>
        /// <param name="CardID">The card's id number</param>
        public Card(int CardID)
        {
            DataTable dt = SQL.Execute("uspGetCardByID", new Param("CardID", CardID));

            if (dt.Rows.Count > 0)
            {
                this.CardID = CardID;
                this.UserID = (int)dt.Rows[0]["UserID"];
                this._CVC = (String)dt.Rows[0]["CVC"];
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

        /// <summary>
        /// Instantiate a card and set the fields equal to the given parameters
        /// </summary>
        /// <param name="CardID">The card's id</param>
        /// <param name="UserID">The card's user's id</param>
        /// <param name="Number">The card number</param>
        /// <param name="CVC">The card's CVC</param>
        /// <param name="ExpMonth">The card's expiration month</param>
        /// <param name="ExpYear">The card's expiration year</param>
        /// <param name="Name">The name on the card</param>
        /// <param name="AddressID">The id of the card's address</param>
        /// <param name="LastFourDigits">The last four digits of the card's number</param>
        private Card(int CardID, int UserID, string Number, string CVC, string ExpMonth, string ExpYear, string Name, int AddressID, string LastFourDigits)
        {
            this.CardID = CardID;
            this.UserID = UserID;
            this._CVC = CVC;
            this._Number = Number;
            this.ExpMonth = ExpMonth;
            this.ExpYear = ExpYear;
            this.Name = Name;
            this.Address = new Address(AddressID);
            this.LastFourDigits = LastFourDigits;
        }

        /// <summary>
        /// Save the card to teh database.
        /// </summary>
        public void Save()
        {
            this.Address.Save();
            DataTable dt = SQL.Execute("uspSaveCard",
               new Param("CardID", CardID),
               new Param("UserID", UserID),
               new Param("Number", IsEncrypted ? _Number : Encryption.Encrypt(_Number, _CVC)),
               new Param("CVC", IsHashed ? _CVC : Authentication.Hash(_CVC)),
               new Param("ExpMonth", ExpMonth),
               new Param("ExpYear", ExpYear),
               new Param("Name", Name),
               new Param("AddressID", Address.AddressID),
               new Param("LastFourDigits", LastFourDigits));

            if (dt.Rows.Count > 0)
                CardID = (int)dt.Rows[0]["CardID"];
        }

        /// <summary>
        /// Delete the card from the database, delete its address, and set its id to -1.
        /// </summary>
        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeleteCard", new Param("CardID", CardID));
            CardID = -1;
            Address.Delete();
        }

        /// <summary>
        /// Decrypt the card number using the CVC as the password.
        /// </summary>
        /// <param name="CVC">The card's unhashed CVC</param>
        /// <returns></returns>
        public bool DecryptNumber(string CVC)
        {
            if (Authentication.Hash(CVC) == this._CVC.Trim() && IsEncrypted == true)
            {
                Number = Encryption.Decrypt(Number, CVC);
                IsEncrypted = false;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Get a list of all cards owned by a specific user.
        /// </summary>
        /// <param name="UserID">The user's id</param>
        /// <returns>A list of cards.</returns>
        public static List<Card> GetCards(int UserID)
        {
            DataTable dt = SQL.Execute("uspGetCards", new Param("UserID", UserID));
            List<Card> cards = new List<Card>();
            foreach (DataRow dr in dt.Rows)
                cards.Add(new Card(
                    (int)dr["CardID"],
                    (int)dr["UserID"],
                    (String)dr["Number"],
                    (String)dr["CVC"],
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
