using JaminBooks.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

        public static bool UserExists(HttpRequest request)
        {
            KeyValuePair<string, string> user = GetCredentials(request);
            return User.Exists(user.Key, user.Value);
        }

        public static bool SetCurrentUser(HttpRequest request)
        {
            KeyValuePair<string, string> user = GetCredentials(request);
            int? UserID;
            if (User.Exists(user.Key, user.Value, out UserID) 
                && !new User(UserID.Value).IsDeleted)
            {
                request.HttpContext.Session.SetInt32("UserID", UserID.Value);
                return true;
            }
            else return false;
        }

        public static KeyValuePair<string, string> GetCredentials(HttpRequest request)
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
                        return new KeyValuePair<string, string>(user["Email"], Hash(user["Password"]));
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
