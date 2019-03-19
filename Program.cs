
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net.Http;
using System.Net;


namespace HaveIBeenPwned
{
    public static class Program
    {
        static string Delete(this string str, char c) => str.Delete(c.ToString());
        static string Delete(this string str, string chars) => str.Replace(chars, "");
        static void Main(string[] args)
        {
            Console.Title = "HaveIBeenPwned?";
            Console.ForegroundColor = ConsoleColor.Green;
            while (true)
            {
                string password = Console.ReadLine();
                Console.Clear();
                byte[] byteString = Encoding.UTF8.GetBytes(password);
                byte[] Bytes = null;
                string hash = string.Empty;
                using (var sha1 = SHA1.Create())
                {
                    Bytes = sha1.ComputeHash(byteString);
                }
                StringBuilder sb = new StringBuilder();
                foreach (var b in Bytes) sb.Append(b.ToString("X2"));
                hash = sb.ToString();
                string firstFive = hash.Substring(0, 5);
                string hashRest = hash.Substring(5, hash.Length - 5);
                try
                {
                    string url = string.Format("https://api.pwnedpasswords.com/range/{0}", firstFive);
                    string response = new WebClient().DownloadString(url);
                    if (response.Contains(hashRest))
                    {
                        // You have been pwned
                        response = response.Delete('\r');
                        string amount = response.Substring(response.IndexOf(hashRest));
                        amount = amount.Substring(amount.IndexOf(":") + 1);
                        amount = amount.Substring(0, amount.IndexOf("\n"));
                        Console.WriteLine(string.Format("Das Passwort '{0}' wurde {1} mal in der Datenbank gefunden!", password, amount));
                    }
                    else Console.WriteLine(string.Format("Das Passwort '{0}' wurde bisher nicht geknackt!", password));
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine(ex.Message);
                }
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
