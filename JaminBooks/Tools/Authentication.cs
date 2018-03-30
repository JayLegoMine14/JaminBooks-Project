using JaminBooks.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JaminBooks.Tools
{
    public class Authentication
    {
       public static string Email;
       public static string Password;
       public static string Name;
       static Random RANDOM = new Random();

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

        public static int CreateUser(HttpRequest request)
        {
            try
            {
                Dictionary<string, string> creds = GetCredentials(request);

                User user = new User();
                user.FirstName = creds["FirstName"];
                user.LastName = creds["LastName"];
                user.Email = creds["Email"];
                user.Password = Hash(creds["Password"]);

                var phoneNumber = creds["Phone"];

                if (user.FirstName != "" &&
                    user.LastName != "" &&
                    user.FirstName.Length <= 50 &&
                    user.LastName.Length <= 50 &&
                    user.Email.Length <= 100 &&
                    phoneNumber.Length <= 20 &&
                    !User.Exists(user.Email) &&
                    new Regex("^(([^<>()[\\]\\.,;:\\s@\"]+(\\.[^<>()[\\]\\.,;:\\s@\"]+)*)|(\".+\"))@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$")
                    .IsMatch(user.Email) &&
                    new Regex("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$")
                    .IsMatch(phoneNumber))
                {
                    user.ConfirmationCode = GenerateConfirmationCode();

                    Phone p = new Phone();
                    p.Number = phoneNumber;
                    p.Category = creds["PhoneCat"];
                    p.Save();

                    user.Save();
                    user.AddPhone(p);
                    if (!SendConfirmationEmail(request, user)) return 2;
                    request.HttpContext.Session.SetInt32("UserID", user.UserID);
                    return 1;
                }
                else return 0;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public static bool SendConfirmationEmail(HttpRequest request, User u)
        {
            try
            {
                var fromAddress = new MailAddress(Email, Name);
                var toAddress = new MailAddress(u.Email, u.FirstName + " " + u.LastName);
                string callbackURL = "http://" + request.Host + @"/Security/Confirm?id=" + u.UserID + "&c=" + u.ConfirmationCode;
                string subject = "Email Confirmation | Jamin' Books";

                LinkedResource res = new LinkedResource("wwwroot/images/slogo.png");
                res.ContentId = Guid.NewGuid().ToString();

                string body = @"
                <div style = ""background-color:#fff;margin:0 auto 0 auto;padding:30px 0 30px 0;color:#4f565d;font-size:13px;line-height:20px;font-family:""Helvetica Neue"",Arial,sans-serif;text-align:left;"">
                            <center>
                              <table style = ""width:550px;text-align:center"">
                                <tbody>
                                  <tr>
                                    <td colspan = ""2"" style = ""padding:30px 0;"">
                                      <img src='cid:" + res.ContentId + @"'/>
                                      <p style = ""color:#1d2227;line-height:28px;font-size:22px;margin:12px 10px 20px 10px;font-weight:400;""> Welcome to Jamin' Books.</p>
                                      <p style = ""color:#1d2227;margin:0 10px 10px 10px;padding:0;""> We'd like to make sure we got your email address right:</p>
                                      <p>
                                        <a style = ""display:inline-block;text-decoration:none;padding:15px 20px;background-color:#650d1b;border:1px solid #500A15;border-radius:3px;color:#FFF;font-weight:bold;"" href = """
                                            + callbackURL + @""" target = ""_blank"" > Confirm Email Address </ a >
                                      </p>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>
                            </center>
                          </div>";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, Password)
                };

                AlternateView alternateView = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
                alternateView.LinkedResources.Add(res);

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    IsBodyHtml = true
                })
                {
                    message.AlternateViews.Add(alternateView);
                    smtp.Send(message);
                    return true;
                }
            }catch(Exception e)
            {
                return false;
            }
        }

        public static string GenerateConfirmationCode()
        {
            const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
            char[] chars = new char[64];

            for (int i = 0; i < 64; i++)
            {
                chars[i] = allowedChars[RANDOM.Next(0, allowedChars.Length)];
            }

            return new string(chars);
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
