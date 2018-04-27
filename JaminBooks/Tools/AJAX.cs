using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace JaminBooks.Tools
{
    /// <summary>
    /// Provides methods for parsing JSON objects in HTML requests.
    /// </summary>
    public class AJAX
    {
        /// <summary>
        /// Converts a HTML request made by AJAX into a dictionary. All values will be strings.
        /// </summary>
        /// <param name="request">An HTML request made by AJAX</param>
        /// <returns>A dictionary of the fields from the data in the AJAX request. All values will be strings.</returns>
        public static Dictionary<string, string> GetFields(HttpRequest request)
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

        /// <summary>
        /// Converts a HTML request made by AJAX into a dictionary. Values will be objects.
        /// </summary>
        /// <param name="request">An HTML request made by AJAX</param>
        /// <returns>A dictionary of the fields from the data in the AJAX request. Values will be objects.</returns>
        public static Dictionary<string, object> GetObjectFields(HttpRequest request)
        {
            MemoryStream stream = new MemoryStream();
            request.Body.CopyTo(stream);
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream))
            {
                string requestBody = reader.ReadToEnd();
                if (requestBody.Length > 0)
                {
                    Dictionary<string, object> fields =
                        JsonConvert.DeserializeObject<Dictionary<string, object>>(requestBody);
                    if (fields != null)
                    {
                        return fields;
                    }
                }
            }

            throw new Exception("Invalid JSON Object");
        }
    }
}