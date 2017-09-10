using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace coonvey.Helpers
{
    public static class GenericHelpers
    {

        public static string[] ImageExtensions = { ".PNG", ".png", ".jpg", ".JPG", ".GIF", ".gif" };
        public static string[] DocumentExtensions = { ".xlsx", ".XLSX", ".xls", ".XLSX" };
        private static Random rand = new Random();
        private static readonly int maximumRegValue = 1000000;
        private static readonly string RegStrFormat = "D6";

        public static string getTimeStamp()
        {
            return DateTime.UtcNow.ToString("yyMMddHHmmssffff"); //e.g 20140112180244 //14digits
        }

        public static string generateSessionID(int num)
        {
            num = 8;
            const string nums = "0123456789";
            return "100023" + getTimeStamp() + new string(Enumerable.Repeat(nums, num)
            .Select(s => s[rand.Next(s.Length)]).ToArray());
            //return "100023" + new string(Enumerable.Repeat(nums, num)
            //.Select(s => s[rand.Next(s.Length)]).ToArray()) + getTimeStamp();
        }

        public static bool HasFile(this HttpPostedFileBase file)
        {
            return (file != null && file.ContentLength > 0) ? true : false;
        }
        public static Dictionary<string, string> AddToDictionary(List<Dictionary<string, string>> row)
        {
            Dictionary<string, string> dic_rows = new Dictionary<string, string>();
            foreach (Dictionary<string, string> r in row)
            {
                foreach (KeyValuePair<string, string> kvp in r)
                {
                    if (dic_rows.ContainsKey(kvp.Key))
                    {
                        dic_rows[kvp.Key] = kvp.Value;
                    }
                    else
                    {
                        dic_rows.Add(kvp.Key.ToString(), kvp.Value.ToString());
                    }
                    //string year = kvp.Key;
                    //string month = kvp.Value;
                }
            }
            return dic_rows;
        }

        public static bool ValidateEmail(string data)
        {
            return Regex.IsMatch(data, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        }

        public static string GetUserIPAddress()
        {
            var context = System.Web.HttpContext.Current;
            string ip = String.Empty;

            if (context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            else if (!String.IsNullOrWhiteSpace(context.Request.UserHostAddress))
                ip = context.Request.UserHostAddress;

            if (ip == "::1")
                ip = "127.0.0.1";

            return ip;
        }

        public static string formRegNum(int num)
        {
            const string nums = "0123456789";
            return new string(Enumerable.Repeat(nums, num)
            .Select(s => s[rand.Next(s.Length)]).ToArray());
        }
        public static string getRandomNum()
        {
            return rand.Next(maximumRegValue).ToString(RegStrFormat);
        }
        public static string getRandomChar()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //var random = new Random();
            return new string(
                Enumerable.Repeat(chars, 8)
                    .Select(s => s[rand.Next(s.Length)])
                    .ToArray());
        }
        public static string getRandomSpecialChar()
        {
            const string chars = "@#$%";
            //var random = new Random();
            return new string(
                Enumerable.Repeat(chars, 1)
                    .Select(s => s[rand.Next(s.Length)])
                    .ToArray());
        }

        public static string ToRelativeTime(this DateTime date)
        {
            int Minute = 60;
            int Hour = Minute * 60;
            int Day = Hour * 24;
            int Year = Day * 365;

            var thresholds = new Dictionary<long, Func<TimeSpan, string>>
                {
                    {2, t => "a second ago"},
                    {Minute,  t => String.Format("{0} seconds ago", (int)t.TotalSeconds)},
                    {Minute * 2,  t => "a minute ago"},
                    {Hour,  t => String.Format("{0} minutes ago", (int)t.TotalMinutes)},
                    {Hour * 2,  t => "an hour ago"},
                    {Day,  t => String.Format("{0} hours ago", (int)t.TotalHours)},
                    {Day * 2,  t => "yesterday"},
                    {Day * 30,  t => String.Format("{0} days ago", (int)t.TotalDays)},
                    {Day * 60,  t => "last month"},
                    {Year,  t => String.Format("{0} months ago", (int)t.TotalDays / 30)},
                    {Year * 2,  t => "last year"},
                    {Int64.MaxValue,  t => String.Format("{0} years ago", (int)t.TotalDays / 365)}
                };
            var difference = DateTime.UtcNow - date.ToUniversalTime();
            return thresholds.First(t => difference.TotalSeconds < t.Key).Value(difference);

        }

        public static string Date()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd HH:MM:sss"); ////Coordinated Universal Time, which is also called the Greenwich Mean Time time zon
        }

        public static string MarkLinks(this string input)
        {
            Regex reg = new Regex(@"((?:http|https):\/\/[a-z0-9\/\?=_#&%~-]+(\.[a-z0-9\/\?=_#&%~-]+)+)|(www(\.[a-z0-9\/\?=_#&%~-]+){2,})", RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(input);

            foreach (Match match in matches)
            {
                input = input.Replace(match.Value, "<a href='" + match.Value + "' target='_blank'>" + match.Value + "</a>");
            }
            input = input.Replace(Environment.NewLine, "<br />").Replace("\n\n", "<br />").Replace("\n", "<br />");
            return input;
        }

        public static string MarkHastags(this string input)
        {
            Regex reg = new Regex(@"\B#\w*[a-zA-Z]+\w*", RegexOptions.IgnoreCase);
            MatchCollection matches = reg.Matches(input);

            foreach (Match match in matches)
            {
                input = input.Replace(match.Value, "<a href='#' class='hashtag'>" + match.Value + "</a>");
            }
            return input;
        }

        public static string Prep(this string input)
        {
            return System.Security.SecurityElement.Escape(input).MarkLinks().MarkHastags();
        }

        public static string RenderPartialView(this Controller controller, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public static string Encrypt(string str)
        {
            string EncrptKey = "2013;[pnuLIT)WebCodeExpert";
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byKey = System.Text.Encoding.UTF8.GetBytes(EncrptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string str)
        {
            str = str.Replace(" ", "+");
            string DecryptKey = "2013;[pnuLIT)WebCodeExpert";
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byte[] inputByteArray = new byte[str.Length];

            byKey = System.Text.Encoding.UTF8.GetBytes(DecryptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(str.Replace(" ", "+"));
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }

        public static string NullToString(object Value)
        {

            // Value.ToString() allows for Value being DBNull, but will also convert int, double, etc.
            return Value == null ? "" : Value.ToString();
        }

    }

}