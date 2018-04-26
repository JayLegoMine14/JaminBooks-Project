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
    /// <summary>
    /// Manages all user and server authentication.
    /// </summary>
    public class Authentication
    {
        /// <summary>
        /// The email address of the web server.
        /// </summary>
        public static string Email;
        /// <summary>
        /// The password to the web servers email account.
        /// </summary>
        public static string Password;
        /// <summary>
        /// The name of the web server.
        /// </summary>
        public static string Name;
        /// <summary>
        /// A new random generator for creating confirmation codes.
        /// </summary>
        static Random RANDOM = new Random();

        /// <summary>
        /// Get the user currently logged in.
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>The user currently logged in</returns>
        public static User GetCurrentUser(HttpContext context)
        {
            int? ID = context.Session.GetInt32("UserID");
            return ID == null ? null : new User(ID.Value);
        }

        /// <summary>
        /// Logout the user from the current context.
        /// </summary>
        /// <param name="context">The current context</param>
        public static void LogoutCurrentUser(HttpContext context)
        {
            context.Session.Remove("UserID");
        }

        /// <summary>
        /// Determine if the given user exists.
        /// </summary>
        /// <param name="request">An HTML request containing the user data</param>
        /// <returns>Whether or not the user exists</returns>
        public static bool UserExists(HttpRequest request)
        {
            Dictionary<string, string> user = AJAX.GetFields(request);
            return User.Exists(user["Email"], Hash(user["Password"]));
        }

        /// <summary>
        /// Determine if the given email already exists.
        /// </summary>
        /// <param name="request">An HTML request containing the email</param>
        /// <returns>Whether of not the given email already exists</returns>
        public static bool EmailExists(HttpRequest request)
        {
            Dictionary<string, string> user = AJAX.GetFields(request);
            return User.Exists(user["Email"]);
        }

        /// <summary>
        /// Determine if the email already exists unless equal to the user's current email.
        /// </summary>
        /// <param name="request">An HTML request containing the email</param>
        /// <returns>Whether or not the given email exists</returns>
        public static bool EmailExistsWithException(HttpRequest request)
        {
            Dictionary<string, string> user = AJAX.GetFields(request);
            return new User(Convert.ToInt32(user["ID"])).Email != user["Email"]
                && User.Exists(user["Email"]);
        }

        /// <summary>
        /// Login the given user
        /// </summary>
        /// <param name="request">An HTML request containing the user data</param>
        /// <returns>And array of boolean values. The first value represents whether or not the login was successful, and the
        /// second whether or not the user was an admin</returns>
        public static bool[] SetCurrentUser(HttpRequest request)
        {
            Dictionary<string, string> user = AJAX.GetFields(request);
            int? UserID;
            if (User.Exists(user["Email"], Hash(user["Password"]), out UserID)
                && !new User(UserID.Value).IsDeleted)
            {
                request.HttpContext.Session.SetInt32("UserID", UserID.Value);
                return new bool[] { true, new User(UserID.Value).IsAdmin };
            }
            else return new bool[] { false };
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="request">An HTML request containing the user data</param>
        /// <param name="requireAdmin">Whether or not only an administrator can complete this action</param>
        /// <param name="login">Whether or not to login the user after creation</param>
        /// <param name="confirm">Whether or not to confirm the user's email address</param>
        /// <returns>The new user's id. 0 represents a failed creation attempt.</returns>
        public static int CreateUser(HttpRequest request, bool requireAdmin = false, bool login = true, bool confirm = true)
        {
            User currentUser = Authentication.GetCurrentUser(request.HttpContext);
            if (!requireAdmin || (currentUser != null && currentUser.IsAdmin))
                try
                {
                    Dictionary<string, string> creds = AJAX.GetFields(request);

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
                        if (confirm)
                            Task.Run(() => SendConfirmationEmail(request, user));
                        else
                        {
                            user.IsConfirmed = true;
                            user.Save();
                        }
                        if (login) request.HttpContext.Session.SetInt32("UserID", user.UserID);
                        return user.UserID;
                    }
                    else return 0;
                }
                catch (Exception e)
                {
                    return 0;
                }
            else return 0;
        }

        /// <summary>
        /// Send a confirmation email to the given user.
        /// </summary>
        /// <param name="request">A request from the host to which the email should link</param>
        /// <param name="u">The user to receive the email</param>
        /// <returns>Whether or not the email sent successfully</returns>
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
            }
            catch (Exception e)
            {
                return false;
            }
        }
        /// <summary>
        /// Generate a new 64 character confirmation code.
        /// </summary>
        /// <returns>A new 64 character confirmation code</returns>
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

        /// <summary>
        /// Hash the given data using SHA256
        /// </summary>
        /// <param name="data">The data to hash</param>
        /// <returns>The hashed data</returns>
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
