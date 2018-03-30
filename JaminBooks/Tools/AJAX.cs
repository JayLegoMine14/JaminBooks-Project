using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JaminBooks.Tools
{
    public class AJAX
    {
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
    }
}
