using JaminBooks.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JaminBooks.Tools
{
    public class Authentication
    {
       public static User GetCurrentUser(HttpContext context)
       {
            int? ID = context.Session.GetInt32("UserID");
            return ID == null ? null : new User(ID.Value);
       }

        public static void LogoutCurrentUser(HttpContext context)
        {
            context.Session.Remove("UserID");
        }

        public static bool UserExists(HttpRequest request)
        {
            Dictionary<string, string> user = GetCredentials(request);
            return User.Exists(user["Email"], Hash(user["Password"]));
        }

        public static bool EmailExists(HttpRequest request)
        {
            Dictionary<string, string> user = GetCredentials(request);
            return User.Exists(user["Email"]);
        }

        public static bool SetCurrentUser(HttpRequest request)
        {
            Dictionary<string, string> user = GetCredentials(request);
            int? UserID;
            if (User.Exists(user["Email"], Hash(user["Password"]), out UserID) 
                && !new User(UserID.Value).IsDeleted)
            {
                request.HttpContext.Session.SetInt32("UserID", UserID.Value);
                return true;
            }
            else return false;
        }

        public static bool CreateUser(HttpRequest request)
        {
            try
            {
                Dictionary<string, string> creds = GetCredentials(request);

                User user = new User();
                user.FirstName = creds["FirstName"];
                user.LastName = creds["LastName"];
                user.Email = creds["Email"];
                user.Password = Hash(creds["Password"]);

                if (user.FirstName != "" &&
                    user.LastName != "" &&
                    user.FirstName.Length <= 50 &&
                    user.LastName.Length <= 50 &&
                    user.Email.Length <= 100 &&
                    !User.Exists(user.Email) &&
                    new Regex("^(([^<>()[\\]\\.,;:\\s@\"]+(\\.[^<>()[\\]\\.,;:\\s@\"]+)*)|(\".+\"))@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$")
                    .IsMatch(user.Email))
                {
                    user.Save();
                    request.HttpContext.Session.SetInt32("UserID", user.UserID);
                    return true;
                }
                else return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Dictionary<string, string> GetCredentials(HttpRequest request)
        {
            MemoryStream stream = new MemoryStream();
            request.Body.CopyTo(stream);
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream))
            {
                string requestBody = reader.ReadToEnd();
                if (requestBody.Length > 0)
                {
                    Dictionary<string, string> user = 
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(requestBody);
                    if (user != null)
                    {
                        return user;
                    }
                }
            }

            throw new Exception("Invalid JSON Object");
        }

        public static string Hash(string data)
        {
            var bytes = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(data));
            var hash = new System.Text.StringBuilder();
            foreach (byte b in bytes)
            {
                hash.Append(b.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
