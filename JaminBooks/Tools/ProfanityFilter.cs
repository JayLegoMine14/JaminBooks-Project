using JaminBooks.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static JaminBooks.Model.SQL;

namespace JaminBooks.Tools
{
    public class ProfanityFilter
    {
        public static string Filter(string s)
        {
            string[] words = s.Split(" ");
            int index = 0;
            foreach(string w in words)
            {
                string word = new string(w.ToLower().Where(c => (char.IsLetterOrDigit(c))).ToArray());
                if (IsProfane(word))
                {
                    string replace = "";
                    foreach (char c in w.ToCharArray())
                        if (char.IsLetterOrDigit(c)) replace += "*";
                        else replace += c;

                    words[index] = replace;
                }
                index++;
            }
            return String.Join(" ", words);
        }

        public static bool IsProfane(string s)
        {
            DataTable result = SQL.Execute("uspIsProfane", new Param("Word", s));
            return result.Rows.Count > 0;
        }
    }
}
